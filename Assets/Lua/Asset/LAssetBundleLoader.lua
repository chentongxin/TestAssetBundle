LAssetBundleLoader = LAssetBase:New();

LAssetBundleLoader.__index = LAssetBundleLoader;

local this = LAssetBundleLoader;

function LAssetBundleLoader:New()
	local self = {};
	setmetatable(self, LAssetMsg);
	return self;
end

function LAssetBundleLoader:SendMsg()
	
end

function LAssetBundleLoader:Awake()
	self.msgId[1] = LAssetEvent.HunkRes;
	this:RegistSelf(this,self.msgId);
end

--lua层传递的resource请求
function LAssetBundleLoader:ProcessEvent(msg)
	if msg.msgId == LAssetEvent.HunkRes then
		LuaLoadReses.Instance:GetResource(msg.sceneName,msg.bundleName,msg.resName, msg.isSingle, msg.callBackFunc);
	elseif msg.msgId == LAssetEvent.ReleaseSingleObj then

	end
end