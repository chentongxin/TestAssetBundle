using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void LoadAssetBundleCallBack(string scenceName, string bundleName);

//单个存取
public class AssetObj
{
    public List<Object> objs;

    public AssetObj(params Object[] tmpObj)
    {
        objs = new List<Object>();
        objs.AddRange(tmpObj);
    }

    public void ReleaseObj()
    {
        for(int i=0; i<objs.Count; i++)
        {
            Resources.UnloadAsset(objs[i]);
        }
    }
}

//一个bundle包里面的Obj
public class AssetResObj
{
    public Dictionary<string, AssetObj> resObjs;

    public AssetResObj(string name, AssetObj tmp)
    {
        resObjs = new Dictionary<string, AssetObj>();
        resObjs.Add(name, tmp);
    }

    public void AddResObj(string name, AssetObj tmpObj)
    {
        resObjs.Add(name, tmpObj);
    }

    public List<Object> GetResObj(string name)
    {
        if (resObjs.ContainsKey(name))
        {
            AssetObj tmpObj = resObjs[name];
            return tmpObj.objs;
        }
        else
        {
            Debug.Log("GetResObj name is not exit == " + name);
            return null;
        }
    }

    public void ReleaseAllResObj()
    {
        List<string> keys = new List<string>();
        keys.AddRange(resObjs.Keys);
        for(int i=0; i<keys.Count; i++)
        {
            ReleaseResObj(keys[i]);
        }
    }

    public void ReleaseResObj(string name)
    {
        if(resObjs.ContainsKey(name))
        {
            AssetObj tmpObj = resObjs[name];
            tmpObj.ReleaseObj();
        }
        else
        {
            Debug.Log("ReleaseResObj object name is not exit == " + name);
        }
    }
}

//对一个场景的所有bundle包的管理
public class IABManager
{
    //把每个包都存起来
    Dictionary<string, IABRelationManager> loadHelper = new Dictionary<string, IABRelationManager>();

    Dictionary<string, AssetResObj> loadObj = new Dictionary<string, AssetResObj>();

    string scenceName;

    public IABManager(string sceneName)
    {
        scenceName = sceneName;
    }

    //是否加载了bundleName
    public bool IsLoadingAssetBundle(string bundleName)
    {
        if (loadHelper.ContainsKey(bundleName))
        {
            return true;
        }
        return false;
    }

    public void DisposeResObj(string bundleName, string resName)
    {
        if(loadObj.ContainsKey(bundleName))
        {
            AssetResObj tmpObj = loadObj[bundleName];
            tmpObj.ReleaseResObj(resName);
        }
    }

    public void DisposeResObj(string bundleName)
    {
        if (loadObj.ContainsKey(bundleName))
        {
            AssetResObj tmpObj = loadObj[bundleName];
            tmpObj.ReleaseAllResObj();
        }
        Resources.UnloadUnusedAssets();
    }

    public void DisposAllObj()
    {
        List<string> keys = new List<string>();
        keys.AddRange(loadObj.Keys);

        for(int i=0; i<loadObj.Count; i++)
        {
            DisposeResObj(keys[i]);
        }

        loadObj.Clear();
    }

    public void DisposeBundle(string bundleName)
    {
        if(loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = loadHelper[bundleName];

            List<string> depences = loader.GetDependence();
            for(int i=0; i<depences.Count; i++)
            {
                if(loadHelper.ContainsKey(depences[i]))
                {
                    IABRelationManager dependence = loadHelper[depences[i]];
                    if (dependence.RemoveReference(bundleName))
                    {
                        DisposeBundle(dependence.getBundleName());
                    }
                }
            }

            if(loader.GetRefference().Count <= 0)
            {
                loader.Dispose();
                loadHelper.Remove(bundleName);
            }
        }
    }
    
    public void DisposAllBundle()
    {
        List<string> keys = new List<string>();
        keys.AddRange(loadHelper.Keys);
        for (int i = 0; i < loadHelper.Count; i++)
        {
            IABRelationManager loader = loadHelper[keys[i]];
            loader.Dispose();
        }
        loadHelper.Clear();
    }

