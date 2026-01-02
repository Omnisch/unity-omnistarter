// author: Omnistudio
// version: 2026.01.02

using Omnis.Text.Conditions;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis.Text
{
    public class Blackboard : IBlackboard
    {
        private readonly Dictionary<string, Value> _dict;

        public Blackboard() {
            _dict = new();
        }

        public Value this[string name] {
            set {
                if (_dict.ContainsKey(name)) {
                    _dict[name] = value;
                } else {
                    _dict.Add(name, value);
                }
                Debug.Log($"[Blackboard] {name} = {value}");
            }
        }


        public bool TryAdd(string name, Value value) {
            bool ok = _dict.TryAdd(name, value);
            if (ok) {
                Debug.Log($"[Blackboard] {name} = {value}");
            } else {
                Debug.LogWarning($"[Blackboard] {name} existed.");
            }
            return ok;
        }
        public bool TryGet(string name, out Value value) => _dict.TryGetValue(name, out value);
    }
}
