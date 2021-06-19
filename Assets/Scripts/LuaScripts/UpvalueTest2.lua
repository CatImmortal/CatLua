
local foo

do
    local b = "b"
    foo = function()
        b = "bb"
        print(b)
    end

end

foo()

