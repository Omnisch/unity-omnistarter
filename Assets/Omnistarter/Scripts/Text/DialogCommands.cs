// author: Omnistudio
// version: 2026.01.02

using System.Collections;
using System.Collections.Generic;

namespace Omnis.Text
{
    public interface IDialogCommand {
        IEnumerator Execute(string[] args, DialogManager ctx);
    }

    public sealed class DialogCommands {
        private readonly Dictionary<string, IDialogCommand> _map = new();
        public void Register(string name, IDialogCommand cmd) => _map[name] = cmd;
        public bool TryGet(string name, out IDialogCommand cmd) => _map.TryGetValue(name, out cmd);
    }
}
