namespace UnityGameFramework.Runtime
{
    public static class InputCmdKey
    {
        public static class Group
        {
            public static string Constructing = "Constructing";
            public static string Common = "Common";
            public static string Topdown = "Topdown";
            public static string SpeedControl = "SpeedControl";
            public static string AbilityCast = "AbilityCast";
        }

        public static class Action
        {
            #region Common

            public static string MouseLeftClick = "MouseLeftClick";
            public static string MouseRightClick = "MouseRightClick";
            public static string MousePosition = "MousePosition";
            public static string Escape = "Escape";
            public static string AltHotKey = "AltHotKey";
            public static string CtrlHotKey = "CtrlHotKey";

            #endregion

            #region Constructing

            public static string SwitchConstructStatus = "SwitchConstructStatus";
            public static string ConstructOrbit = "ConstructOrbit";
            public static string ConstructPlanet = "ConstructPlanet";

            #endregion


            #region Topdown

            public static string Zoom = "Zoom";
            public static string Focus = "Focus";
            public static string Reposition = "Reposition";
            public static string Confirm = "Confirm";
            public static string DragActivation = "DragActivation";
            public static string DragPerform = "DragPerform";

            #endregion

            #region Speed Control
            public static string SpeedControl0 = "SpeedControl_0";
            public static string SpeedControl1 = "SpeedControl_1";
            public static string SpeedControl2 = "SpeedControl_2";
            public static string SpeedControl3 = "SpeedControl_3";
            #endregion

            #region AbilityCast

            public static string AbilityCastPrefix = "AbilityCast_";
            public static string AbilityCastFollowPosition = "SelectorPosition";
            public static string AbilityCastConfirm = "SelectorConfirm";
            public static string AbilityCastCancel = "SelectorCancel";

            #endregion
        }
    }
}