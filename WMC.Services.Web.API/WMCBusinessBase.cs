using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using WMC.Services.DAL;
using WMC.Services.Web.API.Classes;

namespace WMC.Services.Web.API
{
    public class WMCBusinessBase:BusinessBase
    {
        private ObjectCtxManager WCMCtxManager
        {
            get
            {
                return GetObjectContext<WMCDbEntities>("WMCDbEntities");
            }
        }

        /// <summary>
        /// Web configde bulunan OnlineParcaSiparisEntities isimli tag daki connection bigilerine göre yeni bir context oluşturur
        /// </summary>
         public WMCDbEntities WCMContext
        {
            get
            {
                return WCMCtxManager.EFContext as WMCDbEntities;
            }
        }

        /// <summary>
        /// Context de yeni bir transaction açar
        /// </summary>
        /// <returns></returns>
        protected TransactionScope GetTransaction()
        {
            return WCMCtxManager.GetTransaction();
        }
    }
}