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
        public static Transform FindChildRecursively(this Transform transform, string objectname)
        {
            Transform trans = transform.Find(objectname);

            if (trans != null)
            {
                return trans;
            }

            for (int i = 0; i < transform.childCount; ++i)
            {
                trans = transform.GetChild(i).FindChildRecursively(objectname);
                if (trans != null)
                {
                    return trans;
                }
            }

            return trans;
        }

        public static GameObject FindChildGameObjectRecursively(this Transform transform, string objectname)
        {
            Transform target = transform.FindChildRecursively(objectname);
            if (target != null)
            {
                return target.gameObject;
            }
            return null;
        }

        public static T GetChildComponentRecursively<T>(this Transform transform, string objectname) where T : Component
        {
            Transform target = transform.FindChildRecursively(objectname);
            if (target != null)
            {
                return target.GetComponent<T>();
            }
            return null;
        }

        public static T AddChildComponentRecursively<T>(this Transform transform, string objectname, out T component) where T : Component
        {
            try
            {
                GameObject go = transform.FindChildGameObjectRecursively(objectname);
                var ct = go.GetComponent<T>();
                component = ct == null ? go.AddComponent<T>() : ct;
                return component;
            }
            catch (NullReferenceException)
            {
                throw;
            }
        }

        public static T AddChildComponentRecursively<T>(this Transform transform, string objectname) where T : Component
        {
            try
            {
                GameObject go = transform.FindChildGameObjectRecursively(objectname);
                var ct = go.GetComponent<T>();
                if (ct == null)
                {
                    ct = go.AddComponent<T>();
                }
                return ct;
            }
            catch (NullReferenceException)
            {
                throw;
            }
        }

        public static T AddChildComponentRecursively<T>(this GameObject gameObject, string objectname, out T component) where T : Component
        {
            try
            {
                gameObject.transform.AddChildComponentRecursively(objectname, out component);
                return component;
            }
            catch (NullReferenceException)
            {
                throw;
            }
        }

        public static T AddChildComponentRecursively<T>(this GameObject gameObject, string objectname) where T : Component
        {
            try
            {
                return gameObject.transform.AddChildComponentRecursively<T>(objectname);
            }
            catch (NullReferenceException)
            {
                throw;
            }
        }

        public static T AddComponentSelf<T>(this GameObject gameObject) where T : Component
        {
            var ct = gameObject.GetComponent<T>();
            if (ct == null)
            {
                ct = gameObject.AddComponent<T>();
            }
            return ct;
        }

        public static T AddComponentSelf<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.AddComponentSelf<T>();
            return component;
        }

        public static T AddComponentSelf<T>(this Transform transform) where T : Component
        {
            return transform.gameObject.AddComponentSelf<T>();
        }

        public static T AddComponentSelf<T>(this Transform transform, out T component) where T : Component
        {
            component = transform.AddComponentSelf<T>();
            return component;
        }

        public static void SetActivate<T>(this T component, bool active) where T : Component
        {
            component.gameObject.SetActivate(active);
        }

        public static void SetActivate(this GameObject gameObject, bool active)
        {
            if (gameObject.activeSelf != active)
            {
                gameObject.SetActive(active);
            }
        }

        public static void SetText(this UnityEngine.UI.Text text, string content)
        {
            if (text.text != content)
            {
                text.text = content;
            }
        }

        public static void SetText(this TMPro.TextMeshProUGUI text, string content)
        {
            if (text.text != content)
            {
                text.text = content;
            }
        }

        public static void SetText(this TMPro.TextMeshPro text, string content)
        {
            if (text.text != content)
            {
                text.text = content;
            }
        }
    }
}