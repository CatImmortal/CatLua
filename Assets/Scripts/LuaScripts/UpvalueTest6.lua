
local function func1()
    local a = 1

    local function func2()
        local b = a
        return b
    end

    local function func3()
        a = 2
    end

    local c = func2()

    func3()
    

    print(c)

end

func1()