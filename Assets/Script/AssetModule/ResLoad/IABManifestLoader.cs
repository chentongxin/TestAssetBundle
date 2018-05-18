using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IABManifestLoader {

    public AssetBundleManifest assetManifest;

    public string manifestPath;

    public bool isLoadFinish;

    public AssetBundle manifestLoader;

    public IABManifestLoader()
    {
        assetManifest = null;
        manifestLoader = null;
        isLoadFinish = false;
        manifestPath = IPathTools.GetWWWAssetBundlePath() + "/" + IPathTools.GetPlatFormFolderName(Application.platform);
    }
    
    public IEnumerator LoadManifest()
    {
        WWW manifest = new WWW(manifestPath);
        yield return manifest;
        if (!string.IsNullOrEmpty(manifest.error))
        {
            Debug.Log(manifest.error);
        }
        else
        {
            if(manifest.progress >= 1.0)
            {
                manifestLoader = manifest.assetBundle;
                assetManifest = manifestLoader.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                isLoadFinish = true;
            }
        }
    }

    public string[] GetDependence(string name)
    {
        return assetManifest.GetAllDependencies(name);
    }

    public void  UnloadManifest()
    {
        manifestLoader.Unload(true);
    }

    public void SetManifestPath(string path)
    {
        manifestPath = path;
    }

    public static IABManifestLoader instance = null;

    public static IABManifestLoader Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new IABManifestLoader();
            }
            return instance;
        }
    }

    public bool IsLoadFinish()
    {
        return isLoadFinish;
    }
}
