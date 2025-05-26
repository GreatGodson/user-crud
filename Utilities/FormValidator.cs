using System.Text.RegularExpressions;

namespace MyFirstApi.Utilities
{
    public static partial class FormValidator
    {
        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
        private static partial Regex EmailRegex();

        [GeneratedRegex(@"^(?:\+234|0)?[789][01]\d{8}$")]
        private static partial Regex NigerianPhoneRegex();

        [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
        private static partial Regex StrongPasswordRegex();

        public static string? ValidateEmail(string? email)
        {
            return string.IsNullOrWhiteSpace(email) ? "Email is required." : !EmailRegex().IsMatch(email) ? "Invalid email format." : null;
        }

        public static string? ValidatePhoneNumber(string? phoneNumber)
        {
            return string.IsNullOrWhiteSpace(phoneNumber)
                ? "Phone number is required."
                : !NigerianPhoneRegex().IsMatch(phoneNumber) ? "Invalid Nigerian phone number format." : null;
        }

        public static string? ValidateStrongPassword(string? password)
        {
            return string.IsNullOrWhiteSpace(password)
                ? "Password is required."
                : !StrongPasswordRegex().IsMatch(password)
                ? "Password must be at least 8 characters, include upper and lowercase letters, a digit, and a special character."
                : null;
        }

        public static string? ValidateMinLength(string? input, int minLength)
        {
            return string.IsNullOrWhiteSpace(input)
                ? $"Input is required and must be at least {minLength} characters."
                : input.Length < minLength ? $"Input must be at least {minLength} characters long." : null;
        }

        public static string? ValidateMaxLength(string? input, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null; // empty input passes max length, use ValidateMinLength for required fields
            }

            return input.Length > maxLength ? $"Input must be at most {maxLength} characters long." : null;
        }

        public static string? ValidateEmpty(string? value, string errorMessage)
        {
            return string.IsNullOrWhiteSpace(value)
                ? errorMessage : null;
        }
    }
}