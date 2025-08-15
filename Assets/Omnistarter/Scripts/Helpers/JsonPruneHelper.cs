// author: Omnistudio
// version: 2025.08.15

using System;
using System.Text;
using System.Collections.Generic;

namespace Omnis.Utils
{
    public static class JsonPruneHelperLight
    {
        /// <summary>
        /// Remove all "" fields in JSON created by JsonUtility.ToJson.
        /// </summary>
        /// <returns>Condensed JSON</returns>
        public static string RemoveEmptyStringFields(
            string json,
            bool removeEmptyContainers = true,
            bool removeEmptyStringArrayElements = false)
        {
            if (string.IsNullOrEmpty(json)) return json;

            var parser = new Parser(json);
            Node root = parser.ParseValue();
            PruneInPlace(root, removeEmptyContainers, removeEmptyStringArrayElements, isRoot: true);

            var sb = new StringBuilder(json.Length);
            WriteJson(root, sb);
            return sb.ToString();
        }

        // ----------- AST -----------
        private abstract class Node { }
        private sealed class Obj : Node
        {
            public readonly List<Member> Members = new();
        }
        private sealed class Arr : Node
        {
            public readonly List<Node> Items = new();
        }
        private sealed class Str : Node
        {
            public readonly string Value;
            public Str(string v) { Value = v; }
        }
        private sealed class Num : Node
        {
            public readonly string Raw;
            public Num(string r) { Raw = r; }
        }
        private sealed class Lit : Node
        {
            public readonly string Raw; // true / false / null
            public Lit(string r) { Raw = r; }
        }
        private sealed class Member
        {
            public string Key;
            public Node Value;
            public Member(string k, Node v) { Key = k; Value = v; }
        }

        // ----------- Parser -----------
        private sealed class Parser
        {
            private readonly string s;
            private int i;
            public Parser(string s) { this.s = s; this.i = 0; }

            public Node ParseValue()
            {
                SkipWs();
                char c = Peek();
                if (c == '{') return ParseObject();
                if (c == '[') return ParseArray();
                if (c == '"') return new Str(ParseString());
                if (c == '-' || (c >= '0' && c <= '9')) return new Num(ParseNumber());
                if (c == 't' || c == 'f' || c == 'n') return new Lit(ParseLiteral());
                throw new FormatException($"Unexpected char '{c}' at {i}");
            }

            private Obj ParseObject()
            {
                Expect('{'); SkipWs();
                var obj = new Obj();
                if (TryConsume('}')) return obj;

                while (true)
                {
                    SkipWs();
                    string key = ParseString();
                    SkipWs(); Expect(':'); SkipWs();
                    Node val = ParseValue();
                    obj.Members.Add(new Member(key, val));
                    SkipWs();
                    if (TryConsume('}')) break;
                    Expect(','); // next pair
                }
                return obj;
            }

            private Arr ParseArray()
            {
                Expect('['); SkipWs();
                var arr = new Arr();
                if (TryConsume(']')) return arr;

                while (true)
                {
                    SkipWs();
                    arr.Items.Add(ParseValue());
                    SkipWs();
                    if (TryConsume(']')) break;
                    Expect(',');
                }
                return arr;
            }

