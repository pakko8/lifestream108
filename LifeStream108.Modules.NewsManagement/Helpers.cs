using LiteDB;
using System.IO;
using System.Text;

namespace LifeStream108.Modules.NewsManagement
{
    internal static class Helpers
    {
        private const string LiteDbDirectory = @"C:\_Projects\LiteDb\NewsDbFiles";

        public static ConnectionString CreateReadolyLiteDbConnObj(string connString)
        {
            return new ConnectionString
            {
                Filename = connString,
                Connection = ConnectionType.Shared
            };
        }

        public static (string DbConnString, string TableName) GetLiteDbNewsHistoryConnString(int userId)
        {
            string fileName = $"NewsHistory{Number2LetterString(userId)}";
            return (Path.Combine(LiteDbDirectory, fileName), fileName);
        }

        private static string Number2LetterString(int number)
        {
            const char emptyChar = '_';
            string numberStr = number.ToString();
            StringBuilder sbLetters = new StringBuilder($"{emptyChar}{new string('0', 7 - numberStr.Length)}{numberStr}");
            for (int i = 0; i < sbLetters.Length; i++)
            {
                char ch = sbLetters[i];
                if (ch != emptyChar && char.IsDigit(ch))
                {
                    sbLetters[i] = Digit2Letter(ch);
                }
            }
            return sbLetters.ToString();
        }

        private static char Digit2Letter(char digit)
        {
            switch (digit)
            {
                case '0': return 'O';
                case '1': return 'A';
                case '2': return 'B';
                case '3': return 'C';
                case '4': return 'D';
                case '5': return 'E';
                case '6': return 'F';
                case '7': return 'G';
                case '8': return 'H';
                case '9': return 'I';
                default: return 'Z';
            }
        }
    }
}
