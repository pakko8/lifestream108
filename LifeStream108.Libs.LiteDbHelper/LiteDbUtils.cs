using LiteDB;
using System.Text;

namespace LifeStream108.Libs.LiteDbHelper
{
    public static class LiteDbUtils
    {
        public static ConnectionString CreateReadolyConnection(string connString)
        {
            return new ConnectionString
            {
                Filename = connString,
                Connection = ConnectionType.Shared
            };
        }

        public static string PrepateDbFileName(int number)
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
