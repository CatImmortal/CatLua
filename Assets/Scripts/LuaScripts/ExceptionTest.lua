local function div0(a,b)
    if b == 0 then
        error("div by zero")
    else
        return a / b
    end
end

local function div1(a,b)
    return div0(a,b)
end

local function div2(a,b)
    return div1(a,b)
end

local ok,ret = pcall(div0,4,2)
print(ok,ret)

local ok,err = pcall(div2,4,0)
print(ok,err)

local ok,err = pcall(div0,4,{},{})
print(ok,err)