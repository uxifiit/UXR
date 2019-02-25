using System;
using System.ComponentModel.DataAnnotations;

namespace UXR.Studies.ViewModels.Recordings
{
    public class SelectableRecordingViewModel : RecordingViewModel
    {
        public SelectableRecordingViewModel() { }

        public SelectableRecordingViewModel(RecordingViewModel recording)
            : base(recording)
        { }

        public bool IsSelected { get; set; }

        [Display(Name = "Start time")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH\\:mm}")]
        [UIHint("HiddenDate")]
        public override DateTime StartTime { get => base.StartTime; set => base.StartTime = value; }
    }
}
