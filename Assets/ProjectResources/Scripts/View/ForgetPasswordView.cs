using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ForgotPasswordView : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField emailInput;
    public Button resetButton;

    public string GetEmail() => emailInput.text;
    public void SetEmail(string email) => emailInput.text = email;

    public void SetFeedback(string message, bool isError = true)
    {
         ToastPopup.ShowToast?.Invoke(message,3, isError);
        // feedbackText.text = message;
        // feedbackText.color = isError ? Color.red : Color.green;
    }

    public void BindReset(System.Action onResetClicked)
    {
        resetButton.onClick.AddListener(() => onResetClicked.Invoke());
    }

    public void ClearFields()
    {
        emailInput.text = "";
    }
}