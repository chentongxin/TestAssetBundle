using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBase : MonoBase
{
    public void RegisterSelf(MonoBase mono, params ushort[] msgs)
    {
        AssetManager.Instance.RegisterMsg(mono, msgs);
    }

    public void UnRegisterSelf(MonoBase mono, params ushort[] msgs)
    {
        AssetManager.Instance.UnRegisterMsg(mono, msgs);
    }

    public void SendMsg(MsgBase msg)
    {
        AssetManager.Instance.SendMsg(msg);
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
