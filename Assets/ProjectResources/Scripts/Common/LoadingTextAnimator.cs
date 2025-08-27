using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingTextAnimator : MonoBehaviour
{
    public TMP_Text loadingText;
    public float interval = 0.5f;

    public string baseText = "Logging you in";
    private int dotCount = 0;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            dotCount = (dotCount + 1) % 4; // cycles from 0 to 3
            loadingText.text = baseText + new string('.', dotCount);
            timer = 0f;
        }
    }
}