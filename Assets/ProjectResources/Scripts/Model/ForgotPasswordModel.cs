public class ForgotPasswordModel
{
    public string Email { get; private set; }

    public void SetEmail(string email)
    {
        Email = email.Trim();
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

        errorMessage = string.Empty;
        return true;
    }
}