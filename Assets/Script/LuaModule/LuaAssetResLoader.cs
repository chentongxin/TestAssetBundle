using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;

public class LuaResCallBackNode
{
    public string resName;
    public string bundleName;
    public string sceneName;

    public bool isSingle;
    public LuaResCallBackNode next;

    public LuaFunction func;
    public LuaResCallBackNode(string tmpRes, string tmpBundle, string tmpScene, LuaFunction tmpFunc, bool tmpIsSingle, LuaResCallBackNode tmpNode)
    {
        this.resName = tmpRes;
        this.bundleName = tmpBundle;
        this.sceneName = tmpScene;
        this.func = tmpFunc;
        this.isSingle = tmpIsSingle;
        this.next = tmpNode;
    }

    public void Dispose()
    {
        this.resName = null;
        this.bundleName = null;
        this.sceneName = null;
        this.func.Dispose();
        this.next = null;
    }
}

public class LuaResCallBackManager
{
    Dictionary<string, LuaResCallBackNode> manager = null;
    public LuaResCallBackManager()
    {
        manager = new Dictionary<string, LuaResCallBackNode>();
    }

    public void  AddBundleCallBack(string bundle, LuaResCallBackNode tmpNode )
    {
        if(manager.ContainsKey(bundle))
        {
            LuaResCallBackNode TopNode = manager[bundle];
            while(TopNode.next != null)
            {
                TopNode = TopNode.next;
            }
            TopNode.next = tmpNode;
        }
        else
        {
            manager.Add(bundle, tmpNode);
        }
    }

    public void Dispose(string bundle)
    {
        if(manager.ContainsKey(bundle))
        {
            LuaResCallBackNode topNode = manager[bundle];
            while(topNode.next!=null)
            {
                LuaResCallBackNode curNode = topNode;
                topNode = topNode.next;
                curNode.Dispose();
            }
            topNode.Dispose();
            manager.Remove(bundle);
        }
    }

    public void CallBackLua(string bundle)
    {
        if(manager.ContainsKey(bundle))
        {
            LuaResCallBackNode topNode = manager[bundle];
            do
            {
                if(topNode.isSingle)
                {
                    object tmpObj = ILoadManager.Instance.GetSingleResource(topNode.sceneName, topNode.bundleName, topNode.resName);
                    topNode.func.Call(topNode.sceneName, topNode.bundleName, topNode.resName, tmpObj);
                }
                else
                {
                    object[] tmpObj = ILoadManager.Instance.GetMultiResource(topNode.sceneName, topNode.bundleName, topNode.resName);
                    topNode.func.Call(topNode.sceneName, topNode.bundleName, topNode.resName, tmpObj);
                }
                topNode = topNode.next;
            } while (topNode != null);
        }
    }
}

public class LuaLoadReses
{
    private static LuaLoadReses instance = null;

    public static LuaLoadReses Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new LuaLoadReses();
            }
            return instance;
        }
    }

    private LuaResCallBackManager luaResCallBackManager = null;


    public  LuaResCallBackManager CallBackManager
    {
        get
        {
            if (luaResCallBackManager == null)
            {
                luaResCallBackManager = new LuaResCallBackManager();
            }
            return luaResCallBackManager;
        }
    }

    void LoadProgress(string bundleName, float progress)
    {
        if (progress >= 1.0f)
        {
            CallBackManager.CallBackLua(bundleName);
            CallBackManager.Dispose(bundleName);
        }
    }

    public void GetResource(string sceneName,string bundleName,string res, bool single, LuaFunction luaFunc)
    {
        //没有加载
        if (!ILoadManager.Instance.IsLoadingAssetBundle(sceneName, bundleName))
        {
            ILoadManager.Instance.LoadAsset(sceneName, bundleName, LoadProgress);
            string bundleFullName = ILoadManager.Instance.GetBundleRelateName(sceneName, bundleName);
            if (bundleFullName != null)
            {
                LuaResCallBackNode tmpNode = new LuaResCallBackNode(res,bundleFullName,sceneName, luaFunc, single, null);
                CallBackManager.AddBundleCallBack(bundleFullName, tmpNode);

            }
            else
            {
                Debug.Log("Do not contain bundle==" + bundleName);
            }
        }
        else if (ILoadManager.Instance.IsLoadingBundleFinish(sceneName, bundleName))
        {
            if (single)
            {
                Object tmpObj = ILoadManager.Instance.GetSingleResource(sceneName, bundleName, res);
                //this.ReleaseBack.Changer(backId, tmpObj);
                //SendMsg(ReleaseBack);
                luaFunc.Call(sceneName, bundleName, res, tmpObj);
            }
            else
            {
                Object[] tmpObj = ILoadManager.Instance.GetMultiResource(sceneName, bundleName, res);
                //this.ReleaseBack.Changer(backId, tmpObj);
                //SendMsg(ReleaseBack);
                luaFunc.Call(sceneName, bundleName, res, tmpObj);
            }
        }
        else
        {
            //已经加载但是没有完成
            string bundleFullName = ILoadManager.Instance.GetBundleRelateName(sceneName, bundleName);
            if (bundleFullName != null)
            {
                LuaResCallBackNode tmpNode = new LuaResCallBackNode(res, bundleFullName, sceneName, luaFunc, single, null);
                CallBackManager.AddBundleCallBack(bundleFullName, tmpNode);
            }
            else
            {
                Debug.Log("Do not contain bundle==" + bundleName);
            }
        }
    }

    public void UnloadResObj(string sceneName, string bundleName, string resName)
    {
        ILoadManager.Instance.UnloadResObj(sceneName, bundleName, resName);
    }

    public void UnloadBundleObj(string sceneName, string bundleName)
    {
        ILoadManager.Instance.UnloadBundleResObjs(sceneName, bundleName);
    }

    public void UnloadSingleBundle(string sceneName, string bundleName)
    {
        ILoadManager.Instance.UnloadAssetBundle(sceneName, bundleName);
    }

    public void UnloadBundleAndObjes(string sceneName, string bundleName)
    {
        //ILoadManager.Instance(sceneName, bundleName);
    }
}
