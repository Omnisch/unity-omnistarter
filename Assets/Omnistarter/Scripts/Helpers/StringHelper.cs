// author: Omnistudio
// version: 2026.02.25

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Omnis.Utils
{
    /// <summary>
    /// Auxiliary methods of System.String.
    /// </summary>
    public static class StringHelper
    {
        #region Han Characters
        public static readonly string[] hanNumbers = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        public static readonly string[] hanNumbersFormal = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
        private static readonly string[] unit = { "", "十", "百", "千", "万", "十", "百", "千", "亿" };
        private static readonly string[] unitFormal = { "", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿" };

        public static string ParseToHanCardinal(this string numStr, bool useFormal = false)
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

            string decimalStr = splitted[1].ParseToHanIndividual(useFormal);

            return decimalStr == "" ? integerStr : integerStr + "点" + decimalStr;
        }
        public static string ParseToHanIndividual(this string numStr, bool useFormal = false)
        {
            string result = "";
            var numbers = useFormal ? hanNumbersFormal : hanNumbers;

            foreach (char digit in numStr)
            {
                if (digit >= '0' && digit <= '9')
                    result += numbers[digit - '0'];
                else
                    result += digit;
            }

            return result;
        }
        #endregion

        #region Natural Order
        /// <summary>
        /// Used for System.Collections.Generic.IEnumerable&lt;string&gt;.OrderBy().
        /// </summary>
        public static string NaturalOrder(string s) 
            => System.Text.RegularExpressions.Regex.Replace(s, "\\d+", m => int.Parse(m.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("D10"));
        #endregion

        #region Args Parser
        /// <summary>
        /// Parse a text into arguments.<br/>
        /// Whitespace separates args; consecutive whitespace is treated as one separator.<br/>
        /// Supports quotes: "..." or '...'<br/>
        /// Supports backslash escaping: \", \', \\ and optionally \n, \t, \r
        /// </summary>
        public static string[] ParseArgs(string line, bool supportEscapes = true, bool supportSpecialEscapes = true) {
            if (line == null) return Array.Empty<string>();

            var args = new List<string>();
            var sb = new StringBuilder();

            bool inQuotes = false;
            char quoteChar = '\0';
            bool escaping = false;

            void FlushToken() {
                if (sb.Length > 0) {
                    args.Add(sb.ToString());
                    sb.Clear();
                }
            }

            for (int i = 0; i < line.Length; i++) {
                char c = line[i];

                if (escaping) {
                    escaping = false;

                    if (!supportEscapes) {
                        sb.Append('\\');
                        sb.Append(c);
                        continue;
                    }

                    if (supportSpecialEscapes) {
                        // Basic special escapes
                        sb.Append(c switch {
                            'n' => '\n',
                            't' => '\t',
                            'r' => '\r',
                            _ => c
                        });
                    } else {
                        sb.Append(c);
                    }
                    continue;
                }

                if (supportEscapes && c == '\\') {
                    escaping = true;
                    continue;
                }

                if (inQuotes) {
                    if (c == quoteChar) {
                        inQuotes = false;
                        quoteChar = '\0';
                    } else {
                        sb.Append(c);
                    }
                    continue;
                }

                // Not in quotes
                if (c == '"' || c == '\'') {
                    inQuotes = true;
                    quoteChar = c;
                    continue;
                }

                if (char.IsWhiteSpace(c)) {
                    // Treat any run of whitespace as separator
                    FlushToken();
                    continue;
                }

                sb.Append(c);
            }

            if (escaping) {
                // Trailing backslash; interpret literally
                sb.Append('\\');
            }

            if (inQuotes) {
                throw new FormatException("Unclosed quote in argument string.");
            }

            FlushToken();
            return args.ToArray();
        }
        #endregion

        #region Extensions
        public static string TrimStart(this string str, string trimString, System.StringComparison cmp = System.StringComparison.Ordinal)
            => str != null && trimString != null && str.StartsWith(trimString, cmp) ? str[trimString.Length..] : str;
        public static string TrimEnd(this string str, string trimString, System.StringComparison cmp = System.StringComparison.Ordinal)
            => str != null && trimString != null && str.EndsWith(trimString, cmp) ? str[..^trimString.Length] : str;


        /// <summary>
        /// Repeat the string for count times (fractional is fine).
        /// </summary>
        public static string Repeat(this string s, float count) {
            // handle null/empty and non-positive counts
            if (string.IsNullOrEmpty(s) || count <= 0f) {
                return string.Empty;
            }

            int len = s.Length;

            // integer part
            int full = Mathf.FloorToInt(count);

            // fractional part
            float frac = count - full;

            // clamp due to floating-point noise
            if (frac < 0f) frac = 0f;
            if (frac > 1f) frac = 1f;

            // add a tiny epsilon to reduce cases like 0.5f * 3 -> 1.4999999f
            int extra = Mathf.FloorToInt(frac * len + 1e-6f);

            // pre-calc total length (guard overflow)
            long totalLenLong = (long)full * len + extra;
            if (totalLenLong > int.MaxValue) {
                throw new OverflowException("Resulting string is too large.");
            }

            int totalLen = (int)totalLenLong;

            var sb = new StringBuilder(totalLen);

            for (int i = 0; i < full; i++) {
                sb.Append(s);
            }

            if (extra > 0) {
                sb.Append(s, 0, extra);
            }

            return sb.ToString();
        }
        #endregion
    }
}
