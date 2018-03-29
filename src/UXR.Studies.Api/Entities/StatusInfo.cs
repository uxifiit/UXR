using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Api.Entities
{
    [DataContract(Name = "Status")]
    public class StatusInfo
    {
        [DataMember]
        public string ApiVersion { get; set; }
    }
}
