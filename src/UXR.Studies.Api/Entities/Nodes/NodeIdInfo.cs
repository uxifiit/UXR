using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Api.Entities.Nodes
{
    [DataContract(Name = "Node")]
    public class NodeIdInfo
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Id { get; set; }
    }
}
