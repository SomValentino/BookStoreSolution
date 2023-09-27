using System;
using AutoMapper;
using IdentityServer.ViewModels.Admin;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.AutoMapper
{
    public class IdentityServerProfile : Profile
    {
        public IdentityServerProfile()
        {
            CreateMap<ApiScopeViewModel, ApiScope>();
        }
    }
}
