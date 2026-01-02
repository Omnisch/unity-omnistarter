// author: ChatGPT
// version: 2026.01.02

using System;
using System.Globalization;
using UnityEngine;

namespace Omnis.Text.Conditions
{
    public enum ValueKind { Null, Bool, Number, String }

    public readonly struct Value
    {
        public readonly ValueKind Kind;
        public readonly bool Bool;
        public readonly double Number;
        public readonly string String;

        private Value(ValueKind kind, bool b, double n, string s) {
            Kind = kind; Bool = b; Number = n; String = s;
        }

        public static Value From(bool b) => new(ValueKind.Bool, b, 0, null);
        public static Value From(double n) => new(ValueKind.Number, false, n, null);
        public static Value From(string s) => new(ValueKind.String, false, 0, s);
        public static Value Auto(string s) {
            if (s == null || s.Length == 0) {
                return Null;
            } else if (double.TryParse(s, out double d)) {
                return From(d);
            } else if (bool.TryParse(s, out bool b)) {
                return From(b);
            } else {
                return From(s);
            }
        }
        public static readonly Value Null = new(ValueKind.Null, false, 0, null);

        public override string ToString() {
            return Kind switch {
                ValueKind.Bool => $"{Bool} ({Kind})",
                ValueKind.Number => $"{Number} ({Kind})",
                ValueKind.String => $"{String} ({Kind})",
                _ => "Null (Null)"
            };
        }
    }

    public interface IBlackboard
    {
        bool TryGet(string name, out Value value);
    }

    public interface ICondition
    {
        bool Evaluate(IBlackboard bb);
    }

    public interface IValueExpr
    {
        Value Evaluate(IBlackboard bb);
    }

    public sealed class VarExpr : IValueExpr
    {
        private readonly string _name;
        public VarExpr(string name) => _name = name;

        public Value Evaluate(IBlackboard bb)
            => bb.TryGet(_name, out var v) ? v : Value.Null;
    }

    public sealed class LiteralExpr : IValueExpr
    {
        private readonly Value _value;
        public LiteralExpr(Value value) => _value = value;
        public Value Evaluate(IBlackboard bb) => _value;
    }

    public enum CmpOp { Eq, Ne, Gt, Ge, Lt, Le }

    public sealed class CompareCond : ICondition
    {
        private readonly IValueExpr _a, _b;
        private readonly CmpOp _op;
        public CompareCond(IValueExpr a, CmpOp op, IValueExpr b) { _a = a; _op = op; _b = b; }

        public bool Evaluate(IBlackboard bb) {
            var va = _a.Evaluate(bb);
            var vb = _b.Evaluate(bb);

            if (_op is CmpOp.Gt or CmpOp.Ge or CmpOp.Lt or CmpOp.Le) {
                if (va.Kind != ValueKind.Number || vb.Kind != ValueKind.Number) {
                    Debug.LogError("The expressions must be comparable, i.e. numbers.");
                    return false;
                }
                return _op switch {
                    CmpOp.Gt => va.Number >  vb.Number,
                    CmpOp.Ge => va.Number >= vb.Number,
                    CmpOp.Lt => va.Number <  vb.Number,
                    CmpOp.Le => va.Number <= vb.Number,
                    _ => false
                };
            }

            // Eq / Ne
            bool eq = (va.Kind, vb.Kind) switch {
                (ValueKind.Bool, ValueKind.Bool) => va.Bool == vb.Bool,
                (ValueKind.Number, ValueKind.Number) => va.Number == vb.Number,
                (ValueKind.String, ValueKind.String) => string.Equals(va.String, vb.String, StringComparison.Ordinal),
                _ => false
            };
            return _op == CmpOp.Eq ? eq : !eq;
        }
    }

    public sealed class BoolAtomCond : ICondition
    {
        private readonly IValueExpr _expr;
        public BoolAtomCond(IValueExpr expr) => _expr = expr;

        public bool Evaluate(IBlackboard bb) {
            var v = _expr.Evaluate(bb);
            return v.Kind == ValueKind.Bool && v.Bool;
        }
    }

    public sealed class NotCond : ICondition
    {
        private readonly ICondition _inner;
        public NotCond(ICondition inner) => _inner = inner;
        public bool Evaluate(IBlackboard bb) => !_inner.Evaluate(bb);
    }

    public sealed class AndCond : ICondition
    {
        private readonly ICondition _l, _r;
        public AndCond(ICondition l, ICondition r) { _l = l; _r = r; }
        public bool Evaluate(IBlackboard bb) => _l.Evaluate(bb) && _r.Evaluate(bb); // short-circuit
    }

    public sealed class OrCond : ICondition
    {
        private readonly ICondition _l, _r;
        public OrCond(ICondition l, ICondition r) { _l = l; _r = r; }
        public bool Evaluate(IBlackboard bb) => _l.Evaluate(bb) || _r.Evaluate(bb); // short-circuit
    }

    public static class ConditionCompiler
    {
        public static ICondition Compile(string text) {
            var p = new Parser(text);
            var cond = p.ParseOr();
            p.Expect(TokenType.End);
            return cond;
        }

        enum TokenType
        {
            Identifier, Number, String, True, False,
            And, Or, Not,
            Eq, Ne, Gt, Ge, Lt, Le,
            LParen, RParen,
            End
        }

        readonly struct Token
        {
            public readonly TokenType Type;
            public readonly string Text;
            public readonly int Pos;
            public Token(TokenType t, string s, int p) { Type = t; Text = s; Pos = p; }
        }

