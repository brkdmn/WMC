using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
namespace WMC.Services.Contacts.General
{
    [DataContract]
    public class BaseResponse
    {
        private EnumErrorType _errorType = EnumErrorType.None;
        [DataMember]
        public EnumErrorType ErrorType
        {
            get { return _errorType; }
            set
            {
                HasException = value != EnumErrorType.None;
                _errorType = value;
            }
        }

        [DataMember]
        public string ErrorMessage { get; set; }

        public BaseResponse()
        {
            HasException = false;
        }

        [DataMember]
        public bool HasException { get; private set; }


        public bool IsSuccess { get { return !HasException; } }

    }

    [DataContract]
    public class MethodResponse<T> : BaseResponse
    {
        [DataMember]
        public T Data { get; set; }
    }
}

