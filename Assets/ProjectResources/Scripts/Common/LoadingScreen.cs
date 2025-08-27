using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Slider loadingSlider;

    private float loadingTime = 3.0f;
    private float elapsedTime = 0.0f;
    public bool isloadingCompleted;
    [SerializeField] LoginController loginController;
    [SerializeField] LoadingTextAnimator loadingTextAnimator;

    void Update()
    {
        if (elapsedTime < loadingTime)
        {
            elapsedTime += Time.deltaTime;
            loadingSlider.value = Mathf.Clamp01(elapsedTime / loadingTime);
        }
        if (loginController.isLoginCompleted)
        {
            loadingTextAnimator.baseText = "Fetching User Data";
        }
        if (loginController.isUserDataRetrived)
        {
            if (loadingSlider.value == 1)
            {
                isloadingCompleted = true;
            }
        }

    }

    void ODisable()
    {
        loadingSlider.value = 0;
        isloadingCompleted = false;
    }
}