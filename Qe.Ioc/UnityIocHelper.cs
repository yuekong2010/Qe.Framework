﻿using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;

namespace Qe.Ioc
{
    public class UnityIocHelper
    {
        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="containerName">容器名称</param>
        private UnityIocHelper(string containerName)
        {
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            if (containerName == "IOCcontainer")
            {
                _container = new UnityContainer();
                section.Configure(_container, containerName);
            }
            else if (section.Containers.Count > 1)
            {
                _container = new UnityContainer();
                section.Configure(_container, containerName);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 容器
        /// </summary>
        private readonly IUnityContainer _container;

        /// <summary>
        /// UnityIoc容器实例
        /// </summary>
        public static UnityIocHelper Instance { get; } = new UnityIocHelper("IOCcontainer");

        public static UnityIocHelper WfInstance { get; } = new UnityIocHelper("WfIOCcontainer");
        #endregion

        #region 获取对应接口的具体实现类
        /// <summary>
        /// 获取实现类(默认映射)
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <returns>接口</returns>
        public T GetService<T>()
        {
            return _container.Resolve<T>();
        }
        /// <summary>
        /// 获取实现类(默认映射)带参数的
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <param name="parameter">参数</param>
        /// <returns>接口</returns>
        public T GetService<T>(params ParameterOverride[] parameter)
        {
            return _container.Resolve<T>(parameter);
        }
        /// <summary>
        /// 获取实现类(指定映射)带参数的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="parameter"></param>
        /// <returns>接口</returns>
        public T GetService<T>(string name, params ParameterOverride[] parameter)
        {
            return _container.Resolve<T>(name, parameter);
        }
        #endregion

        #region 判断接口是否被注册了
        /// <summary>
        /// 判断接口是否被实现了
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <returns>bool</returns>
        public bool IsResolve<T>()
        {
            return _container.IsRegistered<T>();
        }
        /// <summary>
        /// 判断接口是否被实现了
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <param name="name">映射名称</param>
        /// <returns></returns>
        public bool IsResolve<T>(string name)
        {
            return _container.IsRegistered<T>(name);
        }
        #endregion
    }
}
