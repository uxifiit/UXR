using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.Models;
using UXR.Models;

namespace UXR.Studies
{
    public class StudiesDbInitializer : IPartialDbInitializer<UXRDbContext>
    {
        private string CreateDefaultSessionDefinition()
        {
            return @"
{ 
  ""devices"": [
    { ""device"": ""ET"" }, 
    { ""device"": ""EXTEV"" },
    { ""device"": ""KB"" }, 
    { ""device"": ""ME"" },
    { ""device"": ""WCV"" }, 
    { ""device"": ""SC"" }
  ],
  ""welcome"": {
    ""ignore"": true
  },
  ""preSessionSteps"": [
    { ""action"": { ""actionType"": ""EyeTrackerCalibration"" } } 
  ],
  ""sessionSteps"": [
    { ""action"": { ""actionType"": ""EyeTrackerValidation"" } },
    { 
      ""action"": { ""actionType"": ""ShowDesktop"", ""minimizeAll"": false }, 
      ""completion"": { ""hotkeys"": [ ""F10"" ] } 
    }
  ]
}".Trim();
        }

        public void Seed(UXRDbContext context)
        {

            if (context.SessionTemplates.Any() == false)
            {
                var user = context.Users.FirstOrDefault();
                if (user != null)
                {
                    context.SessionTemplates.Add(new Studies.Models.SessionTemplate()
                    {
                        Name = "Default Template",
                        Author = user,
                        Definition = CreateDefaultSessionDefinition()
                    });
                }
            }
        }
    }
}
