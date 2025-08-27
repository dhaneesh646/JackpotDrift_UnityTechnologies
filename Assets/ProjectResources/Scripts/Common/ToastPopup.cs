using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

public class ToastPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject toastPanel;
    [SerializeField] Image toastImage;
    [SerializeField] private Animator anim;
    private Coroutine toastCloser;
    [SerializeField] private Color messageColor;
    [SerializeField] private Color warningColor;
    [SerializeField] GameObject alertPanel;
    [SerializeField] TMP_Text alertHeading;
    [SerializeField] TMP_Text alertMsg;

    public static Action<string, float, bool> ShowToast;
    public static Action<bool> Alertmessage;
    public static Action<string, string> showAlertPanel;

    private void Awake()
    {
        ShowToast += DisplayToast;
        showAlertPanel += DisplayAlert;
    }

    private void DisplayAlert(string heading, string msg)
    {
        alertHeading.text = heading;
        alertMsg.text = msg;
        alertPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        ShowToast -= DisplayToast;
        showAlertPanel -= DisplayAlert;
    }
    

    

    public void DisplayToast(string message, float toastTime = 3f, bool status = true)
    {
        if (status)
        {
            toastImage.color = warningColor;
        }
        else
        {
            toastImage.color = messageColor;
        }
        messageText.text = message;
        toastPanel.SetActive(true);

        if (toastCloser == null)
        {
            toastCloser = StartCoroutine(CloseToast(toastTime));
        }
        else
        {
            StopCoroutine(toastCloser);
            toastCloser = null;
            toastCloser = StartCoroutine(CloseToast(toastTime));
        }
    }

    private IEnumerator CloseToast(float toastTime)
    {
        yield return new WaitForSeconds(toastTime);
        anim.SetTrigger("Popout");
        toastCloser = null;
    }
}