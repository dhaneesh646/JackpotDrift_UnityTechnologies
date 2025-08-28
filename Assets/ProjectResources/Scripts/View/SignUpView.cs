using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] TMP_InputField confirmPasswordInput;
    [SerializeField] Toggle showPasswordToggle;
    [SerializeField] Toggle showConfirmPasswordToggle;
    [SerializeField] Button signUpButton;

    public string GetUsername() => usernameInput.text;
    public string GetEmail() => emailInput.text;
    public string GetPassword() => passwordInput.text;
    public string GetConfirmPassword() => confirmPasswordInput.text;

    public void SetFeedback(string message, bool isError = true)
    {
        ToastPopup.ShowToast?.Invoke(message,3, isError);
    }

    public void BindSignUp(System.Action onSignUpClicked)
    {
        signUpButton.onClick.AddListener(() => onSignUpClicked.Invoke());
        showPasswordToggle.onValueChanged.AddListener((isOn) =>
        {
            passwordInput.contentType = isOn ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
            passwordInput.ForceLabelUpdate();
        });
        showConfirmPasswordToggle.onValueChanged.AddListener((isOn) =>
        {
            confirmPasswordInput.contentType = isOn ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
            confirmPasswordInput.ForceLabelUpdate();
        });
    }

    public void ClearFields()
    {
        usernameInput.text = "";
        emailInput.text = "";
        passwordInput.text = "";
        confirmPasswordInput.text = "";
    }
}