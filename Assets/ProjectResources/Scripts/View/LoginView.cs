using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Toggle rememberMeToggle;
    public GameObject loginPanel;
    public GameObject autoLoginPanel;


    public string GetEmail() => emailInput.text;
    public string GetPassword() => passwordInput.text;
    public bool IsRememberMeChecked() => rememberMeToggle.isOn;


    public void SetFeedback(string message, bool isError = true)
    {
        ToastPopup.ShowToast?.Invoke(message, 3, isError);
    }

    public void BindLogin(System.Action onLoginClicked)
    {
        loginButton.onClick.AddListener(() => onLoginClicked.Invoke());
    }

    public void ClearFields()
    {
        emailInput.text = "";
        passwordInput.text = "";
    }
}