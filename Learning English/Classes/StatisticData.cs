using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning_English.Classes
{
    public static class StatisticData
    {
        private static int allWordsCount = 0;
        private static int correctWordsCount = 0;

        public static int AllWordsCount
        {
            get { return allWordsCount; }
            set
            {
                if (allWordsCount < 0)
                    return;
                allWordsCount = value;
            }
        }

        public static int CorrectWordsCount
        {
            get { return correctWordsCount; }
            set
            {
                if (correctWordsCount < 0)
                    return;
                correctWordsCount = value;
            }
        }
    }
}
