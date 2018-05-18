using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void NativeResCallBack(NativeResCallBackNode tmpNode);

public class NativeResCallBackNode
{
    public string sceneName;
    public string bundleName;
    public string resName;
    public ushort backMsgId;
    public bool isSingle;

    public NativeResCallBackNode next;

    public NativeResCallBack callBack;

    public NativeResCallBackNode(bool tmpSingle, string tmpSceneName, string tmpBundleName, string tmpResName, ushort tmpBackMsgId, NativeResCallBack tmpCallBack, NativeResCallBackNode tmpNode)
    {
        this.isSingle = tmpSingle;
        this.sceneName = tmpSceneName;
        this.bundleName = tmpBundleName;
        this.resName = tmpResName;
        this.backMsgId = tmpBackMsgId;
        this.callBack = tmpCallBack;
        this.next = tmpNode;
    }

    public void Dispose()
    {
        next = null;
        callBack = null;
        this.sceneName = null;
        this.bundleName = null;
        this.resName = null;
    }
}

public class NativeResCallBackManager
{
    Dictionary<string, NativeResCallBackNode> manager = null;

    public NativeResCallBackManager()
    {
        manager = new Dictionary<string, NativeResCallBackNode>();
    }

    public void AddBundle(string bundle, NativeResCallBackNode curNode)
    {
        if(manager.ContainsKey(bundle))
        {
            NativeResCallBackNode tmpNode = manager[bundle];
            while(tmpNode.next != null)
            {
                tmpNode = tmpNode.next;
            }
            tmpNode.next = curNode;
        }
        else
        {
            manager.Add(bundle, curNode);
        }
    }

    public void Dospose(string bundle)
    {
        if (manager.ContainsKey(bundle))
        {
            NativeResCallBackNode tmpNode = manager[bundle];
            while (tmpNode.next != null)
            {
                NativeResCallBackNode curNode = tmpNode;
                tmpNode = tmpNode.next;
                curNode.Dispose();
                curNode = null;
            }
            tmpNode.Dispose();
            tmpNode = null;
            manager.Remove(bundle);
        }
    }

    public void CallBackRes(string bundle)
    {
        if(manager.ContainsKey(bundle))
        {
            NativeResCallBackNode tmpNode = manager[bundle];
            do
            {
                tmpNode.callBack(tmpNode);
                tmpNode = tmpNode.next;
            } while (tmpNode != null);
        }
    }
}

public class NativeResLoader : AssetBase {
    public override void ProcessEvent(MsgBase tmpMsg)
    {
        //base.ProcessEvent(tmpMsg);

        switch(tmpMsg.m_msgId)
        {
            case (ushort)AssetEvent.ReleaseSingleObj:
                {
                    HunkAssetRes recMsg = (HunkAssetRes)tmpMsg;
                    ILoadManager.Instance.UnloadResObj(recMsg.sceneName, recMsg.bundleName, recMsg.resName);
                }
                
                break;
            case (ushort)AssetEvent.ReleaseBundleObj:
                {
                    HunkAssetRes recMsg = (HunkAssetRes)tmpMsg;
                    ILoadManager.Instance.UnloadBundleResObjs(recMsg.sceneName, recMsg.bundleName);
                }
                break;
            case (ushort)AssetEvent.ReleaseSceneObj:
                {
                    HunkAssetRes recMsg = (HunkAssetRes)tmpMsg;
                    ILoadManager.Instance.UnloadAllResObjs(recMsg.sceneName);
                }
                break;
            case (ushort)AssetEvent.ReleaseSingleBundle:
                {
                    HunkAssetRes recMsg = (HunkAssetRes)tmpMsg;
                    ILoadManager.Instance.UnloadAssetBundle(recMsg.sceneName, recMsg.bundleName);
                }
                break;
            case (ushort)AssetEvent.ReleaseSceneBundle:
                {
                    HunkAssetRes recMsg = (HunkAssetRes)tmpMsg;
                    ILoadManager.Instance.UnloadAllAssetBundle(recMsg.sceneName);
                }
                break;
            case (ushort)AssetEvent.ReleaseAll:
                {
                    HunkAssetRes recMsg = (HunkAssetRes)tmpMsg;
                    ILoadManager.Instance.UnloadAllAssetBundleAndResObjs(recMsg.sceneName);
                }
                break;
            case (ushort)AssetEvent.HunkRes:
                {
                    HunkAssetRes recMsg = (HunkAssetRes)tmpMsg;
                    GetResources(recMsg.sceneName, recMsg.bundleName, recMsg.resName, recMsg.isSingle, recMsg.backMsgId);
                }
                break;

        }
    }

