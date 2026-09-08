using System.Text.RegularExpressions;

namespace InnoClinic.Profiles.API.Application.Validators
{
    public static class ValidationConstants
    {
        public const int NameMaxLength = 50;
        public const int MinBirthYearsAgo = 120;
        public const int MinCareerStartYear = 1950;

        public static readonly Regex PhoneRegex = new(@"^\+?[0-9]{7,15}$", RegexOptions.Compiled);
    }
}
