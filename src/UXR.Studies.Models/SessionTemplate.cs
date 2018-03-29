using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Models.Entities;

namespace UXR.Studies.Models
{
    public class SessionTemplate : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }

        public string Definition { get; set; }

        public virtual ApplicationUser Author { get; set; }
    }
}
