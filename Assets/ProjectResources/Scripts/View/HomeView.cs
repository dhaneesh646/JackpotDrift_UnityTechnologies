using TMPro;
using UnityEngine;

public class HomeView : MonoBehaviour
{
    [SerializeField] TMP_Text username_tmp;
    [SerializeField] TMP_Text totalCoins_tmp;

    public void SetUserDetails(string username)
    {
        username_tmp.text = "Hi," + " " + username;
    }
}
