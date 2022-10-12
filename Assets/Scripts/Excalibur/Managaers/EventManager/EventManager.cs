using System;
using System.Collections.Generic;

namespace Excalibur
{
    public sealed class EventManager : Singleton<EventManager>
    {
        Dictionary<EventHandle, EventHandler> _events;
        
        protected override void Init()
        {
            base.Init();

            _events = new Dictionary<EventHandle, EventHandler>();
        }

        public void SurveilEvent(EventHandle eventType, EventHandler handler)
        {
            if (!_events.ContainsKey(eventType))
            {
                _events.Add(eventType, new EventHandler(handler));
            }
            else
            {
                _events[eventType] -= handler;
                _events[eventType] += handler;
            }
        }

        public void HandleEvent(EventParam eventData)
        {
            if (_events.TryGetValue(eventData.EventType, out EventHandler handler))
            {
                handler.Invoke(eventData);
            }
        }
    }
}
