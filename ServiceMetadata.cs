using System;
using System.Collections.Generic;
using System.Text;

namespace IOCBuilding
{
    /// <summary>
    /// 该类型定义了注册服务的元数据。
    /// </summary>
    public sealed class ServiceMetadata
    {
        /// <summary>
        /// 获取或者设置注册服务的类型。
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// 获取或者设置注册服务的生命周期。
        /// </summary>
        public ServiceLifetime Lifetime { get; set; }

        /// <summary>
        /// 获取或者设置单件的服务实例。
        /// </summary>
        public Object SingletonInstance { get; set; }
    }
}
