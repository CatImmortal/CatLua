
local foo

do
    --闭合upvalue测试
    local b = "b"
    foo = function()
        b = "bb"
        print(b)
    end

end

foo() --bb

