using TMPro;
using UnityEngine;

public class HomeView : MonoBehaviour
{
    [SerializeField] TMP_Text username_tmp;
    [SerializeField] TMP_Text totalCoins_tmp;
    private int slotGamesore = 0;
    private int diceGamescore = 0;

    public void SetUserDetails(string username)
    {
        username_tmp.text = "Hi," + " " + username;
        
        if (!string.IsNullOrEmpty(AppManager.Instance.userDatas.TotalScoreInSlotGame))
        {
            slotGamesore = int.Parse(AppManager.Instance.userDatas.TotalScoreInSlotGame);
        }
        else
        {
            slotGamesore = 0;
        }

        if (!string.IsNullOrEmpty(AppManager.Instance.userDatas.TotalScorenIDiceGame))
        {
            diceGamescore = int.Parse(AppManager.Instance.userDatas.TotalScorenIDiceGame);
        }
        else
        {
            diceGamescore = 0;
        }

        totalCoins_tmp.text = (diceGamescore + slotGamesore).ToString();

    }
}