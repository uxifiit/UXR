using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Models.Entities;

namespace UXR.Studies.Models
{
    public class NodeStatus : BaseEntity
    {
        [Key, ForeignKey(nameof(Node))]
        public int NodeId { get; set; }

        public virtual Node Node { get; set; }

        public bool IsRecording { get; set; }

        public string CurrentSession { get; set; }
    }
}
