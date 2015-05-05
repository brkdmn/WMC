using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WMC.Services.Contacts;
using WMC.Services.Contacts.General;

namespace WMC.Services.Web.API.Business
{
    internal class BsUserService:BaseBs
    {
        
        public Users GetUsersByUserNameAndPassword(string userName, string password)
        {
            var result= EfContext.Users.FirstOrDefault(p => p.UserName == userName && p.Password == password);

            return result;
        }

        public Users GetUserByUserId(int id)
        {
            var result= EfContext.Users.FirstOrDefault(p => p.UserId == id);

            return result;
        }

        public Users SaveUser(Users user)
        {
            if (EfContext.Users.Count(p => p.UserName == user.UserName) > 0)
            {
                return null;
            }
            else
            {
                EfContext.Users.Add(user);
                EfContext.SaveChanges();
                return user;
            }
            
        }

        public bool IsUserByUserNameAndPassword(string userName, string password)
        {
            if (EfContext.Users.Count(p => p.UserName == userName && p.Password == password) > 0)
                return true;
            else
                return false;
        }

        public List<Users>GetUserList()
        {
            var result = EfContext.Users.ToList();
            return result;
        }
    }
}