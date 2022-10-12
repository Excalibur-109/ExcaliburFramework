using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excalibur
{
    public sealed class MonoEventManager : MonoSingleton<MonoEventManager>
    {
        event EventHandler onEventHandle;

        Queue<EventParam> _eventDatasDynamic;
        Queue<EventParam> _tempEventDatas;

        protected override void Awake()
        {
            base.Awake();

            _eventDatasDynamic = new Queue<EventParam>();
            _tempEventDatas = new Queue<EventParam>();
        }

        public void SurveilEvent(EventHandler handler)
        {
            onEventHandle -= handler;
            onEventHandle += handler;
        }

        public void HandleEvent(EventParam eventData)
        {
            if (eventData.EventType > EventHandle.KEY_CODE_BEGIN && eventData.EventType < EventHandle.KEY_CODE_END)
            {
                onEventHandle.Invoke(eventData);
            }
            else
            {
                _eventDatasDynamic.Enqueue(eventData);
            }
        }

        public void Execute()
        {
            if (_eventDatasDynamic.Count > 0)
            {
                foreach (var item in _eventDatasDynamic)
                {
                    _tempEventDatas.Enqueue(item);
                }
                _eventDatasDynamic.Clear();
            }

            if (_tempEventDatas.Count > 0)
            {
                foreach (var item in _tempEventDatas)
                {
                    onEventHandle.Invoke(item);
                }
                _tempEventDatas.Clear();
            }
        }
    }
}
