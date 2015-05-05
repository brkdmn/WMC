using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using WMC.Services.Contacts;
using WMC.Services.Contacts.General;
using WMC.Services.DAL;

namespace WMC.Services.Web.API.Business
{
    internal static class BsFactory<T> where T : BaseBs, new()
    {
        public static T Instance(WMCDbEntities ctx)
        {
            var instance = new T { EfContext = ctx };
            return instance;
        }
    }

    public abstract class BaseBs
    {
        private WMCDbEntities _efContext;

        public WMCDbEntities EfContext
        {
            get { return this._efContext; }
            set
            {
                if (Equals(this._efContext, value)) return;

                var oldValue = this._efContext;
                this._efContext = value;
                this.OnContextChanged(oldValue, value);

            }
        }
        protected virtual void OnContextChanged(WMCDbEntities oldContext, WMCDbEntities newContext)
        {
        }

        #region Helper Methods

        internal static MethodResponse<T> Response<T>(T data)
        {
            return new MethodResponse<T>
            {
                Data = data,
                ErrorType = EnumErrorType.None,
                ErrorMessage = null
            };
        }
        internal static MethodResponse<T> Response<T>(Exception x)
        {
            EnumErrorType errorType;
            // var exceptionType = x.GetType();
            // if (exceptionType == typeof())
            // {
            //     errorType =
            // }
            // else if (exceptionType == typeof())
            // {
            //     errorType =
            // }
            // else
            // {
            errorType = EnumErrorType.UnknownError;
            // }

            return Response<T>(errorType, x.Message);
        }
        internal static MethodResponse<T> Response<T>(EnumErrorType errorType, string message)
        {
            return new MethodResponse<T>
            {
                Data = default(T),
                ErrorType = errorType,
                ErrorMessage = message
            };
        }
        internal static MethodResponse<T> Response<T>(BaseResponse response)
        {
            if (response.IsSuccess)
                throw new ArgumentException("You can't raise a successful response up.");

            return new MethodResponse<T>
            {
                Data = default(T),
                ErrorType = response.ErrorType,
                ErrorMessage = response.ErrorMessage
            };
        }
        internal static IQueryable<T> OrderBy<T>(IQueryable<T> source, string ordering)
        {
            var type = typeof(T);
            var property = type.GetProperty(ordering);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            var resultExp = Expression.Call(
                typeof(Queryable),
                "OrderBy",
                new[] { type, property.PropertyType },
                source.Expression,
                Expression.Quote(orderByExp));

            return source.Provider.CreateQuery<T>(resultExp);
        }
        #endregion
    }
}