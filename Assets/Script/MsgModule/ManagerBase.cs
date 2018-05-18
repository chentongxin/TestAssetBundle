using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNode
{
    //当前数据
    public MonoBase m_data;

    //下一个节点
    public EventNode m_next;

    public EventNode(MonoBase tmpMono)
    {
        m_data = tmpMono;
        m_next = null;
    }
}

public class ManagerBase : MonoBase
{
    //存储注册的消息key value
    public Dictionary<ushort, EventNode> m_eventTree = new Dictionary<ushort, EventNode>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="要注册的脚本"></param>
    /// <param name="msgs">脚本可以注册多个msg</param>
    public void RegisterMsg(MonoBase mono, params ushort[] msgs)
    {
        for(int i=0; i<msgs.Length; i++)
        {
            EventNode tmp = new EventNode(mono);
            RegisterMsg(msgs[i], tmp);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">消息id</param>
    /// <param name="node">node链表</param>
    public void RegisterMsg(ushort id, EventNode node)
    {
        if(!m_eventTree.ContainsKey(id))
        {
            m_eventTree.Add(id, node);
        }
        else
        {
            EventNode tmpNode = m_eventTree[id];
            while(null != tmpNode.m_next)
            {
                tmpNode = tmpNode.m_next;
            }
            tmpNode.m_next = node;
        }
    }

    public void UnRegisterMsg(MonoBase mono, params ushort[] msgs)
    {
        for (int i = 0; i < msgs.Length; i++)
        {
            UnRegisterMsg(msgs[i], mono);
        }
    }

    //去掉一个消息链的mono
    public void UnRegisterMsg(ushort id, MonoBase mono)
    {
        if(m_eventTree.ContainsKey(id))
        {
            EventNode tmp = m_eventTree[id];
            //去掉头部
            if(tmp.m_data == mono)
            {
                EventNode header = tmp;
                if(null != header.m_next)
                {
                    m_eventTree[id] = tmp.m_next;
                    header.m_next = null;
                    //header.m_data = tmp.m_next.m_data;
                    //header.m_next = tmp.m_next.m_next;
                }
                else
                {
                    m_eventTree.Remove(id);
                }
            }
            else
            {
                while(tmp.m_next!=null && tmp.m_next.m_data != mono)
                {
                    tmp = tmp.m_next;
                }

                if(tmp.m_next.m_next != null)
                {
                    EventNode curNode = tmp.m_next;
                    tmp.m_next = curNode.m_next;
                    curNode.m_next = null;
                   // tmp.m_next = tmp.m_next.m_next;
                }
                else
                {
                    tmp.m_next = null;
                }
            }
        }
        else
        {
            Debug.LogWarning("not contain id ==" + id);
        }
    }

    //来了消息处理消息
    public override void ProcessEvent(MsgBase tmpMsg)
    {
        if(!m_eventTree.ContainsKey(tmpMsg.m_msgId))
        {
            Debug.LogError("msg " + tmpMsg.m_msgId + " not contail ");
            Debug.LogError("msg Manager " + tmpMsg.getManager());
        }
        else
        {
            EventNode tmp = m_eventTree[tmpMsg.m_msgId];
            do
            {
                tmp.m_data.ProcessEvent(tmpMsg);
                tmp = tmp.m_next;
            } while (tmp != null);
        }
    }
}
