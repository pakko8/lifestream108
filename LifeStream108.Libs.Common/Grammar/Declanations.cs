namespace LifeStream108.Libs.Common.Grammar
{
    public static class Declanations
    {
        /// <summary>
        /// Склонение существительных в зависимости от числительного. Например, 1 день, 2 дня, 15 дней и т.д.
        /// </summary>
        /// <param name="word1">несклоненное слово, например, "день"</param>
        /// <param name="word2">склоненное слово в единственном числе, например, "дня"</param>
        /// <param name="word3">склоненное слово во множественном числе, например, "дней"</param>
        /// <param name="num">число</param>
        public static string DeclineByNumeral(int num, string word1, string word2, string word3)
        {
            int numTmp = num % 100;
            if (numTmp >= 11 && numTmp <= 19)
            {
                return word3;
            }

            numTmp = numTmp % 10;
            switch (numTmp)
            {
                case 1:
                    return word1;
                case 2:
                case 3:
                case 4:
                    return word2;
                default:
                    return word3;
            }
        }
    }
}
