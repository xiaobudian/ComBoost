﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Security
{
    public class ComBoostAuthenticationProvider : IAuthenticationProvider
    {
        public ComBoostAuthenticationProvider(IHttpContextAccessor accessor)
        {
            if (accessor == null)
                throw new ArgumentNullException(nameof(accessor));
            if (accessor.HttpContext == null)
                throw new ArgumentException("当前环境不存在Http上下文。", "accessor");
            Context = accessor.HttpContext;
        }

        public HttpContext Context { get; private set; }

        public IAuthentication GetAuthentication()
        {
            var authentication = Context.User as ComBoostPrincipal;
            if (authentication == null)
                return new ComBoostAnonymousPrincipal();
            return authentication;
        }

        public async Task<bool> SignInAsync(IDictionary<string, string> properties)
        {
            var securityProvider = Context.RequestServices.GetRequiredService<ISecurityProvider>();
            var permission = await securityProvider.GetPermissionAsync(properties);
            if (permission == null)
                return false;
            await SignInAsync(permission);
            return true;
        }

        public Task SignInAsync(IPermission permission)
        {
            var securityProvider = Context.RequestServices.GetRequiredService<ISecurityProvider>();
            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity identity = new ClaimsIdentity("ComBoostAuthentication", ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaims(permission.GetStaticRoles().Select(t => new Claim(ClaimTypes.Role, securityProvider.ConvertRoleToString(t))));
            identity.AddClaim(new Claim(ClaimTypes.Name, permission.Name));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, permission.Identity));
            principal.AddIdentity(identity);
            return Context.SignInAsync("ComBoost", principal);
        }

        public Task SignOutAsync()
        {
            return Context.SignOutAsync("ComBoost");
        }
    }
}
