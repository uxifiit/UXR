using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Models.Entities;

namespace UXR.Studies.Models
{
    public class Node : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int? GroupId { get; set; }

        public virtual Group Group { get; set; }

        public virtual NodeStatus Status { get; set; }
    }
}
