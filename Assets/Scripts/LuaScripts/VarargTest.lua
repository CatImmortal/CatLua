local function createTable(a,b,...)
    local t = {a,b,...}
    local i = 1
    return t
end

local t = createTable(11,22,33,44,55)

print(t)

