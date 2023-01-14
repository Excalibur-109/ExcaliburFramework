using UnityEngine;

namespace Excalibur
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour, new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }

            DontDestroyOnLoad(this);
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void OnMouseDown()
        {
            
        }

        public static bool Initialized()
        {
            return _instance != null;
        }
    }
}
