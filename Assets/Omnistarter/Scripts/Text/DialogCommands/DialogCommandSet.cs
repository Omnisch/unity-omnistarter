// author: Omnistudio
// version: 2026.01.02

using System.Collections;
using UnityEngine;

namespace Omnis.Text
{
    public class DialogCommandSet : IDialogCommand
    {
        public IEnumerator Execute(string[] args, DialogManager ctx) {
            if (args.Length < 2) {
                throw new System.ArgumentException("Too few arguments, needs to be exactly two arguments: flag name, flag value.");
            } else if (args.Length > 2) {
                throw new System.ArgumentException("Too many arguments, needs to be exactly two arguments: flag name, flag value.");
            }

            ctx.flags[args[0]] = args[1];
            Debug.Log($"Set '{args[0]}' to '{args[1]}'.");
            yield break;
        }
    }
}
