using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Api.Entities.Sessions
{
    [DataContract(Name = "Recording")]
    public class SessionRecordingUpdate
    {
        [DataMember(IsRequired = false)]
        public int? SessionId { get; set; }

        [DataMember(IsRequired = false)]
        public string SessionName { get; set; }

        [DataMember]
        public List<string> Streams { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime? StartedAt { get; set; }
    }
}
