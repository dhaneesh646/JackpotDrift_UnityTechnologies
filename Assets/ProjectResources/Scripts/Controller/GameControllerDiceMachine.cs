using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using OneXR.WorkFlow.Common;

public class GameControllerDiceMachine : BaseGameController
{
    [System.Serializable]
    public class GameRules
    {
        public int[] winNumbers = { 7, 11 };
        public int[] loseNumbers = { 2, 3, 12 };
    }


    [Header("Game UI Elements")]
    [SerializeField] private TMP_Text rollCountText;
    [SerializeField] private Button playerRollResetButton;


    [Header("Game Configuration")]
    [SerializeField] private GameRules gameRules;


    [Header("Dice Elements")]
    [SerializeField] private SpriteRenderer die1Image;
    [SerializeField] private SpriteRenderer die2Image;
    [SerializeField] private Sprite[] diceSprites;


    [Header("Animation Settings")]
    [SerializeField] private float rollDuration = 1f;
    [SerializeField] private int framesPerSecond = 10;


    private int die1Value;
    private int die2Value;
    private bool isRolling = false;
    private bool gameActive = true;
    private int currentSum;
    private bool isGameWin;
    private bool isReroll;
    private int playerRolls = 1;

    protected override void Start()
    {
        base.Start();
        score = int.Parse(AppManager.Instance.userDatas.TotalScorenIDiceGame);
        coinText.text = score.ToString();

        playerRollResetButton.onClick.AddListener(() =>
        {
            playerRolls = 1;
            ResetGame();
        });
        ResetGame();
    }

    protected override void OnActionButtonPressed()
    {
        if (!gameActive || isRolling || playerRolls <= 0) return;
        StartCoroutine(AnimateRoll());
    }

    private IEnumerator AnimateRoll()
    {
        isRolling = true;
        isGameWin = false;
        isReroll = false;
        float elapsedTime = 0f;
        AudioManager.Instance.PlayEffect(SoundEffect.DiceRoll);
        while (elapsedTime < rollDuration)
        {
            int random1 = Random.Range(0, 6);
            int random2 = Random.Range(0, 6);

            die1Image.sprite = diceSprites[random1];
            die2Image.sprite = diceSprites[random2];

            elapsedTime += 1f / framesPerSecond;
            yield return new WaitForSeconds(1f / framesPerSecond);
        }

        die1Value = Random.Range(1, 7);
        die2Value = Random.Range(1, 7);

        die1Image.sprite = diceSprites[die1Value - 1];
        die2Image.sprite = diceSprites[die2Value - 1];

        int sum = die1Value + die2Value;
        resultText.text = $"Total: {sum}";
        currentSum = sum;
        EvaluateResult();
        isRolling = false;
    }

    protected override void EvaluateResult()
    {
        scoreToAdd = 0;

        if (System.Array.Exists(gameRules.winNumbers, element => element == currentSum))
        {
            isGameWin = true;
            isReroll = false;

            scoreToAdd = 50;
            gameActive = false;
            playerRolls += 1;
            rollCountText.text = $"Rolls Left: {playerRolls}";
            UpdateLatestScoreToServer("dice_score");
        }
        else if (System.Array.Exists(gameRules.loseNumbers, element => element == currentSum))
        {
            isGameWin = false;
            isReroll = false;

            gameActive = false;
            playerRolls = Mathf.Max(0, playerRolls - 1);
            rollCountText.text = $"Rolls Left: {playerRolls}";
            UpdateLatestScoreToServer("dice_score");
        }
        else
        {
            isGameWin = false;
            isReroll = true;

            gameActive = false;
            rollCountText.text = $"Rolls Left: {playerRolls}";
            UpdateLatestScoreToServer("dice_score");
        }
    }

    protected override void UpdateLatestScoreToServer(string gameKey)
    {
        base.UpdateLatestScoreToServer(gameKey);
        AppManager.Instance.userDatas.TotalScorenIDiceGame = score.ToString();
        if (isGameWin)
        {
            UpdateWinStatus("You won " + scoreToAdd + " points! Dice total : " + currentSum, true);
        }
        else if (!isGameWin && !isReroll)
        {
            UpdateWinStatus("You loose this round. Dice total : " + currentSum, false);
        }
        else
        {
            UpdateWinStatus("You loose, Reroll. Dice total : " + currentSum, false);
        }

    }

    protected override void ResetGame()
    {
        die1Value = 1;
        die2Value = 1;
        die1Image.sprite = diceSprites[0];
        die2Image.sprite = diceSprites[0];
        gameActive = true;
        if (playerRolls <= 0)
        {
            actionButton.interactable = false;
        }
        else
        {
            actionButton.interactable = true;
        }
        rollCountText.text = $"Rolls Left: {playerRolls}";
    }



    protected override void LoadHomeScene()
    {
        SceneManager.sceneUnloaded += GameSceneUnLoaded;
        SceneLoader.UnloadSceneAsync("Dice_Game");
    }
}
