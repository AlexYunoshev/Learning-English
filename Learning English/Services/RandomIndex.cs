using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning_English.Services
{
    public static class RandomIndex
    {
        static private Random random = new Random();

        static private List<int> IndexOfWords = new List<int>(); // коллекция использованных индексов слов

        static private int counter = 0;


        // Метод возвращает случайное число в пределах индексов слов
        public static int GetRandomValue(int max)
        {
            return random.Next(0, max);
        }

        // Метод возвращает индекс слова
        public static int GetIndex(int max)
        {
            int value = RandomIndex.GetRandomValue(max);

            if (IndexOfWords.Count == 0)
            {
                IndexOfWords.Add(value);
                counter++;
                return value;
            }
            else
            {
                while (counter < max)
                {
                    if (IndexOfWords.IndexOf(value) == -1)
                    {
                        IndexOfWords.Add(value);
                        counter++;
                        return value;
                    }
                    else
                    {
                        value = RandomIndex.GetRandomValue(max);
                    }
                }
                return -1;
            }

        }
    }
}
