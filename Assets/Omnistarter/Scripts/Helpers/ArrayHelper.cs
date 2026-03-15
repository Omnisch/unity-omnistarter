// author: Omnistudio
// version: 2026.03.14

using System.Collections.Generic;
using UnityEngine;

namespace Omnis.Utils
{
    public static class ArrayHelper
    {
        public static T PickRandom<T>(this T[] array) {
            return array[Random.Range(0, array.Length)];
        }

        public static T PickRandom<T>(this List<T> array) {
            return array[Random.Range(0, array.Count)];
        }
    }
}
