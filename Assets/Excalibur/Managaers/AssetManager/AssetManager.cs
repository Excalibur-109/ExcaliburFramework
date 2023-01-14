using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace Excalibur
{
    public sealed class AssetManager : Singleton<AssetManager>
    {
        public T LoadAsset<T>(AssetType assetType, string name) where T : UnityEngine.Object
        {

#if UNITY_EDITOR
            T result = (T)AssetDatabase.LoadAssetAtPath(AssetPath.GetAssetPath(assetType, name), typeof(T));
#else
            Log.General("·ÇUnityEditor");
#endif
            return result;
        }

        public T LoadAsset<T>(string assetPath, string name) where T : UnityEngine.Object
        {
            T result = (T)AssetDatabase.LoadAssetAtPath(assetPath + name, typeof(T));
            return result;
        }
    }
}
