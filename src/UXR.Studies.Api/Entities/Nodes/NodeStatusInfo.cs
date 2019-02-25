using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Api.Entities.Nodes
{
    [DataContract(Name = "Node")]
    public class NodeStatusInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsRecording { get; set; }

        [DataMember(IsRequired = false)]
        public string CurrentSession { get; set; }

        [DataMember]
        public DateTime LastUpdateAt { get; set; }
    }
}
