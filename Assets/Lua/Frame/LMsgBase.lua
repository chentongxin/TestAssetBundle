LMsgBase = { msgId = 0}

LMsgBase.__index = LMsgBase;

function LMsgBase:New(msgid)
	local self = {};
	setmetatable(self, LMsgBase);
	self.msgId = msgid;
	return self;
end

function LMsgBase:GetManager()
	local tempId = math.floor(msgId / MsgSpan) * MsgSpan;
	return math.ceil(tmpId);
end