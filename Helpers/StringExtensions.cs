namespace InformationSystemOfASchoolIducationalPortal.Helpers
{
    public static class StringExtensions
    {
        public static string ToShortName(this string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return "";

            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var lastName = parts[0][0];
            var firstName = parts[1];

            return firstName + " " + lastName + '.';
        }
        public static string OnlyName(this string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return "";

            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var firstName = parts[1];

            return firstName;
        }
    }
}

