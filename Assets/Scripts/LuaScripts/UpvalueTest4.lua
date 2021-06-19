function newCounter()
    local count = 0
    return function ()
        count = count + 1
        return count
    end
end

c1 = newCounter()
print(c1())
print(c1())

c2 = newCounter()
print(c2())
print(c1())
print(c2())