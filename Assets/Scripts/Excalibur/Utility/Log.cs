using UnityEngine;

namespace Excalibur
{
    public static class Log
    {
        public static void General(object msg)
        {
            Debug.Log(msg);
        }

        public static void General(string msg, params object[] objects)
        {
            Debug.LogFormat(msg, objects);
        }

        public static void Warning(string msg)
        {
            Debug.LogWarning(msg);
        }

        public static void Warning(string msg, params object[] objects)
        {
            Debug.LogWarningFormat(msg, objects);
        }

        public static void Error(string msg)
        {
            Debug.LogError(msg);
        }

        public static void Error(string msg, params object[] objects)
        {
            Debug.LogErrorFormat(msg, objects);
        }

        public static void Assertion(string msg)
        {
            Debug.LogAssertion(msg);
        }

        public static void Assertion(string msg, params object[] objects)
        {
            Debug.LogAssertionFormat(msg, objects);
        }
    }
}