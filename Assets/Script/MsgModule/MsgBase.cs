using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgBase  {
    //标识65535个消息 占用两个字节
    public ushort m_msgId;

    public ManagerID getManager()
    {
        int tempId = m_msgId / FrameTools.m_msgSpan;
        return (ManagerID)(tempId * FrameTools.m_msgSpan);
    }

    public MsgBase()
    {
        m_msgId = 0;
    }

    public MsgBase(ushort tmpMsg)
    {
        m_msgId = tmpMsg;
    }
}
