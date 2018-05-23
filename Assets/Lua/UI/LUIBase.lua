LUIBase = { }

LUIBase.__index = LUIBase;

function LUIBase:New()
	local self = {};
	setmetatable(self, LUIBase);
	return self;
end

function LUIBase:RegistSelf(script,msgs)
	LUIManager.GetInstance():RegistMsgs(script,msgs);
end

function LUIBase:UnRegistSelf(script, msgs)
	LUIManager.GetInstance().UnRegistMsgs(script,msgs);
end

function LUIBase:Destroy()
	self:UnRegistSelf(self,self.msgIds);
end