    HunkAssetResBack resBackMsg = null;

    HunkAssetResBack ReleaseBack
    {
        get
        {
            if(resBackMsg == null)
            {
                resBackMsg = new HunkAssetResBack();
            }
            return resBackMsg;
        }
    }
    NativeResCallBackManager callBack = null;
    NativeResCallBackManager CallBack
    {
        get
        {
            if(callBack == null)
            {
                callBack = new NativeResCallBackManager();
            }
            return callBack;
        }
    }
    private void Awake()
    {
        msgIds = new ushort[] {
            (ushort)AssetEvent.ReleaseSingleObj,
            (ushort)AssetEvent.ReleaseBundleObj,
            (ushort)AssetEvent.ReleaseSceneObj,

            (ushort)AssetEvent.ReleaseSingleBundle,
            (ushort)AssetEvent.ReleaseSceneBundle,
            (ushort)AssetEvent.ReleaseAll,
            (ushort)AssetEvent.HunkRes,
        };

        RegisterSelf(this, msgIds);
        gameObject.AddComponent<ILoadManager>();
    }

    public void SendToBackMsg(NativeResCallBackNode tmpNode)
    {
        if(tmpNode.isSingle)
        {
            Object tmpObj = ILoadManager.Instance.GetSingleResource(tmpNode.sceneName, tmpNode.bundleName, tmpNode.resName);
            this.ReleaseBack.Changer(tmpNode.backMsgId, tmpObj);
            SendMsg(ReleaseBack);
        }
        else
        {
            Object[] tmpObj = ILoadManager.Instance.GetMultiResource(tmpNode.sceneName, tmpNode.bundleName, tmpNode.resName);
            this.ReleaseBack.Changer(tmpNode.backMsgId, tmpObj);
            SendMsg(ReleaseBack);
        }
    }

    void LoadProgress(string bundleName, float progress)
    {
        if(progress >= 1.0f)
        {
            CallBack.CallBackRes(bundleName);
            CallBack.Dospose(bundleName);
        }
    }

    public void GetResources(string sceneName, string bundleName, string resName, bool isSingle, ushort backId)
    {
        //没有加载
        if(!ILoadManager.Instance.IsLoadingAssetBundle(sceneName, bundleName))
        {
            ILoadManager.Instance.LoadAsset(sceneName, bundleName, LoadProgress);
            string bundleFullName = ILoadManager.Instance.GetBundleRelateName(sceneName, bundleName);
            if(bundleFullName != null)
            {
                NativeResCallBackNode tmpNode = new NativeResCallBackNode(isSingle, sceneName, bundleName, resName, backId, SendToBackMsg, null);
                CallBack.AddBundle(bundleFullName, tmpNode);

            }
            else
            {
                Debug.Log("Do not contain bundle==" + bundleName);
            }
        }
        else if(ILoadManager.Instance.IsLoadingBundleFinish(sceneName,bundleName))
        {
            if(isSingle)
            {
                Object tmpObj = ILoadManager.Instance.GetSingleResource(sceneName, bundleName, resName);
                this.ReleaseBack.Changer(backId, tmpObj);
                SendMsg(ReleaseBack);
            }
            else
            {
                Object[] tmpObj = ILoadManager.Instance.GetMultiResource(sceneName, bundleName, resName);
                this.ReleaseBack.Changer(backId, tmpObj);
                SendMsg(ReleaseBack);
            }
        }
        else
        {
            //已经加载但是没有完成
            string bundleFullName = ILoadManager.Instance.GetBundleRelateName(sceneName, bundleName);
            if (bundleFullName != null)
            {
                NativeResCallBackNode tmpNode = new NativeResCallBackNode(isSingle, sceneName, bundleName, resName, backId, SendToBackMsg, null);
                CallBack.AddBundle(bundleFullName, tmpNode);
            }
            else
            {
                Debug.Log("Do not contain bundle==" + bundleName);
            }
        }
    }
}
