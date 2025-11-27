using System.Text.RegularExpressions;

namespace TiendaGod.Identity.Validators
{
    public static class PasswordValidator
    {
        private static readonly Regex PasswordRegex =
            new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");

        public static bool Validate(string password)
        {
            return PasswordRegex.IsMatch(password);
        }
    }
}
