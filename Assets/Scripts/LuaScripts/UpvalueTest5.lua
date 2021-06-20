local function func1()
    local a = 1

    --测试对upvalue的值复制
    local function func2()
        local t = {}
        t[1] = 9
        t[2] = a

        return t
    end

    local function func3()
        a = 2
    end

    local t = func2()
    func3()
    
    print(t) --{1 = 9， 2 = 1}
end

func1()