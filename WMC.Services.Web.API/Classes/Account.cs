using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMC.Services.DAL;

namespace WMC.Services.Web.API.Classes
{
    public class Account
    {
        public bool IsUserByUSerNameAndPAssword(string userName, string password)
        {
            using(WMCDbEntities ctx=new WMCDbEntities())
            {
                if (ctx.Users.Count(p => p.UserName == userName && p.Password == password) > 0)
                    return true;
                else
                    return false;
            }
        }
    }
}