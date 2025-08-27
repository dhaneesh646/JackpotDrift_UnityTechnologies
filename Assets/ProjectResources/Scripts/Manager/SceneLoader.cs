namespace OneXR.WorkFlow
{
    namespace Common
    {
        public class SceneLoader
        {
            public static void LoadAdditiveSceneAsync(string sceneName)
            {
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            }
            public static void UnloadSceneAsync(string sceneName)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            }
        }
    }
}