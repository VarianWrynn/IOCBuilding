using System;
using System.Collections.Generic;
using System.Text;

namespace IOCBuilding
{
    /// <summary>
    /// 该特性是一个实现属性注入的特性类，该类型是密封的。它是一个标识属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class InjectionPropertyAttribute : Attribute
    {

    }
}
