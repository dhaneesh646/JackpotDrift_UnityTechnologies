using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;
using System;
using OneXR.WorkFlow.Common;
using UnityEngine.SceneManagement;

public abstract class BaseGameController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] protected Button actionButton;
    [SerializeField] protected GameObject resultPanel;
    [SerializeField] protected TMP_Text resultText;
    [SerializeField] protected TMP_Text winLoseText;

    
    [Header("Menu and Game score panel Buttons")]
    [SerializeField] protected Button menuButton;
    [SerializeField] protected Button gameScorePanelContnueButton;


    protected int score = 0;
    protected int scoreToAdd = 0;
    private Coroutine winCoroutine;


    protected virtual void Start()
    {
        if (actionButton != null)
            actionButton.onClick.AddListener(OnActionButtonPressed);
        if (menuButton != null)
            menuButton.onClick.AddListener(LoadHomeScene);
        if (gameScorePanelContnueButton != null)
            gameScorePanelContnueButton.onClick.AddListener(ResetGame);
    }

    protected virtual void ResetGame(){}
    protected abstract void OnActionButtonPressed();
    protected abstract void EvaluateResult();

    protected virtual void UpdateLatestScoreToServer(string gameKey)
    {
        score += scoreToAdd;

        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        dbRef.Child("users").Child(AppManager.Instance.userDatas.UserId).Child(gameKey).SetValueAsync(score).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Score saved successfully!");
            }
            else
            {
                Debug.LogWarning("Failed to save score: " + task.Exception);
            }
        });
    }

    protected void UpdateWinStatus(string scoreInfo, bool isWin)
    {
        if (winCoroutine != null)
        {
            StopCoroutine(winCoroutine);
        }
        winCoroutine = StartCoroutine(DisplayGameResult(scoreInfo, isWin));
    }

    private IEnumerator DisplayGameResult(string scoreInfo, bool isWin)
    {
        if (actionButton != null) actionButton.interactable = false;
        yield return new WaitForSeconds(1.5f);
        if (actionButton != null) actionButton.interactable = true;

        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultText != null) resultText.text = scoreInfo;
        if (winLoseText != null) winLoseText.text = isWin ? "You Win!" : "You Lose!";
    }

    protected abstract void LoadHomeScene();

    protected void GameSceneUnLoaded(Scene scene)
    {
        SceneManager.sceneUnloaded -= GameSceneUnLoaded;
        SceneLoader.LoadAdditiveSceneAsync("Home");
    }
}
