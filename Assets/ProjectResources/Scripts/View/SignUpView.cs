using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpView : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public Button signUpButton;

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
    }

    public void ClearFields()
    {
        usernameInput.text = "";
        emailInput.text = "";
        passwordInput.text = "";
        confirmPasswordInput.text = "";
    }
}