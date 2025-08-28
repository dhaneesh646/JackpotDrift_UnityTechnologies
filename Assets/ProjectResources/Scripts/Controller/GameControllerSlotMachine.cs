// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Firebase.Database;
// using Firebase.Extensions;
// using OneXR.WorkFlow.Common;
// using TMPro;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// public class GameControllerSlotMachine : MonoBehaviour
// {
//     [SerializeField] Row[] reels;
//     [SerializeField] int score = 0;

//     [SerializeField] Button spinButton;
//     [SerializeField] TMP_Text scoreTetx;
//     [SerializeField] Button menuButton;
//     [SerializeField] GameObject gameresultPanel;
//     [SerializeField] TMP_Text resultText;
//     [SerializeField] TMP_Text winorloseText;

//     private bool isRotateStarted = false;
//     private int scoreToAdd;

//     private List<(int reel, int row)[]> payLines;

//     private readonly Dictionary<string, int> symbolScores = new Dictionary<string, int>
//     {
//         { "Spade", 10 },
//         { "Heart", 20 },
//         { "Bar", 30 },
//         { "Bell", 40 },
//         { "Seven", 100 }
//     };

//     void Start()
//     {
//         spinButton.onClick.AddListener(Spin);
//         scoreTetx.text = AppManager.Instance.userDatas.TotalScore;
//         menuButton.onClick.AddListener(LoadHomeScene);

//         int reelCount = reels.Length;
//         int rowCount = reels[0].visibleSymbolsNames.Length;
//         payLines = GeneratePaylines(reelCount, rowCount);
//     }

//     private void LoadHomeScene()
//     {
//         SceneManager.sceneUnloaded += GameSceneUnLoaded;
//         SceneLoader.UnloadSceneAsync("Game");
//     }

//     private void GameSceneUnLoaded(Scene scene)
//     {
//         SceneManager.sceneUnloaded -= GameSceneUnLoaded;
//         SceneLoader.LoadAdditiveSceneAsync("Home");
//     }

//     void Update()
//     {
//         if (isRotateStarted)
//         {
//             bool allStopped = true;
//             foreach (var reel in reels)
//             {
//                 if (!reel.isRotateStopped) { allStopped = false; break; }
//             }

//             if (allStopped)
//             {
//                 isRotateStarted = false;
//                 CheckForWinDynamic();
//             }
//         }
//     }


//     private List<(int reel, int row)[]> GeneratePaylines(int reelCount, int rowCount)
//     {
//         var lines = new List<(int, int)[]>();

//         // Horizontals
//         for (int r = 0; r < rowCount; r++)
//         {
//             var line = new (int, int)[reelCount];
//             for (int c = 0; c < reelCount; c++)
//                 line[c] = (c, r);
//             lines.Add(line);
//         }

//         // Verticals
//         for (int c = 0; c < reelCount; c++)
//         {
//             var line = new (int, int)[rowCount];
//             for (int r = 0; r < rowCount; r++)
//                 line[r] = (c, r);
//             lines.Add(line);
//         }

//         // Main Diagonal
//         if (reelCount == rowCount) 
//         {
//             var diag1 = new (int, int)[reelCount];
//             for (int i = 0; i < reelCount; i++)
//                 diag1[i] = (i, i);
//             lines.Add(diag1);

//             // Anti Diagonal
//             var diag2 = new (int, int)[reelCount];
//             for (int i = 0; i < reelCount; i++)
//                 diag2[i] = (i, rowCount - 1 - i);
//             lines.Add(diag2);
//         }

//         return lines;
//     }

//     private void CheckForWinDynamic()
//     {
//         foreach (var line in payLines)
//         {
//             string firstSymbol = reels[line[0].reel].visibleSymbolsNames[line[0].row];
//             bool allMatch = true;

//             foreach (var (reelIndex, rowIndex) in line)
//             {
//                 string symbol = reels[reelIndex].visibleSymbolsNames[rowIndex];
//                 if (symbol != firstSymbol)
//                 {
//                     allMatch = false;
//                     break;
//                 }
//             }

//             if (allMatch)
//             {
//                 AwardScore(firstSymbol, "Line Win");
//             }
//         }

//         updateLatestScoreToServer();
//     }

//     private void updateLatestScoreToServer()
//     {
//         DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;
//         dbRef.Child("users").Child(AppManager.Instance.userDatas.UserId).Child("777_score")
//             .SetValueAsync(scoreToAdd).ContinueWithOnMainThread(task =>
//         {
//             if (task.IsCompleted)
//                 Debug.Log("Score saved successfully!");
//             else
//                 Debug.LogWarning("Failed to save score: " + task.Exception);
//         });

//         score += scoreToAdd;
//         scoreTetx.text = score.ToString();
//         AppManager.Instance.userDatas.TotalScore = score.ToString();

//         if (scoreToAdd > 0)
//             StartCoroutine(DisplayGameResult("You won " + scoreToAdd + " points!", true));
//         else
//             StartCoroutine(DisplayGameResult("No win this round.", false));
//     }

