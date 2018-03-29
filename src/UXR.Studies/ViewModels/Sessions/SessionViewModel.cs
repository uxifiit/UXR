using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.ViewModels.Sessions
{
    /// <summary>
    /// <see cref="UXR.Studies.Models.Session" /> 
    /// </summary>
    public class SessionViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Start time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Length")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan Length { get; set; }

        [AutoMapper.IgnoreMap]
        [Display(Name = "Recordings")]
        public virtual int RecordingsCount { get; set; }
    }
}
