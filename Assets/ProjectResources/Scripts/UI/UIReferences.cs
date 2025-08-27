using UnityEngine;
using UnityEngine.UI;

public class UIReferences : MonoBehaviour
{
    [Header("Login UI")]
    public GameObject loginPanel;
    public InputField loginEmailInput;
    public InputField loginPasswordInput;
    public Button loginButton;
    public Button switchToSignUpButton;
    public Text loginFeedbackText;

    [Header("Sign-Up UI")]
    public GameObject signUpPanel;
    public InputField signUpEmailInput;
    public InputField signUpUsernameInput;
    public InputField signUpPasswordInput;
    public InputField signUpConfirmPasswordInput;
    public Button signUpButton;
    public Button switchToLoginButton;
    public Text signUpFeedbackText;
}