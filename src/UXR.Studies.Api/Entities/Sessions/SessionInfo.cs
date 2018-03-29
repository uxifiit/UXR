using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace UXR.Studies.Api.Entities.Sessions
{
    [DataContract(Name = "Session")]
    public class SessionInfo
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Project { get; set; }

        [DataMember]
        public string Owner { get; set; }

        [DataMember]
        public DateTime CreatedAt { get; set; }

        [DataMember]
        public JRaw Definition { get; set; }
    }
}
