using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] Toggle showPasswordToggle;
    [SerializeField] Button loginButton;
    [SerializeField] Toggle rememberMeToggle;
    [SerializeField] public GameObject loginPanel;
    [SerializeField] public GameObject autoLoginPanel;


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
        showPasswordToggle.onValueChanged.AddListener((isOn) =>
        {
            passwordInput.contentType = isOn ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
            passwordInput.ForceLabelUpdate();
        });
    }

    public void ClearFields()
    {
        emailInput.text = "";
        passwordInput.text = "";
    }
}