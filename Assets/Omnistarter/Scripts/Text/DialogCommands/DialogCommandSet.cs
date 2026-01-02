// author: Omnistudio
// version: 2026.01.02

using Omnis.Text.Conditions;
using System.Collections;

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

            ctx.blackboard[args[1]] = Value.Auto(args[2]);
            yield break;
        }
    }
}
