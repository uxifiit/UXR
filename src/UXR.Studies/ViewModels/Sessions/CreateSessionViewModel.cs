using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.Models;
using UXR.Studies.ViewModels.Recordings;

namespace UXR.Studies.ViewModels.Sessions
{
    public class CreateSessionViewModel : CreateSessionViewModelPost
    {
        public int ProjectId { get; set; }
    }

    public class CreateSessionViewModelPost
    {
        private string name = String.Empty;

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get { return name; } set { name = value?.Trim() ?? String.Empty; } }

        [Display(Name = "Start time")]
        [Required(ErrorMessage = "Please set when the session is going to start.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [Display(Name = "Length")]
        [Required(ErrorMessage = "Please set the length of the session.")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan Length { get; set; }

        private string definition = String.Empty;

        [Display(Name = "Definition")]
        [DataType(DataType.MultilineText)]
        public string Definition { get { return definition; } set { definition = value?.Trim() ?? String.Empty; } }

        public bool UseProjectDefinitionTemplate { get; set; }
    }
}
