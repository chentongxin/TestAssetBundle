LAssetMsg = LMsgBase:New()

LAssetMsg.__index = LAssetMsg;

function LAssetMsg:New(msgid,scene,bundle,res,single, backFunc)
	local self = {};
	setmetatable(self, LAssetMsg);
	self.msgId = msgid;
	self.sceneName = scene;
	self.bundleName = bundle;
	self.resName = res;
	self.isSingle = single;
	self.callBackFunc = backFunc;
	return self;
end