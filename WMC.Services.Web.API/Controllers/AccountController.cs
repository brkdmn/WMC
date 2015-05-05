using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WMC.Services.Contacts;
using WMC.Services.Web.API.Business;
using WMC.Services.DAL;
using WMC.Services.Contacts.General;

namespace WMC.Services.Web.API.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : WMCBusinessBase
    {
        
        [AllowAnonymous]
        [Route("Register")]
        public async Task<System.Web.Http.IHttpActionResult> Register(Users user)
        {
            user.CreateTime = DateTime.Now;
            user.CreateUserId = 1;
            user.UpdateTime = DateTime.Now;
            user.UpdateUserId = 1;
            var result= BsFactory<BsUserService>.Instance(WCMContext).SaveUser(user);
            if(result!=null)
            {
                return Ok();
            }else
            {
                return ExecuteResponse("Bu Kayıt daha önce eklenmiştir.");
            }

            

        }
	}
}