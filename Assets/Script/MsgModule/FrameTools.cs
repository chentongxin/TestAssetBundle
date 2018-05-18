using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//消息段
public enum ManagerID
{
    GameManager = 0,
    UIManager = FrameTools.m_msgSpan,
    AudioManager = FrameTools.m_msgSpan * 2,
    NPCManager = FrameTools.m_msgSpan * 3,
    CharactorManager = FrameTools.m_msgSpan * 4,
    AssetManager = FrameTools.m_msgSpan * 5,
    NetManager = FrameTools.m_msgSpan * 6,
}

public class FrameTools
{
    //每个manager管理的消息数
    public const int m_msgSpan = 3000;

}
