
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgCenter : MonoBehaviour
{
    public static MsgCenter Instance = null;

    private void Awake()
    {
        Instance = this;
        gameObject.AddComponent<UIManager>();
        gameObject.AddComponent<NPCManager>();
        gameObject.AddComponent<AssetManager>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
    public void SendToMsg(MsgBase tmpMsg)
    {
        AnasysisMsg(tmpMsg);
    }

    private void AnasysisMsg(MsgBase tmpMsg)
    {
        ManagerID tmpId = tmpMsg.getManager();
        switch(tmpId)
        {
            case ManagerID.AssetManager:
                AssetManager.Instance.ProcessEvent(tmpMsg);
                break;
            case ManagerID.AudioManager:
                break;
            case ManagerID.CharactorManager:
                break;
            case ManagerID.GameManager:
                break;
            case ManagerID.NetManager:
                break;
            case ManagerID.NPCManager:
                break;
            case ManagerID.UIManager:
                UIManager.Instance.ProcessEvent(tmpMsg);
                break;
           default:
                break;
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
