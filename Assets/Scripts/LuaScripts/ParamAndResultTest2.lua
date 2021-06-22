local function g()
    return 33,44,55
end


local t = {11,22,g(),66,77}

print(t)