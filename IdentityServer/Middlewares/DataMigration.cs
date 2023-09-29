using System;
using IdentityServer.Data.DbContext;
using IdentityServer.Models;
using IdentityServer.Services.Interfaces;
using IdentityServer.ViewModels.Account;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Middleware {
    public static class DataMigration {
        public static void DataMigrate (this IApplicationBuilder app) {
            var scope = app.ApplicationServices.CreateScope ();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationUserDbContext> ();

            dbContext.Database.Migrate();
        }
        public static void UseAdminUser (this IApplicationBuilder app) {
            var scope = app.ApplicationServices.CreateScope ();
            var loginService = scope.ServiceProvider.GetRequiredService<ILoginService<ApplicationUser>> ();
            var roleService = scope.ServiceProvider.GetRequiredService<IRoleService> ();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationUserDbContext> ();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration> ();

            var model = new RegisterInputViewModel {
                Username = "adminuser",
                Email = configuration["AdminEmail"], Password = configuration["AdminPassword"]
            };

            var applicationUser = loginService.FindByEmail (model.Email).Result;

            var isAdministrator = false;

            if (applicationUser != null) {
                isAdministrator = roleService.IsUserInRole (applicationUser,
                    "Administrator").Result;
            } else {
                var creationSuccess = loginService.CreateUserAsync (model).Result;

                if (!creationSuccess) {
                    throw new Exception ("Error in creating Administrator");
                }
            }

            if (!isAdministrator) {
                applicationUser = loginService.FindByEmail (model.Email).Result;
                var roleSuccess = roleService.AddUserToRole (applicationUser, "Administrator").Result;

                if (!roleSuccess) throw new Exception ("Could not add user to administrator role");
            }
        }
    }
}