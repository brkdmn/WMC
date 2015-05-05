using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WMC.Services.Contacts.General
{
    public class MessageResponse :ApiController
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

        public MessageResponse()
        {
            HasException = false;
        }

        [DataMember]
        public bool HasException { get; private set; }


        public bool IsSuccess { get { return !HasException; } }

        string _value;
        HttpRequestMessage _request;

        public MessageResponse(string value, HttpRequestMessage request)
        {
            _value = value;
            _request = request;
        }
        public IHttpActionResult ExecuteResponse(string message)
        {
            ModelState.AddModelError("", message);
            return BadRequest(ModelState);
        }
    }
}