//     private void AwardScore(string symbol, string winType)
//     {
//         if (symbolScores.TryGetValue(symbol, out int points))
//         {
//             scoreToAdd += points;
//             Debug.Log($"Win! {winType} with {symbol}. You won {points} points! Total score: {score}");
//         }
//         else
//         {
//             Debug.LogWarning("No score defined for symbol: " + symbol);
//         }
//     }

//     public void Spin()
//     {
//         scoreToAdd = 0;
//         foreach (var reel in reels)
//             reel.StartRotate();
//         isRotateStarted = true;
//     }

//     IEnumerator DisplayGameResult(string scoreInfo, bool isWin)
//     {
//         spinButton.interactable = false;
//         yield return new WaitForSeconds(1.5f);
//         spinButton.interactable = true;
//         gameresultPanel.SetActive(true);
//         resultText.text = scoreInfo;
//         winorloseText.text = isWin ? "You Win!" : "You Lose!";
//     }
// }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using OneXR.WorkFlow.Common;

public class GameControllerSlotMachine : BaseGameController
{
    [SerializeField] private Row[] reels;

    private bool isRotateStarted = false;
    private List<(int reel, int row)[]> payLines;

    private readonly Dictionary<string, int> symbolScores = new Dictionary<string, int>
    {
        { "Spade", 10 },
        { "Heart", 20 },
        { "Bar", 30 },
        { "Bell", 40 },
        { "Seven", 100 }
    };

    protected override void Start()
    {
        base.Start();
        if (menuButton != null)
            menuButton.onClick.AddListener(LoadHomeScene);

        int reelCount = reels.Length;
        int rowCount = reels[0].visibleSymbolsNames.Length;
        payLines = GeneratePaylines(reelCount, rowCount);

        if (!string.IsNullOrEmpty(AppManager.Instance.userDatas.TotalScoreInSlotGame))
        {
            score = int.Parse(AppManager.Instance.userDatas.TotalScoreInSlotGame);
        }
    }

    protected override void OnActionButtonPressed()
    {
        scoreToAdd = 0;
        foreach (var reel in reels)
            reel.StartRotate();
        isRotateStarted = true;
    }

    void Update()
    {
        if (isRotateStarted)
        {
            bool allStopped = true;
            foreach (var reel in reels)
            {
                if (!reel.isRotateStopped) { allStopped = false; break; }
            }

            if (allStopped)
            {
                isRotateStarted = false;
                EvaluateResult();
            }
        }
    }

    protected override void EvaluateResult()
    {
        foreach (var line in payLines)
        {
            string firstSymbol = reels[line[0].reel].visibleSymbolsNames[line[0].row];
            bool allMatch = true;

            foreach (var (reelIndex, rowIndex) in line)
            {
                string symbol = reels[reelIndex].visibleSymbolsNames[rowIndex];
                if (symbol != firstSymbol)
                {
                    allMatch = false;
                    break;
                }
            }

            if (allMatch)
            {
                AwardScore(firstSymbol, "Line Win");
            }
        }

        UpdateLatestScoreToServer("slot_score");
    }

    protected override void UpdateLatestScoreToServer(string gameKey)
    {
        base.UpdateLatestScoreToServer(gameKey);
        AppManager.Instance.userDatas.TotalScoreInSlotGame = score.ToString();

        if (scoreToAdd > 0)
        {
            UpdateWinStatus("You won " + scoreToAdd + " points!", true);
        }
        else
        {
            UpdateWinStatus("No win this round.", false);
        }
    }

    private void AwardScore(string symbol, string winType)
    {
        if (symbolScores.TryGetValue(symbol, out int points))
        {
            scoreToAdd += points;
            Debug.Log($"Win! {winType} with {symbol}. You won {points} points!");
        }
        else
        {
            Debug.LogWarning("No score defined for symbol: " + symbol);
        }
    }

    private List<(int reel, int row)[]> GeneratePaylines(int reelCount, int rowCount)
    {
        var lines = new List<(int, int)[]>();

        // Horizontals
        for (int r = 0; r < rowCount; r++)
        {
            var line = new (int, int)[reelCount];
            for (int c = 0; c < reelCount; c++)
                line[c] = (c, r);
            lines.Add(line);
        }

        // Verticals
        for (int c = 0; c < reelCount; c++)
        {
            var line = new (int, int)[rowCount];
            for (int r = 0; r < rowCount; r++)
                line[r] = (c, r);
            lines.Add(line);
        }

        // Main Diagonal
        if (reelCount == rowCount)
        {
            var diag1 = new (int, int)[reelCount];
            for (int i = 0; i < reelCount; i++)
                diag1[i] = (i, i);
            lines.Add(diag1);

            var diag2 = new (int, int)[reelCount];
            for (int i = 0; i < reelCount; i++)
                diag2[i] = (i, rowCount - 1 - i);
            lines.Add(diag2);
        }

        return lines;
    }

    protected override void LoadHomeScene()
    {
        SceneManager.sceneUnloaded += GameSceneUnLoaded;
        SceneLoader.UnloadSceneAsync("SlotMachine_Game");
    }
}

