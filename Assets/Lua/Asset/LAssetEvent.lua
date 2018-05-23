local lAssetBegin = LManagerID.LAssetManager;

LAssetEvent = 
{
    HunkRes = lAssetBegin + 1,

    ReleaseSingleObj = lAssetBegin + 2,
    ReleaseBundleObj = lAssetBegin + 3,
    ReleaseSceneObj = lAssetBegin + 4,

    ReleaseSingleBundle = lAssetBegin + 5,
    ReleaseSceneBundle = lAssetBegin + 6,
    ReleaseAll = lAssetBegin + 7,
	MaxValue = lAssetBegin + 8,
}