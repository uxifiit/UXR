using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.ViewModels.Recordings
{
    public class RecordingViewModel
    {
        [Display(AutoGenerateField = false)]
        public string ProjectName { get; set; }

        [Display(AutoGenerateField = false)]
        public string SessionName { get; set; }

        [Display(Name = "Node name")]
        public string NodeName { get; set; }

        [Display(Name = "Start time")]
        public DateTime StartTime { get; set; }
    }
}
