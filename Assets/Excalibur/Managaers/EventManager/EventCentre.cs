namespace Excalibur
{
    public sealed class EventCentre : Singleton<EventCentre>
    {
        bool USE_MONO_EVENT;

        protected override void Init()
        {
            base.Init();

            USE_MONO_EVENT = MonoEventManager.Initialized();
        }

        public void SurveilEvent(EventHandler handler, EventHandle eventType = EventHandle.ET_NONE)
        {
            if (USE_MONO_EVENT)
            {
                MonoEventManager.Instance.SurveilEvent(handler);
            }
            else
            {
                EventManager.Instance.SurveilEvent(eventType, handler);
            }
        }

        public void DispatchEvent(EventParam eventData)
        {
            if (USE_MONO_EVENT)
            {
                MonoEventManager.Instance.HandleEvent(eventData);
            }
            else
            {
                EventManager.Instance.HandleEvent(eventData);
            }
        }

        public void onInputEventHandle(EventParam eventData)
        {
            if (eventData.EventType <= EventHandle.KEY_CODE_BEGIN || eventData.EventType >= EventHandle.KEY_CODE_END)
                return;

            //Log.General(eventData.Params[0].ToString());

            switch (eventData.EventType)
            {
                case EventHandle.ET_GET_KEY_DOWN_W:
                    FormManager.Instance.CreateForm(FormType.FT_EXCAMPLE);
                    break;
                case EventHandle.ET_GET_KEY_UP_W:
                    break;
                case EventHandle.ET_GET_KEY_W:
                    break;
                case EventHandle.ET_GET_KEY_DOWN_A:
                    EventParam data = new EventParam(EventHandle.ET_FORM_EXCAMPLE);
                    DispatchEvent(data);
                    break;
                case EventHandle.ET_GET_KEY_UP_A:
                    break;
                case EventHandle.ET_GET_KEY_A:
                    break;
                case EventHandle.ET_GET_KEY_DOWN_S:
                    break;
                case EventHandle.ET_GET_KEY_UP_S:
                    break;
                case EventHandle.ET_GET_KEY_S:
                    break;
                case EventHandle.ET_GET_KEY_DOWN_D:
                    break;
                case EventHandle.ET_GET_KEY_UP_D:
                    break;
                case EventHandle.ET_GET_KEY_D:
                    break;
                case EventHandle.ET_GET_KEY_DOWN_B:
                    FormManager.Instance.ForceActivateFormInOut(FormType.FT_EXCAMPLE);
                    break;
                case EventHandle.ET_GET_KEY_UP_B:
                    break;
                case EventHandle.ET_GET_KEY_B:
                    break;
                case EventHandle.ET_GET_KEY_DOWN_Q:
                    break;
                case EventHandle.ET_GET_KEY_UP_Q:
                    break;
                case EventHandle.ET_GET_KEY_Q:
                    break;
                case EventHandle.ET_GET_KEY_DOWN_E:
                    break;
                case EventHandle.ET_GET_KEY_UP_E:
                    break;
                case EventHandle.ET_GET_KEY_E:
                    break;
                case EventHandle.ET_GET_KEY_DOWN_TAB:
                    break;
                case EventHandle.ET_GET_KEY_UP_TAB:
                    break;
                case EventHandle.ET_GET_KEY_TAB:
                    break;
                case EventHandle.ET_GET_KEY_DOWN_ESC:
                    break;
                case EventHandle.ET_GET_KEY_UP_ESC:
                    break;
                case EventHandle.ET_GET_KEY_ESC:
                    break;
                case EventHandle.ET_GET_KEY_DOWN_SPACE:
                    break;
                case EventHandle.ET_GET_KEY_UP_SPACE:
                    break;
                case EventHandle.ET_GET_KEY_SPACE:
                    break;
            }
        }
    }
}