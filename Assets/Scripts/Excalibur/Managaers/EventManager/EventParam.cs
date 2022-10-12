using System.Collections;

namespace Excalibur
{
    public class EventParam : IQuoteConstraint<EventParam>
    {
        EventHandle _eventType;

        int _setIndex;
        int _getIndex;
        Hashtable _params;

        public EventParam()
        {
            ResetParam();
        }

        public EventParam(EventHandle eventType)
        {
            ResetParam(eventType);
        }

        public EventHandle EventType
        {
            get { return _eventType; }
            private set { }
        }

        public T GetParam<T>()
        {
            if (_getIndex >= 0 && _getIndex < _params.Count)
            {
                return (T)_params[_getIndex++];
            }

            return default(T);
        }

        public void AddParam<T>(T param)
        {
            _params.Add(_setIndex++, typeof(T));
        }

        public void ResetParam(EventHandle eventType = EventHandle.ET_NONE)
        {
            _eventType = eventType;
            _setIndex = 0;
            _getIndex = 0;
            if (_params == null)
            {
                _params = new Hashtable();
            }
            else
            {
                _params.Clear();
            }
        }

        public void Free()
        {
            ResetParam();
        }
    }
}
