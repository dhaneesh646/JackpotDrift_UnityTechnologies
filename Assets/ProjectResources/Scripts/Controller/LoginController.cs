using System;
using System.Collections;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using OneXR.WorkFlow.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    public LoginView view;

    private LoginModel model;
    [SerializeField] LoadingScreen loadingScreen;
    public bool isLoginCompleted;
    public bool isUserDataRetrived;
    private DatabaseReference databaseReference;

    void Start()
    {
        model = new LoginModel();
        view.BindLogin(OnLoginClicked);

        if (model.LoadSavedCredentials(out string savedEmail, out string savedPassword))
        {
            // view.SetFeedback("Auto logging in...", false);
            view.autoLoginPanel.SetActive(true);
            SimulateLogin(savedEmail, savedPassword, true);
        }
        else
        {
            view.loginPanel.SetActive(true);
        }
    }

    void OnLoginClicked()
    {
        string email = view.GetEmail();
        string password = view.GetPassword();

        model.SetCredentials(email, password);

        if (!model.IsValid(out string validationMessage))
        {
            view.SetFeedback(validationMessage);
            return;
        }

        view.SetFeedback("Logging in...", false);

        SimulateLogin(email, password);
    }

    void SimulateLogin(string email, string password, bool autoLogin = false)
    {
        FirebaseAuth firebaseAuth = FirebaseAuth.DefaultInstance;
        Credential credential = EmailAuthProvider.GetCredential(email, password);
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        firebaseAuth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                view.SetFeedback("Invalid credentials", true);

                if (autoLogin)
                {
                    model.ClearSavedCredentials();
                    view.autoLoginPanel.SetActive(false);
                    view.loginPanel.SetActive(true);
                }
                return;
            }

            AuthResult authResult = task.Result;
            if (authResult.User.IsEmailVerified)
            {
                view.SetFeedback("Logged in successfully!", false);
                view.ClearFields();

                if (view.IsRememberMeChecked())
                {
                    model.SetCredentials(email, password);
                    model.SaveCredentials();

                }
                else
                {
                    model.ClearSavedCredentials();
                }
                view.autoLoginPanel.SetActive(true);
                StartCoroutine(GoToHomeScreen());
                StartCoroutine(SetUserDatas(authResult.User.UserId));
            }
            else
            {
                view.SetFeedback("Please verify your email", true);
            }
        });
    }

    private IEnumerator SetUserDatas(string userId)
    {
        AppManager.Instance.UserDatas.UserId = userId;
        var serverData = databaseReference.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(() => serverData.IsCompleted);
        userDetails dts = new userDetails();

        DataSnapshot snapshot = serverData. Result;
        string jsonData = snapshot.GetRawJsonValue();
        if (jsonData != null)
        {
            print("server data found" + jsonData);
            dts = JsonUtility.FromJson<userDetails>(jsonData);
            AppManager.Instance.UserDatas.UserName = dts.username;
            AppManager.Instance.UserDatas.TotalScoreInSlotGame = dts.slot_score;
            AppManager.Instance.userDatas.TotalScorenIDiceGame = dts.dice_score;
        }
        else
        {
            print("no data found");
        }
        isUserDataRetrived = true;
    }

    private IEnumerator GoToHomeScreen()
    {
        isLoginCompleted = true;
        yield return new WaitUntil(() =>

            loadingScreen.isloadingCompleted == true
        );
        SceneManager.sceneUnloaded += LoginSceneUnloaded;
        SceneLoader.UnloadSceneAsync("Login");
    }

    private void LoginSceneUnloaded(Scene scene)
    {
        SceneManager.sceneUnloaded -= LoginSceneUnloaded;
        SceneLoader.LoadAdditiveSceneAsync("Home");
    }

    [Serializable]
    public class userDetails
    {
        public string username;
        public string slot_score;
        public string dice_score;
    }
}
