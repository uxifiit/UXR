//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Mvc;
//using UXR.Studies.Models;

//namespace UXR.Studies.ViewModels.Sessions
//{
//    public class StreamSettingViewModel
//    {
//        public StreamSettingViewModel()
//        {
//        }

//        public StreamSettingViewModel(StreamType streamType)
//        {
//            StreamType = streamType.Name;
//            Required = true;

//            if (StreamType == "Webcamera")  // TODO remove "Webcamera" string
//                Required = false;

//            if (streamType.StreamTypeOptions.Any())
//            {
//                Options = new DropDownListViewModel() { SelectedValue = streamType.StreamTypeOptions.First().Name, Items = streamType.StreamTypeOptions.Select(ss => new SelectListItem() { Text = ss.Name }) };
//            }
//        }
//        public StreamSettingViewModel(StreamType streamType, bool required, string selectedSetting)
//        {
//            StreamType = streamType.Name;
//            Required = required;
//            Options = null;
//            if (streamType.StreamTypeOptions.Any())
//            {
//                Options = new DropDownListViewModel() { SelectedValue = selectedSetting, Items = streamType.StreamTypeOptions.Select(ss => new SelectListItem() { Text = ss.Name }) };
//            }
//        }

//        public void Repopulate(StreamType streamType)
//        {
//            if (streamType.StreamTypeOptions.Any())
//            {
//                string selectedSetting = Options != null ? Options.SelectedValue : streamType.StreamTypeOptions.First().Name;
//                Options = new DropDownListViewModel() { SelectedValue = selectedSetting, Items = streamType.StreamTypeOptions.Select(ss => new SelectListItem() { Text = ss.Name }) };
//            }
//        }

//        [Display(Name = "Stream type")]
//        public string StreamType { get; set; }

//        [Display(Name = "Required")]
//        public bool Required { get; set; }

//        [Display(Name = "Options")]
//        public DropDownListViewModel Options { get; set; }
//    }
//}
