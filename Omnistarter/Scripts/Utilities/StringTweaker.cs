// author: Omnistudio
// version: 2025.02.16

using UnityEngine;

namespace Omnis
{
    public class StringTweaker
    {
        public static readonly string[] hanNumbers = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        public static readonly string[] hanNumbersFormal = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
        private static readonly string[] unit = { "", "十", "百", "千", "万", "十", "百", "千", "亿" };
        private static readonly string[] unitFormal = { "", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿" };

        public static string NumberToHanCardinal(string numStr, bool useFormal = false)
        {
            // Not a number.
            if (!float.TryParse(numStr, out float num)) return "NAN";

            var splitted = num.ToString("F99").TrimEnd('0').Split('.');
            string integerPart = splitted[0];

            var numbers = useFormal ? hanNumbersFormal : hanNumbers;
            var units = useFormal ? unitFormal : unit;
            string integerStr = "";

            if (integerPart == "0")
            {
                integerStr = numbers[0];
            }
            else
            {
                int length = integerPart.Length;
                int midZero = 0;

                for (int i = 0; i < length; i++)
                {
                    int digit = integerPart[i] - '0';
                    int unitIndex = (length - 2 - i) % 8 + 1;

                    if (digit == 0)
                    {
                        midZero++;
                        if (unitIndex % 8 == 0)
                            integerStr += units[unitIndex];
                        else if (midZero < 4 && unitIndex % 4 == 0)
                            integerStr += units[unitIndex];
                    }
                    else
                    {
                        if (midZero > 0)
                        {
                            integerStr += numbers[0];
                            midZero = 0;
                        }
                        integerStr += numbers[digit] + units[unitIndex];
                    }
                }
            }

            if (!useFormal)
            {
                if (integerStr.StartsWith("一十"))
                    integerStr = integerStr[1..];
            }

            string decimalStr = NumeralToHanIndividual(splitted[1], useFormal);

            return decimalStr == "" ? integerStr : integerStr + "点" + decimalStr;
        }

        public static string NumeralToHanIndividual(string no, bool useFormal = false)
        {
            string result = "";
            var numbers = useFormal ? hanNumbersFormal : hanNumbers;

            foreach (char digit in no)
            {
                if (digit >= '0' && digit <= '9')
                    result += numbers[digit - '0'];
                else
                    result += digit;
            }

            return result;
        }
    }
}
