using System.Collections.Generic;
using UnityEngine;
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


        int reelCount = reels.Length;
        int rowCount = reels[0].visibleSymbolsNames.Length;
        payLines = GeneratePaylines(reelCount, rowCount);

        score = int.Parse(AppManager.Instance.userDatas.TotalScoreInSlotGame);
        coinText.text = score.ToString();
    }

    protected override void OnActionButtonPressed()
    {
        scoreToAdd = 0;
        AudioManager.Instance.PlayEffect(SoundEffect.SlotMachine);
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

