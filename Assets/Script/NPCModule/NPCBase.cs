using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBase : MonoBase
{

    public void RegisterSelf(MonoBase mono, params ushort[] msgs)
    {
        NPCManager.Instance.RegisterMsg(mono, msgs);
    }

    public void UnRegisterSelf(MonoBase mono, params ushort[] msgs)
    {
        NPCManager.Instance.UnRegisterMsg(mono, msgs);
    }

    public void SendMsg(MsgBase msg)
    {
        NPCManager.Instance.SendMsg(msg);
    }

    public ushort[] msgIds;

    private void OnDestroy()
    {
        if (msgIds != null)
        {
            UnRegisterSelf(this, msgIds);
        }
    }

    public override void ProcessEvent(MsgBase tmpMsg)
    {
        //throw new System.NotImplementedException();
    }
}
