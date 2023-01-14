using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excalibur.DataStructures
{
    public static class Helpers
    {
        public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
        {
            if (list.Count < 2 || firstIndex == secondIndex)
            {
                return;
            }

            var temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }

        public static void Swap<T>(this ArrayList<T> list, int firstIndex, int secondIndex)
        {
            if (list.Count < 2 || firstIndex == secondIndex)
            {
                return;
            }

            var temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }

        public static string PadCenter(this string text, int newWidth, char filterCharacter = ' ')
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            int length = text.Length;
            int charactersToPad = newWidth - length;
            if (charactersToPad < 0)
            {
                throw new ArgumentException("New width must be greater than string length.", "newWidth");
            }
            int padLeft = charactersToPad / 2 + charactersToPad % 2;
            int padRight = charactersToPad / 2;
            return new String(filterCharacter, padLeft) + text + new String(filterCharacter, padRight);
        }

        /// <summary>
        /// 填充数组
        /// </summary>
        public static void Populate<T>(this T[,] array, int rows, int columns, T defaultValue = default(T))
        {
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    array[i, j] = defaultValue;
                }
            }
        }
    }
}
