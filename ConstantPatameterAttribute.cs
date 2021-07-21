using System;
using System.Collections.Generic;
using System.Text;

namespace IOCBuilding
{
    /// <summary>
    /// 该类型是一个标记特性，用于标注不需要注入而进行传递的参数，可以使用该属性。
    /// 该类型定义了在服务初始化的时候需要从外界出入的参数，如果参数被标注，则说明改参数所需要的参数从外界传入。该类型是密封类型，不可以被继承。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter, AllowMultiple = false,
        Inherited = true)]
    public sealed class ConstantPatameterAttribute : Attribute
    {
    }
}
