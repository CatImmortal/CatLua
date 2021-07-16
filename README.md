# CatLua
在Unity中使用C#实现的Lua解释器，主要目的是学习Lua底层机制与编译原理

实现了可执行Lua字节码指令的基于寄存器的Lua虚拟机，基于栈的跨语言交互机制，基于栈帧的函数调用机制

实现了可解析Lua EBNF文法（包括15种语句与11种表达式）的词法分析器、语法分析器、函数原型编译器，支持Lua基本语法与高级特性（如Table，泛型迭代器，pcall，元表与元方法，闭包与upvalue，vararg）

主要参考：《自己动手实现Lua》、《Lua设计与实现》
