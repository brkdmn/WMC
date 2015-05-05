using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using WMC.Services.Web.API.Business;
using WMC.Services.DAL;
using WMC.Services.Web.API.Classes;

namespace WMC.Services.Web.API.AuthenticationHelper
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            bool isValidUser = false;
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            Account account = new Account();

            if (account.IsUserByUSerNameAndPAssword(context.UserName,context.Password))
            {
                isValidUser = true;
            }

            if (!isValidUser)
            {
                context.SetError("invalid_grant", "Kullanıcı ad yada şifre hatalı");
                return;
            }

            //Şimdilik örnek olarak bir kullanıcı tipi ve rolü gönder..
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            context.Validated(identity);
        }
    }
}