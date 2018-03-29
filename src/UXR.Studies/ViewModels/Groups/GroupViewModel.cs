using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.ViewModels.Groups
{
    public class GroupViewModel
    {
        [Display(AutoGenerateField = false)]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Nodes")]
        public int NodesCount { get; set; }
    }
}
