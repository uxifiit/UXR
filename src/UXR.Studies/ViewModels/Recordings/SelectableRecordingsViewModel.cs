using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.ViewModels.Recordings
{
    public class SelectableRecordingsViewModel
    {
        public SelectableRecordingsViewModel()
        {

        }

        public SelectableRecordingsViewModel(List<RecordingViewModel> recordings)
        {
            Recordings = recordings;
            Selections = Enumerable.Repeat(true, recordings.Count).ToList();
        }

        public List<RecordingViewModel> Recordings { get; set; }

        public List<bool> Selections { get; set; }
    }
}
