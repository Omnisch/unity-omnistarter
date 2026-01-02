// author: Omnistudio
// version: 2026.01.02

using Omnis.Text.Conditions;
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
            blackboard = new();
            IniStorage ini = new(path);
            foreach (var line in ini.pairs) {
                blackboard.TryAdd(line.Name, Value.Auto(line.Get()));
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

            void PushActorLine() {
                if (actorLine.actorId != null)
                    branch.actorLines.Add(actorLine);
                actorLine = new();
            }
            void PushBranch() {
                PushActorLine();
                if (branch.actorLines.Count > 0)
                    entry.Add(branch);
                branch = new();
            }
            void PushEntry() {
                PushBranch();
                if (entryName != "")
                    dialogScript.Add(entryName, entry);
                entry = new();
            }

            string[] texts = File.ReadAllLines(path);

            for (int i = 0; i < texts.Length; i++) {
                var line = texts[i].Trim();

                // skip empty or space lines
                if (line.Length == 0)
                    continue;

                // new entry
                if (line.StartsWith("#")) {
                    PushEntry();

                    // record the name of new entry
                    entryName = line[1..].Trim().ToLowerInvariant();
                }

                // conditions
                else if (line.StartsWith("if ")) {
                    string content = line[3..].Trim();
                    branch.cond = ConditionCompiler.Compile(content);
                }

                // commands, before the speech text
                else if (line.StartsWith("/")) {
                    string content = line[1..].Trim();
                    string[] args = Utils.StringHelper.ParseArgs(content);
                    if (args.Length < 1) {
                        Debug.LogError($"Illegal command at text {i}: '{line}'.");
                    } else {
                        actorLine.cmds.Add(new() { keyword = args[0], args = args });
                    }
                }
                
                // end w/ next entry
                else if (line.StartsWith("goto ")) {
                    string content = line[5..].Trim();
                    branch.nextEntry = content;
                    PushBranch();
                }

                // end w/o next entry
                else if (line == "end") {
                    PushBranch();
                }

                // all other lines will try to parse as speeches
                else {
                    int colonIndex = line.IndexOf(":");
                    if (colonIndex == -1) {
                        Debug.LogError($"Invalid text at \"{line}\"");
                    } else {
                        actorLine.actorId = line[..colonIndex].Trim();
                        actorLine.text = line[(colonIndex + 1)..].Trim();
                        PushActorLine();
                    }
                }
            }

            PushEntry();
        }
    }
}
