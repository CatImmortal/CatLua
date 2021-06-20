
local function func1()
    local a = 1

    --测试对upvalue的值复制
    local function func2()
        local b = a
        return b
    end

    local function func3()
        a = 2
    end

    local b = func2()

    func3()
    

    print(b) --1

end

func1()