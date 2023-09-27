using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServer.Services.Interfaces;
using IdentityServer.ViewModels.Account;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GemSpaceIdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            CreateUser(host);

            host.Run();
        }

        private static void CreateUser(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var loginService = scope.ServiceProvider.GetRequiredService<ILoginService<ApplicationUser>>();
            var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();

            var model = new RegisterInputViewModel { Username = "valazom",
                Email = "valazom@gmail.com" , Password = "Az0m-12345@#"};

            var applicationUser = loginService.FindByEmail(model.Email).Result;

            var isAdministrator = false;

            if (applicationUser != null)
            {
                isAdministrator = roleService.IsUserInRole(applicationUser,
                                            "Administrator").Result;
            }
            else
            {
                var creationSuccess = loginService.CreateUserAsync(model).Result;

                if (!creationSuccess)
                {
                    throw new Exception("Error in creating Administrator");
                }
            }

            if (!isAdministrator)
            {
                var roleSuccess = roleService.AddUserToRole(applicationUser, "Administrator").Result;

                if (!roleSuccess) throw new Exception("Could not add user to administrator role");
            }

            
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
