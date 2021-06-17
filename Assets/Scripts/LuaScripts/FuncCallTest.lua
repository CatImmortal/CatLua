local function max(...)
    local args = {...}
    local val,idx
    for i=1,#args do
        if val == nil or args[i] > val then
            val,idx = args[i],i
        end
    end
    return val,idx
end


local v2,i2 = max(3,9,7,128,35)


