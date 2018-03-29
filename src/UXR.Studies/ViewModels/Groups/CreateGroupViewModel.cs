using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.ViewModels.Groups
{
    public class CreateGroupViewModel
    {
        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }
    }
}
