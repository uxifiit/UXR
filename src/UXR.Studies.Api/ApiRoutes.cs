using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Api
{
    static class ApiRoutes
    {
        public const string PREFIX = "api/studies";

        public static class Session
        {
            public const string PREFIX = ApiRoutes.PREFIX + "/session";

            public const string PARAM_SESSION_ID = @"{sessionId:regex([0-9]+)}";

            // api/studies/session/now
            public const string ACTION_SESSIONS_NOW = "now";

            public static string ResolveCurrentSessionsRoute()
            {
                return PREFIX + "/"
                     + ACTION_SESSIONS_NOW;
            }
        }

        public static class Node
        {
            public const string PREFIX = ApiRoutes.PREFIX + "/node";

            public const string PARAM_NODE_ID = @"{nodeId:regex([0-9]+)}";


            public const string ACTION_LIST = "";
            public static string ResolveListRoute()
            {
                return PREFIX + "/"
                     + ACTION_LIST;
            }


            public const string ACTION_UPDATE = "";
            public static string ResolveUpdateRoute()
            {
                return PREFIX + "/"
                     + ACTION_UPDATE;
            }


            public static class Recording
            {
                public const string PREFIX = Node.PREFIX + "/" + PARAM_NODE_ID + "/recording";

                private static string ResolvePrefix(int nodeId)
                {
                    return PREFIX.Replace(PARAM_NODE_ID, nodeId.ToString());
                }


                public const string DATETIME_FORMAT = "yyyy'-'MM'-'dd'T'HH'_'mm'_'ss"; // we ignore milliseconds and time zone now '-'fffffffK";
                // corresponds to the DateTime Round-trip format O but without ':' and '.': 
                // yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK
                public const string PARAM_STARTTIME = @"{startTime:regex(\d{4}-\d{2}-\d{2}T\d{2}_\d{2}_\d{2})}"; // \-\d{7}(([\+\-]\d{2}_\d{2})|Z)?)}";

                public static string ConvertToRouteString(DateTime dateTime)
                {
                    return dateTime.ToString(DATETIME_FORMAT);
                }


                public static bool TryParseDateTimeParameter(string dateTimeString, out DateTime dateTime)
                {
                    return DateTime.TryParseExact(dateTimeString,
                                                  DATETIME_FORMAT,
                                                  System.Globalization.CultureInfo.InvariantCulture,
                                                  System.Globalization.DateTimeStyles.AssumeLocal,
                                                  out dateTime);
                }


                public const string ACTION_UPLOAD = PARAM_STARTTIME;

                public static string ResolveUploadRoute(int nodeId, DateTime startTime)
                {
                    return ResolvePrefix(nodeId) + "/"
                         + ACTION_UPLOAD.Replace(PARAM_STARTTIME, ConvertToRouteString(startTime));
                }


                private const string PARAM_SESSION_ID = "{sessionId:int?}";

                public const string ACTION_SAVE = PARAM_STARTTIME + "/save/" + PARAM_SESSION_ID;

                public static string ResolveSaveUnassignedRoute(int nodeId, DateTime startTime)
                {
                    return ResolvePrefix(nodeId) + "/"
                         + ACTION_SAVE.Replace(PARAM_STARTTIME, ConvertToRouteString(startTime))
                                      .Replace(PARAM_SESSION_ID, "");
                }

                public static string ResolveSaveRoute(int nodeId, DateTime startTime, int sessionId)
                {
                    return ResolvePrefix(nodeId) + "/"
                         + ACTION_SAVE.Replace(PARAM_STARTTIME, ConvertToRouteString(startTime))
                                      .Replace(PARAM_SESSION_ID, sessionId.ToString());
                }


                public const string ACTION_LIST = PARAM_STARTTIME;

                public static string ResolveListRoute(int nodeId, DateTime startTime)
                {
                    return ResolvePrefix(nodeId) + "/"
                         + ACTION_LIST.Replace(PARAM_STARTTIME, ConvertToRouteString(startTime));
                }
            }
        }


        public static class Status
        {
            public const string PREFIX = ApiRoutes.PREFIX + "/status";

            public const string ACTION_INDEX = "";

            public static string ResolveIndexRoute()
            {
                return PREFIX + "/"
                     + ACTION_INDEX;
            }
        }
    }
}
