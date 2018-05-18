using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IABResLoader : IDisposable{

    private AssetBundle ABRes;

    public IABResLoader(AssetBundle tmpBundle)
    {
        ABRes = tmpBundle;
    }

    //加载单个资源
    public UnityEngine.Object this[string resName]
    {
        get {
            if(this.ABRes == null || !this.ABRes.Contains(resName))
            {
                Debug.Log("res not contain");
                return null;
            }
            return ABRes.LoadAsset(resName);
        }
    }

    //加载多个资源
    public UnityEngine.Object[] LoadResources(string resName)
    {
        if (this.ABRes == null || !this.ABRes.Contains(resName))
        {
            Debug.Log("res not contain");
            return null;
        }
        return this.ABRes.LoadAssetWithSubAssets(resName);
    }

    //卸载单个资源
    public  void UnloadRes(UnityEngine.Object resObj)
    {
        Resources.UnloadAsset(resObj);
    }

    //释放assetbundle包
    public void Dispose()
    {
        if (this.ABRes == null)
        {
            return;
        }
        ABRes.Unload(false);
    }

    //调试需要的
    public void DebugAllRes()
    {
        string[] tmpAssetName = ABRes.GetAllAssetNames();
        for(int i=0; i<tmpAssetName.Length; i++)
        {
            Debug.Log("ABRes contain asset name == " + tmpAssetName[i]);
        }
    }
}