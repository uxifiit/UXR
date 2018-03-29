namespace UXR.Studies
{
    public static class Routes
    {
        public const string ACTION_CREATE = "create";
        public const string ACTION_DELETE = "delete";
        public const string ACTION_EDIT = "edit";
        public const string ACTION_INDEX = "index";
        public const string ACTION_DETAILS = "details";

        public const string AREA_NAME = "studies";

        public static class Project
        {
            public const string PREFIX = "project";

            public const string PARAM_PROJECT_ID = "{projectId:int}";

            public const string ACTION_DETAILS = PARAM_PROJECT_ID;
            public const string ACTION_EDIT = PARAM_PROJECT_ID + "/" + Routes.ACTION_EDIT;
            public const string ACTION_DELETE = PARAM_PROJECT_ID + "/" + Routes.ACTION_DELETE;
        }


        public static class Session
        {
            public const string PREFIX = "session";

            public const string PARAM_SESSION_ID = "{sessionId:int}";

            public const string ACTION_CALENDAR = "calendar";

            public const string ACTION_DETAILS = PARAM_SESSION_ID;
            public const string ACTION_EDIT = PARAM_SESSION_ID + "/" + Routes.ACTION_EDIT;
            public const string ACTION_DELETE = PARAM_SESSION_ID + "/" + Routes.ACTION_DELETE;
            public const string ACTION_DOWNLOAD = PARAM_SESSION_ID + "/" + "download";

            public const string PARAM_PROJECT_ID = "{projectId:int}";

            public const string ACTION_CREATE = Routes.ACTION_CREATE + "/" + PARAM_PROJECT_ID;


            public static class Template
            {
                public const string PREFIX = Session.PREFIX + "/" + "template";

                public const string PARAM_TEMPLATE_ID = "{templateId:int}";

                public const string ACTION_DELETE = PARAM_TEMPLATE_ID + "/" + Routes.ACTION_DELETE;
            }
        }


        public static class Recording
        {
            public const string PREFIX = "recording";

            public const string ACTION_ASSIGN_SESSION = "assign";
        }


        public static class Group
        {
            public const string PREFIX = "group";

            public const string PARAM_GROUP_ID = "{groupId:int}";

            public const string ACTION_DETAILS = PARAM_GROUP_ID;
            public const string ACTION_EDIT = PARAM_GROUP_ID + "/" + Routes.ACTION_EDIT;
            public const string ACTION_DELETE = PARAM_GROUP_ID + "/" + Routes.ACTION_DELETE;


            public static class Node
            {
                public const string PREFIX = Group.PREFIX + "/" + PARAM_GROUP_ID + "/node";


                public const string PARAM_NODE_ID = "{nodeId:int}";

                public const string ACTION_DELETE = PARAM_NODE_ID + "/" + Routes.ACTION_DELETE;
            }
        }


        public static class Dashboard
        {
            public const string PREFIX = "dashboard";
            
            public const string ACTION_NODES = "NodeStatusBoard";
        }
    }
}
