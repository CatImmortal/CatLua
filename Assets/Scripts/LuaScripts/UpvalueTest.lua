-- function newCounter()
--     local count = 0

--     return function()
--         count = count + 1
--         return count
--     end
-- end

-- c1 = newCounter()
-- print(c1())


-- c2 = newCounter()
-- print(c2())

local x = "x"
local y = "y"
local z = "z"

do
    local a = "a"
    local b = "b"
    local c = "c"

    local foo = function()
        b = 1
    end

    foo()

    print(b)
end 

