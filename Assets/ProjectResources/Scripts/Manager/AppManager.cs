using OneXR.WorkFlow.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    public UserDatas userDatas;
    public UserDatas UserDatas { get => userDatas; set => userDatas = value; }
    private static AppManager _instance;


    public static AppManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = FindAnyObjectByType(typeof(AppManager)) as AppManager;
                _instance = obj;
            }

            if (_instance == null)
            {
                var go = new GameObject("AppManager");
                _instance = go.AddComponent<AppManager>();
            }

            return _instance;
        }
    }

    void Start()
    {
        
        SceneLoader.LoadAdditiveSceneAsync("Login");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
