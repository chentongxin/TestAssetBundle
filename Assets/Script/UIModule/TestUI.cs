using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : UIBase {
    GameObject obj = null;
    public override void ProcessEvent(MsgBase tmpMsg)
    {
        //base.ProcessEvent(tmpMsg);

        switch (tmpMsg.m_msgId)
        {
            case (ushort)UIEvent.HunkResBack:
                {
                    HunkAssetResBack recMsg = (HunkAssetResBack)tmpMsg;
                    obj = Instantiate(recMsg.value[0]) as GameObject;
                }
                break;

        }
    }
    private void Awake()
    {
        msgIds = new ushort[] {
            (ushort)UIEvent.HunkResBack,
        };

        RegisterSelf(this, msgIds);
    }

    // Use this for initialization
    void Start () {
        HunkAssetRes res = new HunkAssetRes(true, (ushort)AssetEvent.HunkRes, "SceneOne", "Load", "test", (ushort)UIEvent.HunkResBack);
        SendMsg(res);

    }
	
	// Update is called once per frame
	void Update () {
        if (obj != null)
        {
            obj.transform.Rotate(Vector3.up * 10);
        }
	}
}
