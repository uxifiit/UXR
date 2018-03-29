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
    public class Project : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        public string SessionDefinitionTemplate { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
