LEventNode = { }

LEventNode.__index = LEventNode;

local this = LEventNode;

function LEventNode:New(event)
	local self = {};
	setmetatable(self, LManagerBase);
	self.value = event;
	self.next = nil;
	return self;
end