        sealed class Parser
        {
            private readonly string _s;
            private int _i;
            private Token _cur;

            public Parser(string s) {
                _s = s ?? "";
                _i = 0;
                _cur = NextToken();
            }

            public ICondition ParseOr() {
                var left = ParseAnd();
                while (Match(TokenType.Or)) {
                    var right = ParseAnd();
                    left = new OrCond(left, right);
                }
                return left;
            }

            private ICondition ParseAnd() {
                var left = ParseUnary();
                while (Match(TokenType.And)) {
                    var right = ParseUnary();
                    left = new AndCond(left, right);
                }
                return left;
            }

            private ICondition ParseUnary() {
                if (Match(TokenType.Not))
                    return new NotCond(ParseUnary());
                return ParseAtomOrCompare();
            }

            private ICondition ParseAtomOrCompare() {
                if (Match(TokenType.LParen)) {
                    var inner = ParseOr();
                    Expect(TokenType.RParen);
                    return inner;
                }

                // first, parse a value
                var leftValue = ParseValueExpr();

                // if followed by a comparison
                if (_cur.Type is TokenType.Eq or TokenType.Ne or TokenType.Gt or TokenType.Ge or TokenType.Lt or TokenType.Le) {
                    var opTok = _cur;
                    _cur = NextToken();
                    var rightValue = ParseValueExpr();

                    var op = opTok.Type switch {
                        TokenType.Eq => CmpOp.Eq,
                        TokenType.Ne => CmpOp.Ne,
                        TokenType.Gt => CmpOp.Gt,
                        TokenType.Ge => CmpOp.Ge,
                        TokenType.Lt => CmpOp.Lt,
                        TokenType.Le => CmpOp.Le,
                        _ => throw Error("unknown compare op")
                    };
                    return new CompareCond(leftValue, op, rightValue);
                }

                // else treat the value as a bool atom
                return new BoolAtomCond(leftValue);
            }

            private IValueExpr ParseValueExpr() {
                var t = _cur;

                if (Match(TokenType.True)) return new LiteralExpr(Value.From(true));
                if (Match(TokenType.False)) return new LiteralExpr(Value.From(false));

                if (Match(TokenType.Number)) {
                    if (!double.TryParse(t.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var n))
                        throw Error($"bad number: {t.Text}");
                    return new LiteralExpr(Value.From(n));
                }

                if (Match(TokenType.String))
                    return new LiteralExpr(Value.From(t.Text)); // assume that Text is the content without quotes

                if (Match(TokenType.Identifier))
                    return new VarExpr(t.Text);

                throw Error($"unexpected token: {t.Type}");
            }

            public void Expect(TokenType type) {
                if (_cur.Type != type) throw Error($"expected {type} but got {_cur.Type}");
                _cur = NextToken();
            }

            private bool Match(TokenType type) {
                if (_cur.Type != type) return false;
                _cur = NextToken();
                return true;
            }

            private Exception Error(string msg) => new FormatException($"{msg} at {_cur.Pos}");

            private Token NextToken() {
                // skip white spaces
                while (_i < _s.Length && char.IsWhiteSpace(_s[_i])) _i++;
                if (_i >= _s.Length) return new Token(TokenType.End, "", _i);

                int start = _i;
                char c = _s[_i];

                // parenthesis
                if (c == '(') { _i++; return new Token(TokenType.LParen, "(", start); }
                if (c == ')') { _i++; return new Token(TokenType.RParen, ")", start); }

                // comparison
                if (_i + 1 < _s.Length) {
                    var two = _s.Substring(_i, 2);
                    if (two == "==") { _i += 2; return new Token(TokenType.Eq, two, start); }
                    if (two == "!=") { _i += 2; return new Token(TokenType.Ne, two, start); }
                    if (two == ">=") { _i += 2; return new Token(TokenType.Ge, two, start); }
                    if (two == "<=") { _i += 2; return new Token(TokenType.Le, two, start); }
                }
                if (c == '>') { _i++; return new Token(TokenType.Gt, ">", start); }
                if (c == '<') { _i++; return new Token(TokenType.Lt, "<", start); }

                // strings
                if (c == '"' || c == '\'') {
                    char q = c; _i++;
                    int contentStart = _i;
                    while (_i < _s.Length && _s[_i] != q) _i++;
                    if (_i >= _s.Length) throw new FormatException($"unclosed string at {start}");
                    string content = _s[contentStart.._i];
                    _i++; // skip quote
                    return new Token(TokenType.String, content, start);
                }

                // numbers
                if (char.IsDigit(c) || c == '.') {
                    _i++;
                    while (_i < _s.Length) {
                        char k = _s[_i];
                        if (char.IsDigit(k) || k == '.' || k == 'e' || k == 'E' || k == '+' || k == '-') _i++;
                        else break;
                    }
                    return new Token(TokenType.Number, _s[start.._i], start);
                }

                // keywords
                if (char.IsLetter(c) || c == '_') {
                    _i++;
                    while (_i < _s.Length && (char.IsLetterOrDigit(_s[_i]) || _s[_i] == '_')) _i++;
                    var word = _s[start.._i];

                    return word switch {
                        "and" => new Token(TokenType.And, word, start),
                        "or" => new Token(TokenType.Or, word, start),
                        "not" => new Token(TokenType.Not, word, start),
                        "true" => new Token(TokenType.True, word, start),
                        "false" => new Token(TokenType.False, word, start),
                        _ => new Token(TokenType.Identifier, word, start)
                    };
                }

                throw new FormatException($"unexpected char '{c}' at {start}");
            }
        }
    }
}
