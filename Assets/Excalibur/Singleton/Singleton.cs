
namespace Excalibur
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static T _instance;

        private static object locker = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                return _instance;

            }
            private set { }
        }

        protected Singleton()
        {
            Init();
        }

        protected virtual void Init()
        {

        }

        public bool Initialized()
        {
            return _instance != null;
        }
    }
}
