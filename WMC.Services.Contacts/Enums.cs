using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Services.Contacts
{
    [DataContract]
    public enum EnumErrorType
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        ConnectionError,
        [EnumMember]
        ApplicationError,
        [EnumMember]
        RecordNotFound,
        [EnumMember]
        UnknownError,
        [EnumMember]
        TokenExpiredError,
        [EnumMember]
        NoApplicationAccess,
        [EnumMember]
        CustomError,
        [EnumMember]
        NotSecondRecord
    }
}
