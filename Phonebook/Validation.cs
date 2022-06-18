using System.Globalization;
using System.Text.RegularExpressions;

namespace Phonebook;

internal static class Validation
{
    private const string EmailRegex =
            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

    public static bool IsContactValid(string? name, string? phoneNumber, string? email)
    {
        if (name is null || phoneNumber is null) return false;

        if (name.Length > 50)
        {
            Console.WriteLine("Names cannot be longer than 50 characters");
            return false;
        }

        if (phoneNumber.Length > 10)
        {
            Console.WriteLine("Phone numbers cannot be longer than 10 digits");
            return false;
        }

        if (email?.Length > 320)
        {
            Console.WriteLine("An email address cannot be longer than 320 characters.");
            return false;
        }

        return true;
    }

    public static (bool, long) IsNumber(object testObject)
    {
        bool isNum = long.TryParse(
            Convert.ToString(testObject),
            NumberStyles.Any,
            NumberFormatInfo.InvariantInfo,
            out long number);

        return (isNum, number);
    }

    public static bool IsEmailValid(string email) => Regex.IsMatch(email, EmailRegex, RegexOptions.IgnoreCase);
}