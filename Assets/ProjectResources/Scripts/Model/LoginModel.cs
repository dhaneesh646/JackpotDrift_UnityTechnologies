using UnityEngine;

public class LoginModel
{
    public string Email { get; private set; }
    public string Password { get; private set; }

    public void SetCredentials(string email, string password)
    {
        Email = email.Trim();
        Password = password;
    }

    public bool IsValid(out string errorMessage)
    {
        if (string.IsNullOrEmpty(Email))
        {
            errorMessage = "Email is required.";
            return false;
        }

        if (!Email.Contains("@") || !Email.Contains("."))
        {
            errorMessage = "Invalid email format.";
            return false;
        }

        if (string.IsNullOrEmpty(Password))
        {
            errorMessage = "Password is required.";
            return false;
        }

        if (Password.Length < 6)
        {
            errorMessage = "Password must be at least 6 characters.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    public void SaveCredentials()
    {
        PlayerPrefs.SetString("saved_email", Email);
        PlayerPrefs.SetString("saved_password", Password);
        PlayerPrefs.SetInt("remember_me", 1);
        PlayerPrefs.Save();
    }

    public bool LoadSavedCredentials(out string savedEmail, out string savedPassword)
    {
        bool remember = PlayerPrefs.GetInt("remember_me", 0) == 1;
        savedEmail = PlayerPrefs.GetString("saved_email", "");
        savedPassword = PlayerPrefs.GetString("saved_password", "");
        return remember && !string.IsNullOrEmpty(savedEmail) && !string.IsNullOrEmpty(savedPassword);
    }

    public void ClearSavedCredentials()
    {
        PlayerPrefs.DeleteKey("saved_email");
        PlayerPrefs.DeleteKey("saved_password");
        PlayerPrefs.DeleteKey("remember_me");
        PlayerPrefs.Save();
    }
}