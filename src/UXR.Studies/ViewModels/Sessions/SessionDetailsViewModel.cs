using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.ViewModels.Recordings;

namespace UXR.Studies.ViewModels.Sessions
{
    public class SessionDetailsViewModel : SessionViewModel
    {
        public int ProjectId { get; set; }

        [Display(Name = "Project")]
        public string ProjectName { get; set; }

        [AutoMapper.IgnoreMap]
        public List<RecordingViewModel> Recordings { get; set; } = new List<RecordingViewModel>();

        [AutoMapper.IgnoreMap]
        public override int RecordingsCount
        {
            get
            {
                return Recordings?.Count() ?? 0;
            }
            set { }
        }
    }
}
