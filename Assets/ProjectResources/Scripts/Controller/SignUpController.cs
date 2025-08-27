using System;
using System.Collections;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class SignUpController : MonoBehaviour
{
    public SignUpView view;
    private SignUpModel model;

    void Start()
    {
        model = new SignUpModel();
        view.BindSignUp(OnSignUpClicked);
    }

    void OnSignUpClicked()
    {
        string username = view.GetUsername();
        string email = view.GetEmail();
        string password = view.GetPassword();
        string confirmPassword = view.GetConfirmPassword();

        model.SetCredentials(username, email, password, confirmPassword);

        if (!model.IsValid(out string validationMessage))
        {
            view.SetFeedback(validationMessage);
            return;
        }

        view.SetFeedback("Creating account...", false);

        SimulateSignUp(username, email, password);
    }

    void SimulateSignUp(string username, string email, string password)
    {
        FirebaseAuth firebaseAuth = FirebaseAuth.DefaultInstance;
        string m_email = email;
        string m_password = password;
        firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync are canceleld");
                return;
            }
            else if (task.IsFaulted)
            {
                view.SetFeedback("Email aldready exist", true);
                return;
            }

            AuthResult authResult = task.Result;
            view.ClearFields();
            if (authResult.User.IsEmailVerified)
            {
                view.SetFeedback("Account created successfully!", false);
            }
            else
            {
                SendEmailVerification();
                SaveUsernameToDatabase(authResult.User.UserId, username);
            }
        });
    }

    void SaveUsernameToDatabase(string userId, string username)
    {
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        dbRef.Child("users").Child(userId).Child("username").SetValueAsync(username).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Username saved successfully!");
            }
            else
            {
                Debug.LogWarning("Failed to save username: " + task.Exception);
            }
        });
    }

    private void SendEmailVerification()
    {
        StartCoroutine(SendEmailVerificationAsync());
    }

    private IEnumerator SendEmailVerificationAsync()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            var sendEmailTask = user.SendEmailVerificationAsync();
            yield return new WaitUntil(() => sendEmailTask.IsCompleted);
            {
                if (sendEmailTask.Exception != null)
                {
                    Debug.Log("Email sent error");
                    FirebaseException firebaseException = sendEmailTask.Exception.GetBaseException() as FirebaseException;
                    AuthError error = (AuthError)firebaseException.ErrorCode;
                }
                else
                {
                    ToastPopup.showAlertPanel?.Invoke("Alert", "Verification mail sented");
                    Debug.Log("Email sent Suscess Fully");
                }
            }
        }
    }
}