using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.ViewModels.GroupNodes
{
    public class CreateNodeViewModel
    {
        [Display(Name = "Name")]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Name { get; set; }

        public string GroupName { get; set; }
    }
}
