using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WMC.Services.Contacts;
using WMC.Services.Web.API.Business;

namespace WMC.Services.Web.API.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : WMCBusinessBase
    {
        
        public Users GetUsersByUserNameAndPassword(string userName,string password)
        {
            return BsFactory<BsUserService>.Instance(WCMContext).GetUsersByUserNameAndPassword(userName, password);
        }

        
        [AllowAnonymous]
        [Route("UserList")]
        public IHttpActionResult GetUserList()
        {
            var data = BsFactory<BsUserService>.Instance(WCMContext).GetUserList();

            return Ok(data);
        }
    }
}