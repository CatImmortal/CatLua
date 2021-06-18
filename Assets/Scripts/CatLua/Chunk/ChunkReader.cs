using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
namespace CatLua
{
    /// <summary>
    /// Chunk数据读取器
    /// </summary>
    public struct ChunkReader
    {
        public ChunkReader(byte[] data)
        {
            this.data = data;
            curIndex = 0;
        }

        /// <summary>
        /// 二进制trunk
        /// </summary>
        private byte[] data;

        /// <summary>
        /// 当前索引
        /// </summary>
        private int curIndex;

        public byte ReadByte()
        {
            byte b = data[curIndex];
            curIndex++;
            return b;
        }

        public byte[] ReadBytes(uint n)
        {
            byte[] bytes = new byte[n];
            Array.Copy(data, curIndex, bytes, 0, n);
            curIndex += (int)n;
            return bytes;
        }

        public uint ReadUint()
        {
            uint ui = BitConverter.ToUInt32(data, curIndex);
            curIndex += 4;
            return ui;
        }

        public ulong ReadUlong()
        {
            ulong ul = BitConverter.ToUInt64(data, curIndex);
            curIndex += 8;
            return ul;
        }

        public long ReadLuaInteger()
        {
            long l = BitConverter.ToInt64(data, curIndex);
            curIndex += 8;
            return l;
        }

        public double ReadLuaNumber()
        {
            double d = BitConverter.ToDouble(data, curIndex);
            curIndex += 8;
            return d;
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        public string ReadString()
        {
            //短字符串 长度<253 用1byte记录长度+1
            ulong size = ReadByte();

            if (size == 0)
            {
                return string.Empty;
            }

            if (size == 0xFF)
            {
                //长字符串 长度>=253 用1个size_t记录长度+1
                size = ReadUint();
            }

            byte[] bytes = ReadBytes((uint)size - 1);

            string s = Encoding.UTF8.GetString(bytes);
            return s;
        }

        /// <summary>
        /// 读取指令表
        /// </summary>
        public uint[] ReadCode()
        {
            uint[] code = new uint[ReadUint()];
            for (int i = 0; i < code.Length; i++)
            {
                //1条指令 4byte
                code[i] = ReadUint();
            }
            return code;
        }

        /// <summary>
        /// 读取常量
        /// </summary>
        public LuaConstantUnion ReadConstant()
        {
            LuaConstantUnion constant;

            byte tag = ReadByte();
            switch (tag)
            {
                case Constants.TagNil:
                    constant = new LuaConstantUnion(LuaConstantType.Nil);
                    break;
                case Constants.TagBoolean:
                    constant = new LuaConstantUnion(LuaConstantType.Boolean, boolean: ReadByte() != 0);
                    break;
                case Constants.TagInteger:
                    constant = new LuaConstantUnion(LuaConstantType.Integer, integer: ReadLuaInteger());
                    break;
                case Constants.TagNumber:
                    constant = new LuaConstantUnion(LuaConstantType.Number, number: ReadLuaNumber());
                    break;
                case Constants.TagShortStr:
                    constant = new LuaConstantUnion(LuaConstantType.ShorStr, str: ReadString());
                    break;
                case Constants.TagLongStr:
                    constant = new LuaConstantUnion(LuaConstantType.LongStr, str: ReadString());
                    break;
                default:
                    throw new Exception("常量tag不对");
            }

            return constant;
        }

        /// <summary>
        /// 读取常量表
        /// </summary>
        public LuaConstantUnion[] ReadConstants()
        {
            LuaConstantUnion[] constants = new LuaConstantUnion[ReadUint()];
            for (int i = 0; i < constants.Length; i++)
            {
                constants[i] = ReadConstant();
            }
            return constants;
        }

        /// <summary>
        /// 读取upvalue表
        /// </summary>
        public Upvalue[] ReadUpvalues()
        {
            Upvalue[] upvalues = new Upvalue[ReadUint()];
            for (int i = 0; i < upvalues.Length; i++)
            {
                Upvalue upvalue = new Upvalue
                {
                    Instack = ReadByte(),
                    Index = ReadByte()
                };
                upvalues[i] = upvalue;
            }
            return upvalues;
        }

        /// <summary>
        /// 读取子函数原型表
        /// </summary>
        public FuncPrototype[] ReadProtos(string parentSource)
        {
            FuncPrototype[] protos = new FuncPrototype[ReadUint()];
            for (int i = 0; i < protos.Length; i++)
            {
                protos[i] = ReadProto(parentSource);
            }
            return protos;
        }

        /// <summary>
        /// 读取行号表
        /// </summary>
        public uint[] ReadLineInfo()
        {
            uint[] lineInfo = new uint[ReadUint()];
            for (int i = 0; i < lineInfo.Length; i++)
            {
                lineInfo[i] = ReadUint();
            }
            return lineInfo;
        }

        /// <summary>
        /// 读取局部变量表
        /// </summary>
        public LocalVar[] ReadLocVars()
        {
            LocalVar[] locvars = new LocalVar[ReadUint()];
            for (int i = 0; i < locvars.Length; i++)
            {
                LocalVar locVar = new LocalVar();
                locVar.VarName = ReadString();
                locVar.StartPC = ReadUint();
                locVar.EndPC = ReadUint();
                locvars[i] = locVar;
            }
            return locvars;
        }

        /// <summary>
        /// 读取upvalue名列表
        /// </summary>
        public string[] ReadUpvalueNames()
        {
            string[] names = new string[ReadUint()];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = ReadString();
            }
            return names;
        }

