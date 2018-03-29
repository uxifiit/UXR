using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.Api.Entities.Sessions;

namespace UXR.Studies.Api.Entities.Nodes
{
    [DataContract(Name = "Node")]
    public class NodeStatusUpdate
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember(IsRequired = false)]
        public SessionRecordingUpdate Recording { get; set; }
    }
}
