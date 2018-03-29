using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UXR.Studies.Controllers.Validation
{
    class SessionDefinitionValidation
    {
        public static bool CheckDefinitionTemplateJson(ModelStateDictionary ModelState, string property, string definitionJson)
        {
            string json = definitionJson?.Trim();
            if (String.IsNullOrEmpty(json) == false
                && json.StartsWith("{") && json.EndsWith("}"))
            {
                try
                {
                    var jToken = Newtonsoft.Json.Linq.JToken.Parse(json);
                    if (jToken.HasValues == false)
                    {
                        ModelState.AddModelError(property, "Definition must not be empty.");
                    }

                    return jToken.HasValues;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(property, ex);
                }
            }
            else
            {
                ModelState.AddModelError(property, "Definition is not valid JSON object.");
            }
            return false;
        }
    }
}
