using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.ViewModels.SessionTemplates
{
    public class CreateSessionTemplateViewModel
    {
        private string name = String.Empty;

        [Display(Name = "Name")]
        [Required]
        public string Name { get { return name; } set { name = value?.Trim() ?? String.Empty; } }


        private string definition = String.Empty;

        [Display(Name = "Template")]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Definition { get { return definition; } set { definition = value?.Trim() ?? String.Empty; } }
    }
}
