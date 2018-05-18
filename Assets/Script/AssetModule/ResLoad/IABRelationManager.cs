using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IABRelationManager
{
    IABLoader assetLoader;
    string bundleName;
    //依赖关系
    List<string> dependenceBundle = null;

    //被依赖的关系
    List<string> referBundle = null;

    LoaderProgress loaderProcess;

    public IABRelationManager()
    {
        dependenceBundle = new List<string>();
        referBundle = new List<string>();
    }

    //添加被依赖关系
    public void AddRefference(string bundleName)
    {
        referBundle.Add(bundleName);
    }
    //获取被依赖关系
    public List<string> GetRefference()
    {
        return referBundle;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns>是否释放了自己</returns>
    public bool RemoveReference(string bundleName)
    {
        for(int i=0; i<referBundle.Count; i++)
        {
            if(bundleName.Equals(referBundle[i]))
            {
                referBundle.RemoveAt(i);
            }
        }

        if(referBundle.Count<0)
        {
            Dispose();
            return true;
        }
        return false;
    }

    public void SetDependence(string[] depence)
    {
        if(depence.Length > 0)
        {
            dependenceBundle.AddRange(depence);
        }
    }

    public List<string> GetDependence()
    {
        return dependenceBundle;
    }

    public void RemoveDependence(string bundleName)
    {
        for(int i=0; i<dependenceBundle.Count; i++)
        {
            if(bundleName.Equals(dependenceBundle[i]))
            {
                dependenceBundle.RemoveAt(i);
            }
        }
    }

    bool IsLoadFinish;
    public void BundleLoadFinish(string bundleName)
    {
        IsLoadFinish = true;
    }

    public bool IsBundleLoadFinish()
    {
        return IsLoadFinish;
    }

    public string getBundleName()
    {
        return bundleName;
    }

    public LoaderProgress GetProgress()
    {
        return loaderProcess;
    }

    public void Initial(string bundle, LoaderProgress progress)
    {
        bundleName = bundle;
        IsLoadFinish = false;
        loaderProcess = progress;
        assetLoader = new IABLoader(progress, BundleLoadFinish);
        assetLoader.SetBundleName(bundle);
        string bundlePath = IPathTools.GetWWWAssetBundlePath() + "/" + bundle;
        assetLoader.LoadResources(bundlePath);
    }

    #region 由下层提供API
    public void DebuggerAsset()
    {
        if(assetLoader != null)
        {
            assetLoader.DebugLoader();
        }
        else
        {
            Debug.Log("asset load is null");
        }
    }

    public IEnumerator LoadAssetBundle()
    {
        yield return assetLoader.CommonLoad();
    }


    //释放的过程
    public void Dispose()
    {
        assetLoader.Dispose();
    }

    public UnityEngine.Object GetSingleResource(string bundleName)
    {
        return assetLoader.GetResources(bundleName);
    }

    public UnityEngine.Object[] GetMultiResource(string bundleName)
    {
        return assetLoader.GetMultiResources(bundleName);
    }

    #endregion
}
