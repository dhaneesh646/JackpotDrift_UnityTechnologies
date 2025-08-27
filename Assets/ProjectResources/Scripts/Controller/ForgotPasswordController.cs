using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

public class ForgotPasswordController : MonoBehaviour
{
    public ForgotPasswordView view;
    private ForgotPasswordModel model;

    void Start()
    {
        model = new ForgotPasswordModel();
        view.BindReset(OnResetClicked);
    }

    void OnResetClicked()
    {
        string email = view.GetEmail();
        model.SetEmail(email);

        if (!model.IsValid(out string validationMessage))
        {
            view.SetFeedback(validationMessage);
            return;
        }

        view.SetFeedback("Sending reset email...", false);
        SendPasswordReset(email);
    }

    void SendPasswordReset(string email)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Password reset canceled.");
                view.SetFeedback("Request canceled. Try again.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Password reset failed: " + task.Exception);
                view.SetFeedback("Failed to send reset email. Check the address.");
                return;
            }

            ToastPopup.showAlertPanel?.Invoke("Alert", "Password reset email sent! Check your inbox.");
            view.ClearFields();

            // Optional: Show popup
            // popup.ShowMessage("We've sent a password reset email to your inbox.");
        });
    }
}