    public void DisposeAllBundleAndRes()
    {
        DisposAllObj();
        DisposAllBundle();
    }
    //对外接口
    public void LoadAssetBundle(string bundle, LoaderProgress progress, LoadAssetBundleCallBack callBack)
    {
        if(!loadHelper.ContainsKey(bundle))
        {
            IABRelationManager loader = new IABRelationManager();
            loader.Initial(bundle,progress);
            loadHelper.Add(bundle, loader);
            callBack(scenceName, bundle);
        }
        else
        {
            Debug.Log("IABManger have contain bundle name ==" + bundle);
        }
    }

    string[] GetDependences(string bundleName)
    {
        return IABManifestLoader.Instance.GetDependence(bundleName);
    }

    public IEnumerator LoadAssetBundleDependences(string bundleName, string refName, LoaderProgress progress)
    {
        if(!loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = new IABRelationManager();
            loader.Initial(bundleName, progress);
            if(refName != null)
            {
                loader.AddRefference(refName);
            }
            loadHelper.Add(bundleName, loader);
            yield return LoadAssetBundles(bundleName);
        }
        else
        {
            if (refName != null)
            {
                IABRelationManager loader = loadHelper[bundleName];
                loader.AddRefference(refName);
            }
        }
    }

    /// <summary>
    /// 加载 assetbundle 必须先加载manifest
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public IEnumerator LoadAssetBundles(string bundleName)
    {
        while(!IABManifestLoader.Instance.IsLoadFinish())
        {
            yield return null;
        }

        IABRelationManager loader = loadHelper[bundleName];

        string[] dependence = GetDependences(bundleName);

        loader.SetDependence(dependence);

        for(int i=0; i<dependence.Length; i++)
        {
            yield return LoadAssetBundleDependences(dependence[i], bundleName, loader.GetProgress());
        }

        yield return loader.LoadAssetBundle();
    }

    #region 由下层提供API
    public void DebugAssetBundle(string bundleName)
    {
        if(loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = loadHelper[bundleName];
            loader.DebuggerAsset();
        }
    }

    public bool IsLoadingFinish(string bundleName)
    {
        if (loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = loadHelper[bundleName];
            return loader.IsBundleLoadFinish();
        }
        else
        {
            Debug.Log("IABRelation no contain bundle==" + bundleName);
            return false;
        }
    }

    public UnityEngine.Object GetSingleResources(string bundleName, string resName)
    {
        //表示是否已经缓存了
        if (loadObj.ContainsKey(bundleName))
        {
            AssetResObj tmpRes = loadObj[bundleName];
            List<Object> tmpObj = tmpRes.GetResObj(resName);
            if(tmpObj != null)
            {
                return tmpObj[0];
            }
        }

        //表示已经加载过bundle
        if (loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = loadHelper[bundleName];
            Object tmpObj = loader.GetSingleResource(resName);

            //放入缓存
            AssetObj tmpAssetObj = new AssetObj(tmpObj);
            if(loadObj.ContainsKey(bundleName))
            {
                loadObj[bundleName].AddResObj(resName, tmpAssetObj);
            }
            else
            {
                AssetResObj tmpRes = new AssetResObj(resName, tmpAssetObj);
                loadObj.Add(bundleName, tmpRes);
            }

            return tmpObj;
        }
        return null;
    }

    public UnityEngine.Object[] GetMultiResources(string bundleName, string resName)
    {
        //表示是否已经缓存了
        if (loadObj.ContainsKey(bundleName))
        {
            AssetResObj tmpRes = loadObj[bundleName];
            List<Object> tmpObj = tmpRes.GetResObj(resName);
            if (tmpObj != null)
            {
                return tmpObj.ToArray();
            }
        }

        //表示已经加载过bundle
        if (loadHelper.ContainsKey(bundleName))
        {
            IABRelationManager loader = loadHelper[bundleName];
            Object[] tmpObj = loader.GetMultiResource(resName);

            //放入缓存
            AssetObj tmpAssetObj = new AssetObj(tmpObj);
            if (loadObj.ContainsKey(bundleName))
            {
                loadObj[bundleName].AddResObj(resName, tmpAssetObj);
            }
            else
            {
                AssetResObj tmpRes = new AssetResObj(resName, tmpAssetObj);
                loadObj.Add(bundleName, tmpRes);
            }

            return tmpObj;
        }
        return null;
    }
   

    #endregion
}
