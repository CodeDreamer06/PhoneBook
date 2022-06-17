namespace Phonebook;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine(Helpers.Message);
        UserInput.ShowMenu();
    }
}