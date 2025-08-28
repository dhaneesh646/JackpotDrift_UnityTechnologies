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
    [SerializeField] GameObject gamePanel;
    [SerializeField] Button machineGame;
    [SerializeField] Button diceGame;
    private string sceneToLoad;
    void Start()
    {
        homeView.SetUserDetails(AppManager.Instance.userDatas.UserName);
        playButton.onClick.AddListener(DesplaHomePanel);
        machineGame.onClick.AddListener(() =>
        {
            LoadGameScene("SlotMachine_Game");
        });
        diceGame.onClick.AddListener(() =>
        {
            LoadGameScene("Dice_Game");
        });
    }

    void DesplaHomePanel()
    {
        gamePanel.SetActive(true);
    }

    private void LoadGameScene(string sceneName)
    {
        SceneManager.sceneUnloaded += HomeSceneUnloaded;
        sceneToLoad = sceneName;
        SceneLoader.UnloadSceneAsync("Home");
    }

    private void HomeSceneUnloaded(Scene scene)
    {
        SceneManager.sceneUnloaded -= HomeSceneUnloaded;
        SceneLoader.LoadAdditiveSceneAsync(sceneToLoad);
    }
}
