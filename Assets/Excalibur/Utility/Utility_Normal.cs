using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Excalibur
{
    public static partial class Utility
    {
        public static bool StrAvalible(string paramStr)
        {
            return !string.IsNullOrEmpty(paramStr) && !string.IsNullOrWhiteSpace(paramStr);
        }

        public static Vector2 ScreenVector()
        {
            return new Vector2(Screen.width, Screen.height);
        }

        public static T LoadAsset<T>(AssetType assetType, string assetName, bool intantiate = true) where T : UnityEngine.Object
        {
            T reference = AssetManager.Instance.LoadAsset<T>(assetType, assetName);
            if (intantiate)
                return (T)PrefabUtility.InstantiatePrefab(reference);
            return reference;
        }
    }
}
