// author: Omnistudio
// version: 2026.01.02

using Omnis.Text.Conditions;
using System;
using System.Collections;
using System.Globalization;

namespace Omnis.Text
{
    public class DialogCommandAdd : IDialogCommand
    {
        private static readonly double DefaultValue = 0;

        /// <summary>
        /// /add &lt;flag&gt; &lt;delta_value&gt;
        /// </summary>
        public IEnumerator Execute(string[] args, DialogManager ctx) {
            if (args.Length < 3) {
                throw new ArgumentException("Too few arguments, needs to be exactly two arguments: flag name, delta value (number).");
            } else if (args.Length > 3) {
                throw new ArgumentException("Too many arguments, needs to be exactly two arguments: flag name, delta value (number).");
            }

            if (!double.TryParse(args[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double delta)) {
                throw new ArgumentException($"Invalid argument: {args[2]}. The delta value must be a number.");
            }

            double original = DefaultValue;

            // flag not existed
            if (!ctx.blackboard.TryGet(args[1], out var flagValue)) {
                ctx.blackboard[args[1]] = Value.From(DefaultValue);
            }
            // flag existed but not a number
            else if (flagValue.Kind is not ValueKind.Number) {
                throw new ArgumentException($"Invalid argument: {args[1]}. The flag is existed but not a number: {flagValue}.");
            }

            ctx.blackboard[args[1]] = Value.From(original + delta);
            yield break;
        }
    }
}
