using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning_English.Services
{
    public static class RandomIndex
    {
        static Random random = new Random();

        // Метод определяет индекс слова если выбрано условие "Слова вразброс"
        public static int GetRandomIndex(int max)
        {
            return random.Next(0, max);
        }
    }
}
