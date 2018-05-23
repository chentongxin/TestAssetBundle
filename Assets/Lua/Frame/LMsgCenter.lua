--主要负责各个消息模块儿 转发

LMsgCenter = { msgId = 0}

LMsgCenter.__index = LMsgCenter;

local this = LMsgCenter;

function LMsgCenter:New(msgid)
	local self = {};
	setmetatable(self, LMsgCenter);
	self.msgId = msgid;
	return self;
end

function LMsgCenter.GetInstance()
	return this;
end

function LMsgCenter.RecvMsg(msg)

end

function LMsgCenter.SendToMsg(msg)
	this.AnasyMsg(msg);
end

function LMsgCenter.AnasyMsg(msg)
	managerId = msg:GetManager();
	
	if managerId == LManagerID.LAssetManager then
	
	elseif managerId == LManagerID.LAudioManager then
	
	elseif managerId == LManagerID.LUIManager then
		LUIManager.GetInstance().ProcessEvent(msg);
	end

end