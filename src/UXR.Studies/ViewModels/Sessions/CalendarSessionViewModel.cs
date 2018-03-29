using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.ViewModels.Users;

namespace UXR.Studies.ViewModels.Sessions
{
    public class CalendarSessionViewModel
    {
        [Display(Name = "Project")]
        public string ProjectName { get; set; }

        [Display(Name = "Owner")]
        public UserNameViewModel ProjectOwner { get; set; }

        [Display(Name = "Start time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:HH\\:mm}")]
        public DateTime StartTime { get; set; }

        [Display(Name = "End time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:HH\\:mm}")]
        public DateTime EndTime { get; set; }
    }
}
