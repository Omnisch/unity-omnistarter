// author: Omnistudio
// version: 2025.08.06

using System;
using UnityEngine;

namespace Omnis.Utils
{
    public static class THelper
    {
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
