using UnityEngine;

public class UserDatas : MonoBehaviour
{
    private string userId;
    private string userName;
    private string totalScore;

    public string UserId { get => userId; set => userId = value; }
    public string UserName { get => userName; set => userName = value; }
    public string TotalScore { get => totalScore; set => totalScore = value; }
}
