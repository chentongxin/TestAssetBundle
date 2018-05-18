using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AssetEvent
{
    HunkRes = ManagerID.AssetManager + 1,

    ReleaseSingleObj,
    ReleaseBundleObj,
    ReleaseSceneObj,

    ReleaseSingleBundle,
    ReleaseSceneBundle,
    ReleaseAll,
}

public class HunkAssetRes : MsgBase
{
    public string sceneName;
    public string bundleName;
    public string resName;
    public ushort backMsgId;
    public bool isSingle;

    public HunkAssetRes(bool tmpSingle, ushort msgid, string tmpSceneName,string tmpBundleName, string tmpResName, ushort tmpBackMsgId)
    {
        this.isSingle = tmpSingle;
        this.m_msgId = msgid;
        this.sceneName = tmpSceneName;
        this.bundleName = tmpBundleName;
        this.resName = tmpResName;
        this.backMsgId = tmpBackMsgId;
    }
}


public class HunkAssetResBack:MsgBase
{
    public Object[] value;
    public HunkAssetResBack()
    {
        this.m_msgId = 0;
        this.value = null;
    }

    public void Changer(ushort msgid, params Object[] tmpValue)
    {
        this.m_msgId = msgid;
        this.value = tmpValue;
    }

    public void Changer(ushort msgid)
    {
        this.m_msgId = msgid;
    }

    public void Changer(params Object[] tmpValue)
    {
        this.value = tmpValue;
    }
}