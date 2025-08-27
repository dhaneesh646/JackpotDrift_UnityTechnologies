public class SignUpModel
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string ConfirmPassword { get; private set; }

    public void SetCredentials(string username, string email, string password, string confirmPassword)
    {
        Username = username.Trim();
        Email = email.Trim();
        Password = password;
        ConfirmPassword = confirmPassword;
    }

    public bool IsValid(out string errorMessage)
    {
        if (string.IsNullOrEmpty(Username))
        {
            errorMessage = "Username is required.";
            return false;
        }

        if (string.IsNullOrEmpty(Email) || !Email.Contains("@") || !Email.Contains("."))
        {
            errorMessage = "Invalid email format.";
            return false;
        }

        if (string.IsNullOrEmpty(Password) || Password.Length < 6)
        {
            errorMessage = "Password must be at least 6 characters.";
            return false;
        }

        if (Password != ConfirmPassword)
        {
            errorMessage = "Passwords do not match.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}