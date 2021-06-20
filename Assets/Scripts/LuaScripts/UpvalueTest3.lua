
local func1 = function()

    local a = 1

    local func2 = function()

        local func3 = function()
            a = 2
        end
        
        func3()

    end
    
    func2()

    print(a)--2
end

func1()  