local function g()
    return 33,44,55
end

local function f()
    return g()  --尾调用 返回值处理有问题 之后再修
end

local t = {11,22,f()}

print(t)