            private string ParseString()
            {
                Expect('"');
                var sb = new StringBuilder();
                while (true)
                {
                    if (i >= s.Length) throw new FormatException("Unterminated string");
                    char c = s[i++];
                    if (c == '"') break;
                    if (c == '\\')
                    {
                        if (i >= s.Length) throw new FormatException("Bad escape");
                        char e = s[i++];
                        switch (e)
                        {
                            case '"': sb.Append('\"'); break;
                            case '\\': sb.Append('\\'); break;
                            case '/': sb.Append('/'); break;
                            case 'b': sb.Append('\b'); break;
                            case 'f': sb.Append('\f'); break;
                            case 'n': sb.Append('\n'); break;
                            case 'r': sb.Append('\r'); break;
                            case 't': sb.Append('\t'); break;
                            case 'u':
                                if (i + 4 > s.Length) throw new FormatException("Bad \\u escape");
                                int code = Hex4(s[i], s[i + 1], s[i + 2], s[i + 3]);
                                i += 4;
                                sb.Append((char)code);
                                break;
                            default: throw new FormatException($"Unknown escape \\{e}");
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }

            private string ParseNumber()
            {
                int start = i;
                if (Peek() == '-') i++;
                ConsumeDigits(); // int
                if (TryConsume('.')) ConsumeDigits(); // frac
                if (TryConsume('e') || TryConsume('E'))
                {
                    if (TryConsume('+') || TryConsume('-')) { }
                    ConsumeDigits();
                }
                return s[start..i];
            }

            private string ParseLiteral()
            {
                if (Match("true")) return "true";
                if (Match("false")) return "false";
                if (Match("null")) return "null";
                throw new FormatException($"Unexpected literal at {i}");
            }

            // helpers
            private void ConsumeDigits()
            {
                if (i >= s.Length || s[i] < '0' || s[i] > '9')
                    throw new FormatException($"Digit expected at {i}");
                while (i < s.Length && s[i] >= '0' && s[i] <= '9') i++;
            }
            private int Hex4(char a, char b, char c, char d)
            {
                static int H(char ch) =>
                    ch >= '0' && ch <= '9' ? ch - '0' :
                    ch >= 'A' && ch <= 'F' ? 10 + ch - 'A' :
                    ch >= 'a' && ch <= 'f' ? 10 + ch - 'a' :
                    throw new FormatException("Bad hex");
                return (H(a) << 12) | (H(b) << 8) | (H(c) << 4) | H(d);
            }
            private void SkipWs()
            {
                while (i < s.Length)
                {
                    char c = s[i];
                    if (c == ' ' || c == '\t' || c == '\r' || c == '\n') i++;
                    else break;
                }
            }
            private char Peek()
            {
                if (i >= s.Length) throw new FormatException("Unexpected end");
                return s[i];
            }
            private void Expect(char c)
            {
                if (i >= s.Length || s[i] != c)
                    throw new FormatException($"Expected '{c}' at {i}");
                i++;
            }
            private bool TryConsume(char c)
            {
                if (i < s.Length && s[i] == c) { i++; return true; }
                return false;
            }
            private bool Match(string kw)
            {
                if (i + kw.Length <= s.Length && string.CompareOrdinal(s, i, kw, 0, kw.Length) == 0)
                {
                    i += kw.Length;
                    return true;
                }
                return false;
            }
        }

        // ----------- Pruner -----------
        private static void PruneInPlace(Node node, bool removeEmptyContainers, bool removeEmptyStringArrayElements, bool isRoot)
        {
            if (node is Obj o)
            {
                for (int idx = o.Members.Count - 1; idx >= 0; --idx)
                {
                    var m = o.Members[idx];

                    if (m.Value is Str s && s.Value.Length == 0)
                    {
                        o.Members.RemoveAt(idx);
                        continue;
                    }

                    PruneInPlace(m.Value, removeEmptyContainers, removeEmptyStringArrayElements, isRoot: false);

                    if (removeEmptyContainers && IsEmptyContainer(m.Value))
                    {
                        o.Members.RemoveAt(idx);
                    }
                }
            }
            else if (node is Arr a)
            {
                for (int i = a.Items.Count - 1; i >= 0; --i)
                {
                    var v = a.Items[i];

                    if (removeEmptyStringArrayElements && v is Str sv && sv.Value.Length == 0)
                    {
                        a.Items.RemoveAt(i);
                        continue;
                    }

                    PruneInPlace(v, removeEmptyContainers, removeEmptyStringArrayElements, isRoot: false);

                    if (removeEmptyContainers && IsEmptyContainer(v))
                    {
                        a.Items.RemoveAt(i);
                    }
                }
            }
        }

        private static bool IsEmptyContainer(Node n)
        {
            return (n is Obj o && o.Members.Count == 0) || (n is Arr a && a.Items.Count == 0);
        }

        // ----------- Writer -----------
        private static void WriteJson(Node n, StringBuilder sb)
        {
            switch (n)
            {
                case Obj o:
                    sb.Append('{');
                    for (int i = 0; i < o.Members.Count; i++)
                    {
                        if (i > 0) sb.Append(',');
                        WriteString(o.Members[i].Key, sb);
                        sb.Append(':');
                        WriteJson(o.Members[i].Value, sb);
                    }
                    sb.Append('}');
                    break;

                case Arr a:
                    sb.Append('[');
                    for (int i = 0; i < a.Items.Count; i++)
                    {
                        if (i > 0) sb.Append(',');
                        WriteJson(a.Items[i], sb);
                    }
                    sb.Append(']');
                    break;

                case Str s: WriteString(s.Value, sb); break;
                case Num num: sb.Append(num.Raw); break;
                case Lit lit: sb.Append(lit.Raw); break;
                default: throw new InvalidOperationException("Unknown node");
            }
        }

        private static void WriteString(string s, StringBuilder sb)
        {
            sb.Append('"');
            for (int k = 0; k < s.Length; k++)
            {
                char c = s[k];
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20)
                        {
                            sb.Append("\\u");
                            sb.Append(((int)c).ToString("X4"));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append('"');
        }
    }
}
