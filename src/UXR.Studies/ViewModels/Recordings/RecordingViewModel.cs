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
        public RecordingViewModel() { }

        public RecordingViewModel(RecordingViewModel recording)
        {
            NodeName = recording.NodeName;
            StartTime = recording.StartTime;
        }

        [Display(Name = "Node name")]
        public string NodeName { get; set; }

        [Display(Name = "Start time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH\\:mm}")]
        public virtual DateTime StartTime { get; set; }
    }
}
