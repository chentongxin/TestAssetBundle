using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ILoadManager : MonoBehaviour
{
    public static ILoadManager Instance;

    void Awake()
    {
        Instance = this;
        //1、加载IABManifest
        StartCoroutine(IABManifestLoader.Instance.LoadManifest());


    }

    IABSenceManager sceneManager;

    //sceneName manager
    private Dictionary<string, IABSenceManager> loadManager = new Dictionary<string, IABSenceManager>();
    //2、读取配置文件
    public void ReadConfiger(string scenceName)
    {
        if(!loadManager.ContainsKey(scenceName))
        {
            IABSenceManager tmpManager = new IABSenceManager(scenceName);
            tmpManager.ReadConfiger(scenceName);
            loadManager.Add(scenceName, tmpManager);
        }
    }

    public void LoadCallBack(string sceneName, string bundleName)
    {
        if(loadManager.ContainsKey(sceneName))
        {
            IABSenceManager tmpManager = loadManager[sceneName];
            StartCoroutine(tmpManager.LoadAssetSys(bundleName));
        }
        else
        {
            Debug.Log("bundle name is not contain == " + bundleName);
        }
    }

    //提供加载功能
    public void LoadAsset(string sceneName, string bundleName, LoaderProgress progress)
    {
        if(!loadManager.ContainsKey(sceneName))
        {
            ReadConfiger(sceneName);
        }
        IABSenceManager tmpManager = loadManager[sceneName];
        tmpManager.LoadAsset(bundleName, progress, LoadCallBack);
    }

    #region 下层功能
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sceneName">SceneOne</param>
    /// <param name="bundleName">Load</param>
    /// <param name="resName">TestTwo</param>
    /// <returns></returns>
    public Object GetSingleResource(string sceneName, string bundleName, string resName)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSenceManager tmpManager = loadManager[sceneName];
            return tmpManager.GetSingleResources(bundleName, resName);
        }
        else
        {
            Debug.Log("SceneName == " + sceneName + "Bundle Name ==" + bundleName + " is not load");
        }
        return null;
    }

    public Object[] GetMultiResource(string sceneName, string bundleName, string resName)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSenceManager tmpManager = loadManager[sceneName];
            return tmpManager.GetMultiResources(bundleName, resName);
        }
        else
        {
            Debug.Log("SceneName == " + sceneName + "Bundle Name ==" + bundleName + " is not load");
        }
        return null;
    }

    /// <summary>
    /// 释放某一个资源
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="bundleName"></param>
    /// <param name="resName"></param>
    public void UnloadResObj(string sceneName, string bundleName, string resName)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSenceManager tmpManager = loadManager[sceneName];
            tmpManager.DisposeResObj(bundleName, resName);
        }
    }

    //释放整个包
    public void UnloadBundleResObjs(string sceneName, string bundleName)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSenceManager tmpManager = loadManager[sceneName];
            tmpManager.DisposeBundleRes(bundleName);
        }
    }

    //释放整个场景的obj
    public void UnloadAllResObjs(string sceneName)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSenceManager tmpManager = loadManager[sceneName];
            tmpManager.DisposeAllRes();
        }
    }

    //释放一个bundle
    public void UnloadAssetBundle(string sceneName, string bundleName)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSenceManager tmpManager = loadManager[sceneName];
            tmpManager.DisposeBundle(bundleName);
        }
    }

    //释放一个场景的全部bundle
    public void UnloadAllAssetBundle(string sceneName)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSenceManager tmpManager = loadManager[sceneName];
            tmpManager.DisposeAllBundle();

            System.GC.Collect();
        }
    }

    //释放一个场景的全部bundle和obj
    public void UnloadAllAssetBundleAndResObjs(string sceneName)
    {
        if (loadManager.ContainsKey(sceneName))
        {
            IABSenceManager tmpManager = loadManager[sceneName];
            tmpManager.DisposeAllBundleAndRes();

            System.GC.Collect();
        }

    }

    public void DebugAllAssetBundle(string sceneName)
    {
        if(loadManager.ContainsKey(sceneName))
        {
            IABSenceManager tmpManager = loadManager[sceneName];
            tmpManager.DebugAllAsset();
        }
    }
    #endregion


    public bool IsLoadingBundleFinish(string sceneName,string bundleName)
    {
        bool tmpBool = loadManager.ContainsKey(sceneName);

        if(tmpBool)
        {
            IABSenceManager tmpManager = loadManager[sceneName];
            return tmpManager.IsLoadingFinish(bundleName);
        }
        return false;
    }

    public bool IsLoadingAssetBundle(string sceneName,string bundleName)
    {
        bool tmpBool = loadManager.ContainsKey(sceneName);

        if (tmpBool)
        {
            IABSenceManager tmpManager = loadManager[sceneName];
            return tmpManager.IsLoadingAssetBundle(bundleName);
        }
        return false;
    }

    public string GetBundleRelateName(string sceneName, string bundleName)
    {
        IABSenceManager tmpManager = loadManager[sceneName];
        if(tmpManager != null)
        {
            return tmpManager.GetBundleRelateName(bundleName);
        }
        return null;
    }

    void OnDestroy()
    {
        loadManager.Clear();
        System.GC.Collect();
    }
}
