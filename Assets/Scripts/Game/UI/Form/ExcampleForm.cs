using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Excalibur;

public class ExcampleForm : Form
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnOpen()
    {
    }

    protected override void OnClose()
    {
    }

    protected override void onEventHandler(EventParam eventData)
    {
        switch (eventData.EventType)
        {
            case EventHandle.ET_FORM_EXCAMPLE:
                Log.General("Ê¾Àý´°¿Ú£¡");
                break;
        }
    }

    protected override void OnReset()
    {
        throw new System.NotImplementedException();
    }
}
