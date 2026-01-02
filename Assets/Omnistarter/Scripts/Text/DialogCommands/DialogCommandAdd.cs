// author: Omnistudio
// version: 2026.01.02

using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

namespace Omnis.Text
{
    public class DialogCommandAdd : IDialogCommand
    {
        public IEnumerator Execute(string[] args, DialogManager ctx) {
            if (args.Length < 3) {
                throw new ArgumentException("Too few arguments, needs to be exactly two arguments: flag name, delta value (float).");
            } else if (args.Length > 3) {
                throw new ArgumentException("Too many arguments, needs to be exactly two arguments: flag name, delta value (float).");
            }

            float delta, original;

            if (!float.TryParse(args[2], NumberStyles.Float, CultureInfo.InvariantCulture, out delta)) {
                throw new ArgumentException($"Invalid argument: {args[2]}. The delta value must be a float number.");
            } else if (!float.TryParse(ctx.flags[args[1]], NumberStyles.Float, CultureInfo.InvariantCulture, out original)) {
                throw new ArgumentException($"Invalid argument: {args[1]}. The flag is existed and not a float number: {ctx.flags[args[1]]}.");
            }

            ctx.flags[args[1]] = (original + delta).ToString(CultureInfo.InvariantCulture);
            Debug.Log($"Added '{args[2]}' to '{args[1]}'.");
            yield break;
        }
    }
}
