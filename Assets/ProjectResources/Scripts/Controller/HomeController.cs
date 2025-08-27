using System;
using Firebase.Database;
using OneXR.WorkFlow.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeController : MonoBehaviour
{
    public HomeView homeView;
    [SerializeField] Button playButton;
    void Start()
    {
        homeView.SetUserDetails(AppManager.Instance.userDatas.UserName);
        playButton.onClick.AddListener(LoadGameScene);
    }

    private void LoadGameScene()
    {
        SceneManager.sceneUnloaded += HomeSceneUnloaded;
        SceneLoader.UnloadSceneAsync("Home");
    }

    private void HomeSceneUnloaded(Scene scene)
    {
        SceneManager.sceneUnloaded -= HomeSceneUnloaded;
        SceneLoader.LoadAdditiveSceneAsync("Game");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
