// author: Omnistudio
// version: 2026.01.02

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Omnis.Text
{
    public partial class DialogManager
    {
        public void ReadDialogIniFromBrowser() {
            var extensionsIni = new[] { new SFB.ExtensionFilter("Ini Files", "ini") };
            SFB.StandaloneFileBrowser.OpenFilePanelAsync("Load Ini", "", extensionsIni, multiselect: false, (paths) => {
                if (paths.Length > 0) {
                    Debug.Log($"Read dialog ini file '{paths[0]}'");
                    ReadDialogIni(paths[0]);
                } else {
                    Debug.LogWarning("No dialog ini files selected, operation canceled.");
                }
            });
        }

        public void ReadDialogIni(string path) {
            flags = new();
            IniStorage ini = new(path);
            foreach (var line in ini.pairs) {
                flags.TryAdd(line.Name, line.Get());
                Debug.Log($"[Dialog Ini] {line.Name}: {line.Get()}");
            }
        }



        public void ReadDialogScriptFromBrowser() {
            var extensionsTxt = new[] { new SFB.ExtensionFilter("Text Files", "txt") };
            SFB.StandaloneFileBrowser.OpenFilePanelAsync("Load Dialogs", "", extensionsTxt, multiselect: false, (paths) => {
                if (paths.Length > 0) {
                    Debug.Log($"Read dialog script '{paths[0]}'");
                    ReadDialogScript(paths[0]);
                    TryEnter("start");
                } else {
                    Debug.LogWarning("No dialog scripts selected, operation canceled.");
                }
            });
        }

        public void ReadDialogScript(string path) {
            dialogScript = new();

            string entryName = "";
            var entry = new List<EntryBranch>();
            var branch = new EntryBranch();
            var actorLine = new ActorLine();

            string[] texts = File.ReadAllLines(path);

            for (int i = 0; i < texts.Length; i++) {
                var line = texts[i].Trim();

                // skip empty or space lines
                if (line.Length == 0)
                    continue;

                // entry
                if (line.StartsWith("#")) {
                    // push last entry to the list
                    if (entryName != "")
                        dialogScript.Add(entryName, entry);

                    // record the name of new entry
                    entry = new();
                    entryName = line[1..].Trim().ToLowerInvariant();
                    branch = new();
                }

                // flag conditions
                else if (line.StartsWith("if ")) {
                    string content = line[3..].Trim();
                    int equalsSignIndex = content.IndexOf('=');
                    if (equalsSignIndex == -1) {
                        Debug.LogError($"Illegal syntax at text {i}: '{line}'.");
                    } else {
                        // add one condition to current branch node
                        branch.conditions.Add(new(content[..equalsSignIndex].Trim(), content[(equalsSignIndex + 1)..].Trim()));
                    }
                }

                // commands, before the speech text
                else if (line.StartsWith("/")) {
                    string content = line[1..].Trim();
                    string[] args = Utils.StringHelper.ParseArgs(content);
                    if (args.Length < 1) {
                        Debug.LogError($"Illegal command at text {i}: '{line}'.");
                    } else if (args.Length == 1) {
                        actorLine.cmds.Add(new() { keyword = args[0] });
                    } else {
                        actorLine.cmds.Add(new() { keyword = args[0], args = args[1..] });
                    }
                }

                // next entry
                else if (line.StartsWith("goto ")) {
                    string content = line[5..].Trim();
                    branch.nextEntry = content;
                    // push uncovered commands
                    if (actorLine.cmds.Count > 0) {
                        branch.actorLines.Add(actorLine);
                    }
                    // push branch
                    entry.Add(branch);
                    branch = new();
                }

                // all other lines will try to parse as speeches
                else {
                    int colonIndex = line.IndexOf(":");
                    if (colonIndex == -1) {
                        Debug.LogError($"Invalid text at \"{line}\"");
                    } else {
                        actorLine.actorId = line[..colonIndex].Trim();
                        actorLine.text = line[(colonIndex + 1)..].Trim();

                        branch.actorLines.Add(actorLine);
                        actorLine = new();
                    }
                }
            }

            // push the last entry to the list
            if (entryName != "")
                dialogScript.Add(entryName, entry);
        }
    }
}