        /// <summary>
        /// 检查头信息
        /// </summary>
        public ChunkHeader CheckHeader()
        {
            ChunkHeader header = new ChunkHeader();

            header.Signature = ReadBytes(4);
            string sign = Encoding.UTF8.GetString(header.Signature);
            if (sign != Constants.LuaSignature)
            {
                throw new Exception("签名不对");
            }

            header.Version = ReadByte();
            if (header.Version != Constants.LuacVersion)
            {
                throw new Exception("版本不对");
            }

            header.Format = ReadByte();
            if (header.Format != Constants.LuacFormat)
            {
                throw new Exception("格式不对");
            }

            header.LuacData = ReadBytes(6);
            string data = Encoding.UTF8.GetString(header.LuacData);
            if (data != Constants.LuacData)
            {
                throw new Exception("luacData不对");
            }

            header.CintSize = ReadByte();
            if (header.CintSize != Constants.CintSize)
            {
                throw new Exception("cint size不对");
            }

            header.SizetSize = ReadByte();
            if (header.SizetSize != Constants.CsizetSize)
            {
                throw new Exception("csizet size不对");
            }

            header.InstructionSize = ReadByte();
            if (header.InstructionSize != Constants.InstructionSize)
            {
                throw new Exception("指令size不对");
            }

            header.LuaIntergerSize = ReadByte();
            if (header.LuaIntergerSize != Constants.LuaIntergerSize)
            {
                throw new Exception("lua整数size不对");
            }

            header.LuaNumberSize = ReadByte();
            if (header.LuaNumberSize != Constants.LuaNumberSize)
            {
                throw new Exception("lua浮点数size不对");
            }

            header.LuacInt = ReadLuaInteger();
            if (header.LuacInt != Constants.LuacInt)
            {
                throw new Exception("luac int不对");
            }

            header.LuacNum = ReadLuaNumber();
            if (header.LuacNum != Constants.LuacNum)
            {
                throw new Exception("luac num不对");
            }

            return header;
        }

        /// <summary>
        /// 读取函数原型
        /// </summary>
        public FuncPrototype ReadProto(string parentSource)
        {
            string source = ReadString();
            if (string.IsNullOrEmpty(source))
            {
                //子函数的proto 源文件名和父函数相同
                source = parentSource;
            }

            FuncPrototype proto = new FuncPrototype
            {
                Source = source,
                LineDefined = ReadUint(),
                LastLineDefined = ReadUint(),
                NumParams = ReadByte(),
                IsVararg = ReadByte(),
                MaxStackSize = ReadByte(),
                Code = ReadCode(),
                Constants = ReadConstants(),
                Upvalues = ReadUpvalues(),
                Protos = ReadProtos(source),
                LineInfo = ReadLineInfo(),
                Locvars = ReadLocVars(),
                UpvalueNames = ReadUpvalueNames()
            };

            return proto;
        }
    }


}

