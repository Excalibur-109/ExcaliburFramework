using System.Collections.Generic;
using UnityEngine;

namespace Excalibur
{
    public sealed class InputManager : MonoSingleton<InputManager>
    {
        Dictionary<EventHandle, EventParam> _inputEventDic;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            // 放Awake里面，如果MonoEventManager在InputManager后生成，MonoEventManager就不起作用
            RegistInputEvent();
        }

        private void RegistInputEvent()
        {
            if (_inputEventDic == null)
                _inputEventDic = new Dictionary<EventHandle, EventParam>();
            else
                _inputEventDic.Clear();

            for (EventHandle type = EventHandle.ET_GET_KEY_DOWN_W; type <= EventHandle.ET_GET_KEY_SPACE; ++type)
            {
                EventParam data = new EventParam(type);
                KeyCode key = KeyCode.None;
                switch (type)
                {
                    case EventHandle.ET_GET_KEY_DOWN_W:
                    case EventHandle.ET_GET_KEY_UP_W:
                    case EventHandle.ET_GET_KEY_W:
                        key = KeyCode.W;
                        break;
                    case EventHandle.ET_GET_KEY_DOWN_A:
                    case EventHandle.ET_GET_KEY_UP_A:
                    case EventHandle.ET_GET_KEY_A:
                        key = KeyCode.A;
                        break;
                    case EventHandle.ET_GET_KEY_DOWN_S:
                    case EventHandle.ET_GET_KEY_UP_S:
                    case EventHandle.ET_GET_KEY_S:
                        key = KeyCode.S;
                        break;
                    case EventHandle.ET_GET_KEY_DOWN_D:
                    case EventHandle.ET_GET_KEY_UP_D:
                    case EventHandle.ET_GET_KEY_D:
                        key = KeyCode.D;
                        break;
                    case EventHandle.ET_GET_KEY_DOWN_B:
                    case EventHandle.ET_GET_KEY_UP_B:
                    case EventHandle.ET_GET_KEY_B:
                        key = KeyCode.B;
                        break;
                    case EventHandle.ET_GET_KEY_DOWN_Q:
                    case EventHandle.ET_GET_KEY_UP_Q:
                    case EventHandle.ET_GET_KEY_Q:
                        key = KeyCode.Q;
                        break;
                    case EventHandle.ET_GET_KEY_DOWN_E:
                    case EventHandle.ET_GET_KEY_UP_E:
                    case EventHandle.ET_GET_KEY_E:
                        key = KeyCode.E;
                        break;
                    case EventHandle.ET_GET_KEY_DOWN_TAB:
                    case EventHandle.ET_GET_KEY_UP_TAB:
                    case EventHandle.ET_GET_KEY_TAB:
                        key = KeyCode.Tab;
                        break;
                    case EventHandle.ET_GET_KEY_DOWN_ESC:
                    case EventHandle.ET_GET_KEY_UP_ESC:
                    case EventHandle.ET_GET_KEY_ESC:
                        key = KeyCode.Escape;
                        break;
                    case EventHandle.ET_GET_KEY_DOWN_SPACE:
                    case EventHandle.ET_GET_KEY_UP_SPACE:
                    case EventHandle.ET_GET_KEY_SPACE:
                        key = KeyCode.Space;
                        break;
                }

                if (key != KeyCode.None)
                {
                    data.AddParam(key);
                }
                _inputEventDic.Add(type, data);
                EventCentre.Instance.SurveilEvent(EventCentre.Instance.onInputEventHandle, type);
            }
        }

        public void Execute()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_DOWN_W]);
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_UP_W]);
            }

            if (Input.GetKey(KeyCode.W))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_W]);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_DOWN_A]);
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_UP_A]);
            }

            if (Input.GetKey(KeyCode.A))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_A]);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_DOWN_S]);
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_UP_S]);
            }

            if (Input.GetKey(KeyCode.S))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_S]);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_DOWN_D]);
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_UP_D]);
            }

            if (Input.GetKey(KeyCode.D))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_D]);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_DOWN_B]);
            }

            if (Input.GetKeyUp(KeyCode.B))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_UP_B]);
            }

            if (Input.GetKey(KeyCode.B))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_B]);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_DOWN_Q]);
            }

            if (Input.GetKeyUp(KeyCode.Q))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_UP_Q]);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_Q]);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_DOWN_E]);
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_UP_E]);
            }

            if (Input.GetKey(KeyCode.E))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_E]);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_DOWN_TAB]);
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_UP_TAB]);
            }

            if (Input.GetKey(KeyCode.Tab))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_TAB]);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_DOWN_ESC]);
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_UP_ESC]);
            }

            if (Input.GetKey(KeyCode.Escape))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_ESC]);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_DOWN_SPACE]);
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_UP_SPACE]);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                EventCentre.Instance.DispatchEvent(_inputEventDic[EventHandle.ET_GET_KEY_SPACE]);
            }
        }
    }
}
