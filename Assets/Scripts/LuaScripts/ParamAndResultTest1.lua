local function g()
    return 33,44,55
end

local function f()
    return g()  
end

local t = {11,22,f()}

print(t)