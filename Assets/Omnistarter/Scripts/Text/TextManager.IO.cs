// author: Omnistudio
// version: 2025.09.03

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Omnis.Text
{
    public partial class TextManager
    {
        public void ReadDialogIni(string path)
        {
            this.DialogState = new();
            IniStorage ini = new(path);
            foreach (var line in ini.pairs)
            {
                this.DialogState.TryAdd(line.Name, line.Get());
                Debug.Log($"[Dialog INI] {line.Name}: {line.Get()}");
            }
        }

        public void ReadDialogScript(string path)
        {
            this.dialogScript = new();
            string entryName = "";
            List<EntryBranch> entry = new();
            EntryBranch branch = new();

            string[] texts = File.ReadAllLines(path);

            foreach (string text in texts)
            {
                var trimmed = text.Trim();

                // Skip empty or space lines.
                if (trimmed.Length == 0)
                    continue;

                switch (trimmed[0])
                {
                    // Entry
                    case '[':
                        {
                            int rightBracketIndex = trimmed.IndexOf(']');
                            if (rightBracketIndex == -1)
                            {
                                Debug.LogError($"Missing closing bracket at \"{trimmed}\"");
                            }
                            else
                            {
                                // Add last entry to the list.
                                if (entryName != "")
                                    this.dialogScript.Add(entryName, entry);

                                // Record the name of new entry.
                                entry = new();
                                entryName = trimmed[1..rightBracketIndex];
                                branch = new();
                            }
                            break;
                        }
                    // Condition
                    case '>':
                        {
                            string content = trimmed[1..];
                            int equalsSignIndex = content.IndexOf('=');
                            if (equalsSignIndex == -1)
                            {
                                Debug.LogError($"Illegal syntax at \"{trimmed}\"");
                            }
                            else
                            {
                                // Add one condition to current branch node.
                                branch.conditions.Add(new(content[..equalsSignIndex], content[(equalsSignIndex + 1)..]));
                            }
                            break;
                        }
                    // Result
                    case '<':
                        {
                            string content = trimmed[1..];
                            int equalsSignIndex = content.IndexOf('=');
                            if (equalsSignIndex == -1)
                            {
                                if (branch.nextEntry == "")
                                {
                                    // If not an equation, set next entry.
                                    branch.nextEntry = content;
                                }
                                else
                                {
                                    // Only 1 next entry per branch.
                                    Debug.LogError($"Multiple next entry at \"{trimmed}\"");
                                }
                            }
                            else
                            {
                                // Add one condition to current branch node.
                                branch.results.Add(new(content[..equalsSignIndex], content[(equalsSignIndex + 1)..]));
                            }
                            break;
                        }
                    // Separation of branches
                    case '-':
                        {
                            entry.Add(branch);
                            branch = new();
                            break;
                        }
                    // All other lines will try to parse as speeches.
                    default:
                        {
                            int colonIndex = trimmed.IndexOf(":");
                            if (colonIndex == -1)
                            {
                                Debug.LogError($"Invalid line at \"{trimmed}\"");
                            }
                            else
                            {
                                branch.actorLines.Add(new EntryBranch.ActorLine
                                {
                                    actorId = trimmed[..colonIndex].Trim(),
                                    line = trimmed[(colonIndex + 1)..].Trim()
                                });
                            }
                            break;
                        }
                }
            }

            // Add the last entry to the list.
            if (entryName != "")
                this.dialogScript.Add(entryName, entry);
        }
    }
}
