using System.ComponentModel;

namespace OneXR.WorkFlow
{
    namespace Common
    {
        //-----------------------------------------> Scene specific enum STARTS HERE
        public enum SceneName
        {
            [Description("Application")]
            APPLICATION,
            [Description("SplashScene")]
            SPLASH,
            [Description("Login")]
            LOGIN,
            [Description("Dashboard")]
            DASHBOARD,
            [Description("Home")]
            HOME,
            [Description("Settings")]
            Settings,
            [Description("Streaming")]
            STREAMING,
            [Description("StreamingServer")]
            STREAMINGSERVER,
            [Description("ProjectSceneLoader")]
            PROJECTSCENELOADER,
            [Description("Project")]
            PROJECT,
            [Description("WebJoinRoom")]
            WebGL
        }
    }
}