using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// Lua解释器状态
    /// </summary>
    public class LuaState
    {
        public LuaState()
        {
            stack = new LuaStack(20);
        }

        private LuaStack stack;

        /// <summary>
        /// 获取栈顶索引
        /// </summary>
        public int Top
        {
            get
            {
                return stack.Top;
            }
        }


        /// <summary>
        /// 获取绝对索引
        /// </summary>
        public int GetAbsIndex(int index)
        {
            return stack.GetAbsIndex(index);
        }

        /// <summary>
        /// 从栈顶弹出n个值
        /// </summary>
        public void Pop(int n)
        {
            for (int i = 0; i < n; i++)
            {
                stack.Pop();
            }
        }

        /// <summary>
        /// 将栈中form位置的值复制到target位置
        /// </summary>
        public void Copy(int form, int target)
        {
            LuaValueUnion value = stack.Get(form);
            stack.Set(target, value);
        }

        /// <summary>
        /// 将index位置的值复制并压入栈顶
        /// </summary>
        public void CopyToTop(int index)
        {
            LuaValueUnion value = stack.Get(index);
            stack.Push(value);
        }

        /// <summary>
        /// 将栈顶值弹出，然后复制到index位置
        /// </summary>
        public void PopAndCopy(int index)
        {
            LuaValueUnion value = stack.Pop();
            stack.Set(index, value);
        }

        /// <summary>
        /// 将栈顶值弹出，然后插入到index位置
        /// </summary>
        public void PopAndInsert(int index)
        {
            Rotate(index, 1);
        }

        /// <summary>
        /// 删除index位置的值，然后将上面的值都往下移1位
        /// </summary>
        public void Remove(int index)
        {
            Rotate(index, -1);
            Pop(1);
        }

        /// <summary>
        /// 将index到top的值往栈顶方向旋转n位，如果n是负数，就朝栈底方向旋转
        /// </summary>
        public void Rotate(int index, int n)
        {
            int absIndex = GetAbsIndex(index);

            int middle;
            if (n >= 0)
            {
                //向栈顶旋转
                middle = Top - n;
            }
            else
            {
                //向栈底旋转
                middle = absIndex - n - 1;
            }

            //翻转三次
            stack.Reverse(absIndex, middle);
            stack.Reverse(middle + 1, Top);
            stack.Reverse(absIndex, Top);
        }

        /// <summary>
        /// 设置栈顶索引
        /// </summary>
        public void SetTop(int index)
        {
            int newTop = GetAbsIndex(index);
            int n = Top - newTop;

            if (n > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    stack.Pop();
                }
            }
            else if (n < 0)
            {
                n = -n;
                for (int i = 0; i < n; i++)
                {
                    stack.Push(default);
                }
            }

        }

        /// <summary>
        /// 压入nil值
        /// </summary>
        public void Push()
        {
            stack.Push(default);
        }

        /// <summary>
        /// 压入bool值
        /// </summary>
        public void Push(bool b)
        {
            stack.Push(new LuaValueUnion(LuaDataType.Boolean, boolean: b));
        }

        /// <summary>
        /// 压入integer值
        /// </summary>
        public void Push(long l)
        {
            stack.Push(new LuaValueUnion(LuaDataType.Integer, integer: l));
        }

        /// <summary>
        /// 压入number值
        /// </summary>
        public void Push(double d)
        {
            stack.Push(new LuaValueUnion(LuaDataType.Number, number: d));
        }

        /// <summary>
        /// 压入string值
        /// </summary>
        public void Push(string str)
        {
            stack.Push(new LuaValueUnion(LuaDataType.String, str: str));
        }

        /// <summary>
        /// 获取栈中index位置的值的类型
        /// </summary>
        public LuaDataType GetType(int index)
        {
            if (!stack.IsValid(index))
            {
                return LuaDataType.None;
            }
            return stack.Get(index).Type;
        }

        /// <summary>
        /// 栈中index位置的值是否是None或Nil
        /// </summary>
        public bool IsNoneOrNil(int index)
        {
            LuaValueUnion value = stack.Get(index);
            return value.Type == LuaDataType.None || value.Type == LuaDataType.Nil;
        }

        /// <summary>
        /// 栈中index位置的值是否是string或number
        /// </summary>
    
        public bool IsStringOrNumber(int index)
        {
            LuaValueUnion value = stack.Get(index);
            return value.Type == LuaDataType.String || value.Type == LuaDataType.Number;
        }

        /// <summary>
        /// 栈中index位置的值是否是number或可以转换为number
        /// </summary>
        public bool IsNumber(int index)
        {
            double d;
            bool result = TryGetNumber(index, out d);
            return result;
        }

        /// <summary>
        /// 从栈中index位置取出一个bool值，如果不是bool类型则进行类型转换
        /// </summary>
        public bool GetBoolean(int index)
        {
            LuaValueUnion value = stack.Get(index);
            return ConvertToBoolean(value);
        }

        /// <summary>
        /// 转换为bool值
        /// </summary>
        public bool ConvertToBoolean(LuaValueUnion value)
        {
            //Lua中只有nil和false表示假，其他都为真

            switch (value.Type)
            {
                case LuaDataType.Nil:
                    return false;

                case LuaDataType.Boolean:
                    return value.Boolean;

                default:
                    return true;
 
            }
        }



        /// <summary>
        /// 从栈中index位置取出一个number，如果无法转换会返回0
        /// </summary>
        public double GetNumber(int index)
        {
            double d;
            TryGetNumber(index,out d);
            return d;
        }

        /// <summary>
        /// 从栈中index位置取出一个number，如果无法转换会返回false
        /// </summary>
        public bool TryGetNumber(int index,out double d)
        {
            LuaValueUnion value = stack.Get(index);
            switch (value.Type)
            {
                case LuaDataType.Integer:
                    d = value.Integer;
                    return true;

                case LuaDataType.Number:
                    d = value.Number;
                    return true;

                default:
                    d = 0;
                    return false;   
            }
        }

        /// <summary>
        /// 从栈中index位置取出一个integer，如果无法转换会返回0
        /// </summary>
        public long GetInteger(int index)
        {
            long l;
            TryGetInteger(index, out l);
            return l;
        }

        /// <summary>
        /// 从栈中index位置取出一个integer，如果无法转换会返回false
        /// </summary>
        public bool TryGetInteger(int index,out long l)
        {
            LuaValueUnion value = stack.Get(index);
            if (value.Type == LuaDataType.Integer)
            {
                l = value.Integer;
                return true;
            }

            l = 0;
            return false;
        }

        /// <summary>
        /// 从栈中index位置取出一个string，如果是数字，会将值转换为字符串，并修改栈
        /// </summary>
        public string GetString(int index)
        {
            LuaValueUnion value = stack.Get(index);
            string str = string.Empty;
            switch (value.Type)
            {
              
                case LuaDataType.Integer:
                    str = value.Integer.ToString();
                    stack.Set(index, new LuaValueUnion(LuaDataType.String,str: str));
                    break;

                case LuaDataType.Number:
                    str = value.Number.ToString();
                    stack.Set(index, new LuaValueUnion(LuaDataType.String, str: str));
                    break;

                case LuaDataType.String:
                    str = value.Str;
                    break;
            }

            return str;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 1; i <= stack.Top; i++)
            {
                LuaValueUnion value = stack.Get(i);
                switch (value.Type)
                {
                  
                    case LuaDataType.Boolean:
                        s += string.Format("[{0}]", GetBoolean(i));
                        break;
                  
                    case LuaDataType.Integer:
                        s += string.Format("[{0}]", GetInteger(i));
                        break;
                    case LuaDataType.Number:
                        s += string.Format("[{0}]", GetNumber(i));
                        break;
                    case LuaDataType.String:
                        s += string.Format("[\"{0}\"]", GetString(i));
                        break;
                    default:
                        s += string.Format("[{0}]", value.Type.ToString());
                        break;

                }
            }
            return s;
        }
    }
}

