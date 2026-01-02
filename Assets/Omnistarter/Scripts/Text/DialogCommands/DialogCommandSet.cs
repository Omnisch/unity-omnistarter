// author: Omnistudio
// version: 2026.01.02

using System.Collections;
using UnityEngine;

namespace Omnis.Text
{
    public class DialogCommandSet : IDialogCommand
    {
        public IEnumerator Execute(string[] args, DialogManager ctx) {
            if (args.Length < 3) {
                throw new System.ArgumentException("Too few arguments, needs to be exactly two arguments: flag name, flag value.");
            } else if (args.Length > 3) {
                throw new System.ArgumentException("Too many arguments, needs to be exactly two arguments: flag name, flag value.");
            }

            ctx.flags[args[1]] = args[2];
            Debug.Log($"Set '{args[1]}' to '{args[2]}'.");
            yield break;
        }
    }
}
