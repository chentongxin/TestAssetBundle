using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : ManagerBase
{

    private Dictionary<string, GameObject> m_subMember = new Dictionary<string, GameObject>();

    public static NPCManager Instance = null;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendMsg(MsgBase msg)
    {
        //本模块处理
        if (msg.getManager() == ManagerID.NPCManager)
        {
            ProcessEvent(msg);
        }
        else //交给消息中心
        {
            MsgCenter.Instance.SendToMsg(msg);
        }
    }

    public void RegisterGameObject(string name, GameObject obj)
    {
        if (!m_subMember.ContainsKey(name))
        {
            m_subMember.Add(name, obj);
        }
    }

    public void UnRegisterGameObject(string name)
    {
        if (m_subMember.ContainsKey(name))
        {
            m_subMember.Remove(name);
        }
    }

    public GameObject GetGameObject(string name)
    {
        if (m_subMember.ContainsKey(name))
        {
            return m_subMember[name];
        }
        return null;
    }
}
