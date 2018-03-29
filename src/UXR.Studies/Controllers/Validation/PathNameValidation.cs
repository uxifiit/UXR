using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UXR.Studies.Controllers.Validation
{
    class PathNameValidation
    {
        private const string forbiddenCharactersMessage = "Name cannot contain \" < > | : * ? \\ /";

        public static bool CheckContainsNoForbiddenPathCharacters(ModelStateDictionary ModelState, string property, string value)
        {
            var forbiddenCharacters = Path.GetInvalidFileNameChars();

            if (value.IndexOfAny(forbiddenCharacters) != -1)
            {
                ModelState.AddModelError(nameof(property), forbiddenCharactersMessage);
                return false;
            }

            return true;
        }
    }
}
