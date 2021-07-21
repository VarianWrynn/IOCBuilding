using System;
using System.Collections.Generic;
using System.Text;

namespace IOCBuilding
{
    /// <summary>
    /// 该类型也是一个标记特性，用于标记方法，可以通过方法实现注入。
    /// 该类型定义了方法注入的特性类型，该类型是密封类型，不可以被继承。它也是一个标识类型。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InjectionMethodAttribute : Attribute
    {
    }
}
