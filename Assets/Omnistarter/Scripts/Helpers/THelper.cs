// author: Omnistudio
// version: 2025.11.02

using System;
using UnityEngine;

namespace Omnis.Utils
{
    public static class THelper
    {
        public static bool IsIntegerLike(object v) =>
            v is sbyte || v is byte || v is short || v is ushort || v is int || v is uint || v is long || v is ulong;

        public static bool ToBool(object v) {
            if (v is bool b) return b;
            if (IsIntegerLike(v)) return Convert.ToInt64(v) != 0;
            if (v is string s) return bool.TryParse(s, out var r) && r;
            return false;
        }


        public static TField GetFieldByName<TStruct, TField>(this TStruct rootObj, string fieldName)
        {
            object currObj = rootObj;
            Type currType = typeof(TStruct);

            foreach (var layer in fieldName.Split('.'))
            {
                if (currObj == null)
                {
                    Debug.LogError($"Null encountered at {layer} in {fieldName}");
                    return default;
                }

                var field = currType.GetField(layer);
                if (field != null)
                {
                    currObj = field.GetValue(currObj);
                    currType = field.FieldType;
                    continue;
                }

                Debug.LogError($"No field named \"{layer}\" in type {currType.Name}");
                return default;
            }

            try
            {
                return (TField)currObj;
            }
            catch (Exception e)
            {
                Debug.LogError($"Type mismatch: {e}");
                return default;
            }
        }
    }
}
