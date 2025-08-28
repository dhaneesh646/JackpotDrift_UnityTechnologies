using UnityEngine;

public class UserDatas : MonoBehaviour
{
    private string userId;
    private string userName;
    private string totalScoreInSlotGame;
    private string totalScorenIDiceGame;

    public string UserId { get => userId; set => userId = value; }
    public string UserName { get => userName; set => userName = value; }
    public string TotalScoreInSlotGame { get => totalScoreInSlotGame; set => totalScoreInSlotGame = value; }
    public string TotalScorenIDiceGame { get => totalScorenIDiceGame; set => totalScorenIDiceGame = value; }
}
