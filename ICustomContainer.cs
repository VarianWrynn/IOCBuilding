using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace IOCBuilding
{
    /// <summary>
    /// 我们定义的 IOC 容器抽象基类型，它定义了 IOC 容器的核心功能。
    /// </summary>
    public interface ICustomContainer
    {
        /// <summary>
        /// 提供服务的名称和参数构建实例以 TFrom 类型注册 TTo 实例。
        /// </summary>
        /// <typeparam name="TFrom">TTo 的基类类型。</typeparam>
        /// <typeparam name="TTo">TFrom 的子类类型。</typeparam>
        /// <param name="serviceName">要注册服务的名称。</param>
        /// <param name="lifetime">要注册的服务的生命周期。</param>
        /// <param name="parameterValues">要注册的服务在构建实例时需要的非注入参数。</param>
        void Register<TFrom, TTo>(string serviceName, ServiceLifetime lifetime, params object[] parameterValues) where TTo : TFrom;

        /// <summary>
        /// 以指定名称解析该基类型的实例。
        /// </summary>
        /// <typeparam name="TFrom">要解析实例的基类型。</typeparam>
        /// <param name="serviceName">要解析实例的名称。</param>
        /// <returns></returns>
        TFrom Resolve<TFrom>(string serviceName);
    }
}
