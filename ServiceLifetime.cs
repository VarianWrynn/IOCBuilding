using System;
using System.Collections.Generic;
using System.Text;

namespace IOCBuilding
{
    /// <summary>
    /// 服务的生命周期。我们可以实现对注册服务的生命周期的管理，类型简单，不多说了。
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// 瞬时服务实例。
        /// </summary>
        Transient,

        /// <summary>
        /// 单例服务实例。
        /// </summary>
        Singleton,

        /// <summary>
        /// 作用域服务实例。
        /// </summary>
        Scoped,

        /// <summary>
        /// 线程服务实例。
        /// </summary>
        PerThread
    }
}
