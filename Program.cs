namespace PhoneBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Helpers.message);
            SqlAccess.Read();
        }
    }
}