using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void LoaderProgress(string bundleName, float progress);
public delegate void LoadFinish(string bundle);
public class IABLoader
{
    private string bundleName;

    private string commonBundlePath = "";

    private WWW commonLoader;

    private float commResLoaderProgress;
    
    //每一帧回调
    private LoaderProgress loadProgress;
    //load完成回调
    private LoadFinish loadFinish;

    private IABResLoader abResLoader;

    public IABLoader(LoaderProgress tmpLoaderProgress, LoadFinish tmpLoadFinish)
    {
        bundleName = "";
        commonBundlePath = "";
        commResLoaderProgress = 0;
        loadProgress = tmpLoaderProgress;
        loadFinish = tmpLoadFinish;
        abResLoader = null;
    }

    //设置包名
    public void SetBundleName(string bundle)
    {
        this.bundleName = bundle;
    }

    //要求上层传递完整路径
    public void LoadResources(string path)
    {
        commonBundlePath = path;
    }

    //协程加载
    public IEnumerator CommonLoad()
    {
        commonLoader = new WWW(commonBundlePath);
        while(!commonLoader.isDone)
        {
            commResLoaderProgress = commonLoader.progress;
            if(loadProgress != null)
            {
                loadProgress(bundleName, commResLoaderProgress);
            }
            
            yield return commonLoader.progress;
            commResLoaderProgress = commonLoader.progress;
        }
        //加载完成
        if(commResLoaderProgress >= 1.0)
        {
            abResLoader = new IABResLoader(commonLoader.assetBundle);
            if (loadProgress != null)
            {
                loadProgress(bundleName, commResLoaderProgress);
            }
            if (loadFinish != null)
            {
                loadFinish(bundleName);
            }
        }
        else
        {
            Debug.LogError("load bundle error == " + bundleName);
        }
        commonLoader = null;
    }

    #region 下层提供功能

    //Debug
    public void DebugLoader()
    {
        if (abResLoader != null)
        {
            abResLoader.DebugAllRes();
        }
    }

    //获取单个资源
    public UnityEngine.Object GetResources(string resName)
    {
        if (abResLoader != null)
        {
            return abResLoader[resName];
        }
        return null;
    }

    //获取多个资源
    public UnityEngine.Object[] GetMultiResources(string resName)
    {
        if (abResLoader != null)
        {
            return abResLoader.LoadResources(resName);
        }
        return null;
    }

    //卸载单个资源
    public void UnloadAssetRes(UnityEngine.Object resObj)
    {
        if (abResLoader != null)
            abResLoader.UnloadRes(resObj);
    }

    //释放功能
    public void Dispose()
    {
        if (abResLoader != null)
        {
            abResLoader.Dispose();
            abResLoader = null;
        }
    }

        #endregion
    }
