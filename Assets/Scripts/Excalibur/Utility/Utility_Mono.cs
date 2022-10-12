using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Excalibur
{
    // Monobehaviour Component
    public static partial class Utility
    {
        public static void SetActive(GameObject go, bool active)
        {
            if (go.activeSelf != active)
            {
                go.SetActive(active);
            }
        }

        public static void SetActive<T>(T t, bool active) where T : Component
        {
            if (t.gameObject.activeSelf != active)
            {
                t.gameObject.SetActive(active);
            }
        }

        public static void SetParent(Transform transform, Transform parent, bool worldPosition = true)
        {
            transform.SetParent(parent, worldPosition);
        }

        public static void SetParent<T>(T t, Transform parent, bool worldPosition = true) where T : Component
        {
            if (t.transform.parent.transform == parent)
                return;

            SetParent(t.transform, parent, worldPosition);
        }

        public static GameObject CreateGameObject(string name)
        {
            GameObject go = new GameObject(name);
            return go;
        }

        public static GameObject CreateGameObject(string name, bool worldObject)
        {
            GameObject go = CreateGameObject(name);
            if (!worldObject)
            {
                RectTransform rect = go.AddComponent<RectTransform>();
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.one;
            }
            return go;
        }

        public static GameObject CreateGameObject(string name, bool worldObject, Transform parent)
        {
            GameObject go = CreateGameObject(name, worldObject);
            go.transform.SetParent(parent);
            return go;
        }
        public static Transform FindRecursively(Transform parent, string objectname)
        {
            Transform trans = parent.Find(objectname);
            if (trans != null)
            {
                return trans;
            }

            for (int i = 0; i < parent.childCount; ++i)
            {
                trans = FindRecursively(parent.GetChild(i), objectname);
                if (trans != null)
                {
                    return trans;
                }
            }

            return trans;
        }
    }
}
