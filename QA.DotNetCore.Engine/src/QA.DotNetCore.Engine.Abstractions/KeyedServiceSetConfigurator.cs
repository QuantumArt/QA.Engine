using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Abstractions
{
    /// <summary>
    /// Класс, в котором можно регистрировать множество сервисов, реализующих общий интерфейс
    /// </summary>
    /// <typeparam name="TServiceInterface"></typeparam>
    public class KeyedServiceSetConfigurator<TKey, TServiceInterface>
    {
        private readonly ConcurrentDictionary<TKey, IList<TServiceInterface>> _instancesMap = new ConcurrentDictionary<TKey, IList<TServiceInterface>>();
        private readonly ConcurrentDictionary<TKey, IList<Type>> _typesMap = new ConcurrentDictionary<TKey, IList<Type>>();
        private readonly IList<TServiceInterface> _allInstances = new List<TServiceInterface>();
        private readonly IList<Type> _allTypes = new List<Type>();

        public KeyedServiceSetConfigurator()
        {
        }

        public void RegisterInstance(TServiceInterface service, params TKey[] keys)
        {
            foreach(var key in keys)
            {
                _instancesMap.GetOrAdd(key, new List<TServiceInterface>()).Add(service);
            }

            _allInstances.Add(service);
        }

        public void Register<T>(params TKey[] keys) where T : TServiceInterface
        {
            RegisterInternal(typeof(T), keys);
        }

        public void Register(Type t, params TKey[] keys)
        {
            if (typeof(TServiceInterface).IsAssignableFrom(t))
            {
                RegisterInternal(t, keys);
            }
            else
            {
                throw new ArgumentException($"Type {t.FullName} is not assignable from {typeof(TServiceInterface).FullName}");
            }
        }

        private void RegisterInternal(Type t, params TKey[] keys)
        {
            foreach (var key in keys)
            {
                _typesMap.GetOrAdd(key, new List<Type>()).Add(t);
            }

            _allTypes.Add(t);
        }

        /// <summary>
        /// Получить все зарегистрированные сервисы
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TServiceInterface> GetServices(IServiceProvider scopedProvider) =>
            GetServicesInternal(scopedProvider, _allInstances, _allTypes);


        /// <summary>
        /// Получить зарегистрированные сервисы по ключу
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TServiceInterface> GetServices(IServiceProvider scopedProvider, TKey key)
        {
            var instances = _instancesMap.TryGetValue(key, out var actualInstances) ? actualInstances : new List<TServiceInterface>();
            var types = _typesMap.TryGetValue(key, out var actualTypes) ? actualTypes : new List<Type>();
            return GetServicesInternal(scopedProvider, instances, types);
        }

        private IEnumerable<TServiceInterface> GetServicesInternal(IServiceProvider scopedProvider, IList<TServiceInterface> instances, IList<Type> types)
        {
            if (types.Any())
            {
                return instances.Concat(types.Select(t => (TServiceInterface)scopedProvider.GetRequiredService(t))).ToList();
            }

            return instances;
        }
    }
}
