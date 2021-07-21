using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace IOCBuilding
{
    /// <summary>
    /// 自定义的IOC容器实现。
    /// https://ing.cnblogs.com/
    /// </summary>
    public class LeeContainer: ICustomContainer
    {
        private readonly IDictionary<string, ServiceMetadata> _Container;
        private readonly IDictionary<string, object[]> _Parameters;
        private readonly IDictionary<string, object> _ScopedContainer;


        /// <summary>
        /// 初始化类型的新实例，实例化容器。
        /// </summary>
        public LeeContainer()
        {
            _Container = new ConcurrentDictionary<string, ServiceMetadata>();
            _Parameters = new ConcurrentDictionary<string, object[]>();
            _ScopedContainer = new Dictionary<string, object>();
        }

        /// <summary>
        /// 可以创建子作用域。
        /// </summary>
        /// <returns></returns>
        public LeeContainer CreateScoped()
        {
            return new LeeContainer(_Container, _Parameters, new Dictionary<string, object>());
        }

        /// <summary>
        /// 通过是有构造函数初始化容器。
        /// </summary>
        /// <param name="container"></param>
        /// <param name="parameters"></param>
        /// <param name="scopedContainer"></param>
        private LeeContainer(IDictionary<string, ServiceMetadata> container, IDictionary<string, object[]> parameters, IDictionary<string, object> scopedContainer)
        {
            this._Container = container;
            this._Parameters = parameters;
            this._ScopedContainer = scopedContainer;
        }

        /// <summary>
        /// 以 TFrom 类型注册 TTo 实例。
        /// </summary>
        /// <typeparam name="TFrom">TTo 的基类类型</typeparam>
        /// <typeparam name="TTo">TFrom 的子类类型。</typeparam>
        public void Register<TFrom, TTo>() where TTo : TFrom
        {
            Register<TFrom, TTo>(null, ServiceLifetime.Transient, null);
        }

        /// <summary>
        /// 以 TFrom 类型注册 TTo 实例。
        /// </summary>
        /// <typeparam name="TFrom">TTo 的基类类型</typeparam>
        /// <typeparam name="TTo">TFrom 的子类类型。</typeparam>
        /// <param name="lifetime">要注册的服务的生命周期。</param>
        public void Register<TFrom, TTo>(ServiceLifetime lifetime) where TTo : TFrom
        {
            Register<TFrom, TTo>(null, lifetime, null);
        }

        /// <summary>
        /// 提供服务构建实例所需参数来以 TFrom 类型注册 TTo 实例。
        /// </summary>
        /// <typeparam name="TFrom">TTo 的基类类型</typeparam>
        /// <typeparam name="TTo">TFrom 的子类类型。</typeparam>
        /// <param name="parameterValues">要注册的服务在构建实例时需要的非注入参数。</param>
        public void Register<TFrom, TTo>(params object[] parameterValues) where TTo : TFrom
        {
            Register<TFrom, TTo>(null, ServiceLifetime.Transient, parameterValues);
        }

        /// <summary>
        /// 提供服务的名称构建实例来以 TFrom 类型注册 TTo 实例。
        /// </summary>
        /// <typeparam name="TFrom">TTo 的基类类型</typeparam>
        /// <typeparam name="TTo">TFrom 的子类类型。</typeparam>
        /// <param name="serviceName">要注册服务的名称。</param>
        public void Register<TFrom, TTo>(string serviceName) where TTo : TFrom
        {
            Register<TFrom, TTo>(serviceName, ServiceLifetime.Transient, null);
        }

        /// <summary>
        /// 提供服务的名称构建实例来以 TFrom 类型注册 TTo 实例。
        /// </summary>
        /// <typeparam name="TFrom">TTo 的基类类型</typeparam>
        /// <typeparam name="TTo">TFrom 的子类类型。</typeparam>
        /// <param name="serviceName">要注册服务的名称。</param>
        /// <param name="lifetime">要注册的服务的生命周期。</param>
        public void Register<TFrom, TTo>(string serviceName, ServiceLifetime lifetime) where TTo : TFrom
        {
            Register<TFrom, TTo>(serviceName, lifetime, null);
        }

        /// <summary>
        /// 提供服务的名称和参数构建实例以 TFrom 类型注册 TTo 实例。
        /// </summary>
        /// <typeparam name="TFrom">TTo 的基类类型</typeparam>
        /// <typeparam name="TTo">TFrom 的子类类型。</typeparam>
        /// <param name="serviceName">要注册服务的名称。</param>
        /// <param name="lifetime">要注册的服务的生命周期。</param>
        /// <param name="parameterValues">要注册的服务在构建实例时需要的非注入参数。</param>
        public void Register<TFrom, TTo>(string serviceName, ServiceLifetime lifetime, params object[] parameterValues) where TTo : TFrom
        {
            string key;
            if (string.IsNullOrEmpty(serviceName) || string.IsNullOrWhiteSpace(serviceName))
            {
                key = typeof(TFrom).FullName;
                if (!_Container.ContainsKey(key))
                {
                    _Container.Add(key, new ServiceMetadata() { ServiceType = typeof(TTo), Lifetime = lifetime });
                }
            }
            else
            {
                key = string.Format("{0}_{1}", typeof(TFrom).FullName, serviceName);
                if (!_Container.ContainsKey(key))
                {
                    _Container.Add(key, new ServiceMetadata() { ServiceType = typeof(TTo), Lifetime = lifetime });
                }
            }
            if (parameterValues != null && parameterValues.Length > 0)
            {
                _Parameters.Add(key, parameterValues);
            }
        }

        /// <summary>
        /// 以指定类型解析该类型的实例。
        /// </summary>
        /// <typeparam name="TFrom">要解析实例的基类型。</typeparam>
        /// <returns></returns>
        public TFrom Resolve<TFrom>()
        {
            return Resolve<TFrom>(null);
        }

        /// <summary>
        /// 以指定名称解析该基类型的实例。
        /// </summary>
        /// <typeparam name="TFrom">要解析实例的基类型。</typeparam>
        /// <param name="serviceName">要解析实例的名称。</param>
        /// <returns></returns>
        public TFrom Resolve<TFrom>(string serviceName)
        {
            return (TFrom)Create(typeof(TFrom), serviceName);
        }


        /// <summary>
        /// 通过递归实现解析实例对象。
        /// </summary>
        /// <param name="baseType">服务的基类型。</param>
        /// <param name="serviceName">服务实例的名称。</param>
        /// <returns></returns>
        private object Create(Type baseType, string serviceName = null)
        {
            #region 处理关键字

            string keyword;

            if (string.IsNullOrEmpty(serviceName) || string.IsNullOrWhiteSpace(serviceName))
            {
                keyword = string.Format("{0}", baseType.FullName);
            }
            else
            {
                keyword = string.Format("{0}_{1}", baseType.FullName, serviceName);
            }

            #endregion

            Type targetType = null; ServiceLifetime lifetime = ServiceLifetime.Transient;
            if (_Container.ContainsKey(keyword))
            {
                targetType = _Container[keyword].ServiceType;
                lifetime = _Container[keyword].Lifetime;
            }
            else if (keyword.IndexOf('_') != -1)
            {
                if (_Container.ContainsKey(keyword.Split('_')[0]))
                {
                    keyword = keyword.Split('_')[0];
                    targetType = _Container[keyword].ServiceType;
                    lifetime = _Container[keyword].Lifetime;
                }
            }
            else
            {
                throw new Exception("类型还未注册!");
            }

            #region 生命周期

            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    break;
                case ServiceLifetime.Singleton:
                    if (_Container[keyword].SingletonInstance != null)
                    {
                        return _Container[keyword].SingletonInstance;
                    }
                    break;
                case ServiceLifetime.Scoped:
                    if (_ScopedContainer.ContainsKey(keyword))
                    {
                        return _ScopedContainer[keyword];
                    }
                    break;
                case ServiceLifetime.PerThread:
                    var objInstance = CallContext.GetData($"{keyword}{Thread.CurrentThread.ManagedThreadId}");
                    if (objInstance != null)
                    {
                        return objInstance;
                    }
                    break;
                default:
                    break;
            }

            #endregion

            #region 选择构造函数

            ConstructorInfo ctor = null;

            //1、通过特性约束
            ctor = targetType.GetConstructors().FirstOrDefault(c => c.IsDefined(typeof(SelectedConstructorAttribute), true));

            if (ctor == null)
            {
                //2、参数最多的
                ctor = targetType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
            }

            #endregion

            #region 核心创建对象代码

            IList<object> parameters = new List<object>();
            var values = _Parameters.ContainsKey(keyword) ? _Parameters[keyword] : null;
            int index = 0;
            foreach (var parameter in ctor.GetParameters())
            {
                if (values != null && values.Length > 0 && parameter.IsDefined(typeof(ConstantPatameterAttribute), true))
                {
                    parameters.Add(values[index++]);
                }
                else
                {
                    var parameterType = parameter.ParameterType;
                    var instance = Create(parameterType, serviceName);
                    parameters.Add(instance);
                }
            }
            object oIntance = Activator.CreateInstance(targetType, parameters.ToArray());

            #endregion

            #region 属性注入

            Type propertyType = null;
            foreach (var property in targetType.GetProperties().Where(p => p.IsDefined(typeof(InjectionPropertyAttribute), true)))
            {
                propertyType = property.PropertyType;
                var propInstance = Create(propertyType);
                property.SetValue(oIntance, propInstance);
            }

            #endregion

            #region 方法注入

            foreach (var methodInfo in targetType.GetMethods().Where(p => p.IsDefined(typeof(InjectionMethodAttribute), true)))
            {
                IList<object> methodParameters = new List<object>();
                values = _Parameters.ContainsKey(keyword) ? _Parameters[keyword] : null;
                index = 0;
                foreach (var parameter in methodInfo.GetParameters())
                {
                    if (values != null && values.Length > 0 && parameter.IsDefined(typeof(ConstantPatameterAttribute)))
                    {
                        methodParameters.Add(values[index++]);
                    }
                    else
                    {
                        var methodParaType = parameter.ParameterType;
                        var paraInstance = Create(methodParaType, serviceName);
                        methodParameters.Add(paraInstance);
                    }
                }
                methodInfo.Invoke(oIntance, methodParameters.ToArray());
            }

            #endregion

            #region 生命周期

            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    break;
                case ServiceLifetime.Singleton:
                    if (_Container[keyword].SingletonInstance == null)
                    {
                        _Container[keyword].SingletonInstance = oIntance;
                    }
                    break;
                case ServiceLifetime.Scoped:
                    if (!_ScopedContainer.ContainsKey(keyword))
                    {
                        _ScopedContainer.Add(keyword, oIntance);
                    }
                    break;
                case ServiceLifetime.PerThread:
                    CallContext.SetData($"{keyword}{Thread.CurrentThread.ManagedThreadId}", oIntance);
                    break;
                default:
                    break;
            }

            #endregion

            return oIntance;
        }
    }
}
