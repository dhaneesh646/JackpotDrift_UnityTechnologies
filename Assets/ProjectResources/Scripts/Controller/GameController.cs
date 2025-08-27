using System;
using System.Collections;
using Firebase.Database;
using Firebase.Extensions;
using OneXR.WorkFlow.Common;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Row[] reels;
    public int score = 0;
    [SerializeField] Button spinButton;
    [SerializeField] TMP_Text scoreTetx;
    [SerializeField] Button menuButton;
    [SerializeField] GameObject gameresultPanel;
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text winorloseText;
    private bool isRotateStarted = false;
    private int scoreToAdd;

    void Start()
    {
        spinButton.onClick.AddListener(Spin);
        scoreTetx.text = AppManager.Instance.userDatas.TotalScore;
        menuButton.onClick.AddListener(LoadHomeScene);
    }

    private void LoadHomeScene()
    {
        SceneManager.sceneUnloaded += GameSceneUnLoaded;
        SceneLoader.UnloadSceneAsync("Game");
    }

    private void GameSceneUnLoaded(Scene scene)
    {
        SceneManager.sceneUnloaded -= GameSceneUnLoaded;
        SceneLoader.LoadAdditiveSceneAsync("Home");
    }

    void Update()
    {
        if (isRotateStarted)
        {
            if (reels[0].isRotateStopped && reels[1].isRotateStopped && reels[2].isRotateStopped)
            {
                isRotateStarted = false;
                CheckForWin();
            }
        }
    }

    public void CheckForWin()
    {
        string[] reel1Symbols = reels[0].visibleSymbolsNames;
        string[] reel2Symbols = reels[1].visibleSymbolsNames;
        string[] reel3Symbols = reels[2].visibleSymbolsNames;

        CheckHorizontalLines(reel1Symbols, reel2Symbols, reel3Symbols);
        CheckDiagonalLines(reel1Symbols, reel2Symbols, reel3Symbols);
        CheckMixedWins(reel1Symbols, reel2Symbols, reel3Symbols);

        updateLatestScoreToServer();
    }

    private void updateLatestScoreToServer()
    {
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        dbRef.Child("users").Child(AppManager.Instance.userDatas.UserId).Child("score").SetValueAsync(scoreToAdd).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Username saved successfully!");
            }
            else
            {
                Debug.LogWarning("Failed to save username: " + task.Exception);
            }
        });
        score += scoreToAdd;

        scoreTetx.text = score.ToString();
        AppManager.Instance.userDatas.TotalScore = score.ToString();

        if (scoreToAdd > 0)
        {
            StartCoroutine(DisplayGameResult("You won " + scoreToAdd + " points!", true));
        }
        else
        {
            StartCoroutine(DisplayGameResult("No win this round.", false));
        }
    }

    private void CheckHorizontalLines(string[] r1, string[] r2, string[] r3)
    {
        // Top horizontal line
        if (r1[0] == r2[0] && r2[0] == r3[0])
        {
            AwardScore(r1[0], "Top Horizontal");
        }

        // Center horizontal line
        if (r1[1] == r2[1] && r2[1] == r3[1])
        {
            AwardScore(r1[1], "Center Horizontal");
        }

        // Bottom horizontal line
        if (r1[2] == r2[2] && r2[2] == r3[2])
        {
            AwardScore(r1[2], "Bottom Horizontal");
        }
    }

    private void CheckDiagonalLines(string[] r1, string[] r2, string[] r3)
    {
        if (r1[0] == r2[1] && r2[1] == r3[2])
        {
            AwardScore(r1[0], "Diagonal 1");
        }

        // Bottom-left to Top-right diagonal
        if (r1[2] == r2[1] && r2[1] == r3[0])
        {
            AwardScore(r1[2], "Diagonal 2");
        }
    }

    private void CheckMixedWins(string[] r1, string[] r2, string[] r3)
    {
        CheckSpecificMixedPattern(r1[0], r2[1], r3[2]);
        CheckSpecificMixedPattern(r1[2], r2[1], r3[0]);

        CheckSpecificMixedPattern(r1[0], r2[0], r3[1]);
        CheckSpecificMixedPattern(r1[0], r2[1], r3[0]);
        CheckSpecificMixedPattern(r1[0], r2[1], r3[1]);

        CheckSpecificMixedPattern(r1[1], r2[0], r3[0]);
        CheckSpecificMixedPattern(r1[1], r2[0], r3[1]);
        CheckSpecificMixedPattern(r1[1], r2[1], r3[0]);
        CheckSpecificMixedPattern(r1[1], r2[2], r3[1]);
        CheckSpecificMixedPattern(r1[1], r2[1], r3[2]);

        CheckSpecificMixedPattern(r1[2], r2[1], r3[1]);
        CheckSpecificMixedPattern(r1[2], r2[1], r3[2]);

    }

    private void CheckSpecificMixedPattern(string s1, string s2, string s3)
    {
        if (s1 == s2 && s2 == s3)
        {
            switch (s1)
            {
                case "Spade":
                    scoreToAdd += 5;
                    Debug.Log("Mixed Spade win! Awarded 5 points. Total score: " + score);
                    break;
                case "Heart":
                    scoreToAdd += 10;
                    Debug.Log("Mixed Heart win! Awarded 10 points. Total score: " + score);
                    break;
                case "Bar":
                    scoreToAdd += 15;
                    Debug.Log("Mixed Bar win! Awarded 15 points. Total score: " + score);
                    break;
                case "Bell":
                    scoreToAdd += 20;
                    Debug.Log("Mixed Bell win! Awarded 20 points. Total score: " + score);
                    break;
                case "Seven":
                    scoreToAdd += 50;
                    Debug.Log("Mixed Seven win! Awarded 50 points. Total score: " + score);
                    break;
            }
        }
    }

    private void AwardScore(string symbol, string winType)
    {
        int points = 0;
        switch (symbol)
        {
            case "Spade":
                points = 10;
                break;
            case "Heart":
                points = 20;
                break;
            case "Bar":
                points = 30;
                break;
            case "Bell":
                points = 40;
                break;
            case "Seven":
                points = 100;
                break;
            default:
                Debug.LogWarning("No score defined for symbol: " + symbol);
                return;
        }
        scoreToAdd += points;
        Debug.Log("Win! " + winType + " with " + symbol + " symbols. You won " + points + " points! Total score: " + score);
    }

    public void Spin()
    {
        scoreToAdd = 0;
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].StartRotate();
            isRotateStarted = true;
        }
    }
    
    IEnumerator DisplayGameResult(string scoreInfo ,bool isWin)
    {
        spinButton.interactable = false;
        yield return new WaitForSeconds(1.5f);
        spinButton.interactable = true;
        gameresultPanel.SetActive(true);
        resultText.text = scoreInfo;
        winorloseText.text = isWin ? "You Win!" : "You Lose!";
    }
}