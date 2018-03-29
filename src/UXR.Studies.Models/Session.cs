using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Models.Entities;

namespace UXR.Studies.Models
{
    public class Session : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        // TODO [Required]
        public TimeSpan Length { get; set; }

        public int ProjectId { get; set; }

        [Required]
        public virtual Project Project { get; set; }

        public string Definition { get; set; }
    }
}
