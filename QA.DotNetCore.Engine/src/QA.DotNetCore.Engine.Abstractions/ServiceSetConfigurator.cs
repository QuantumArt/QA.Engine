using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Abstractions
{
    /// <summary>
    /// Класс, в котором можно регистрировать множество сервисов, реализующих общий интерфейс
    /// </summary>
    /// <typeparam name="TServiceInterface"></typeparam>
    public class ServiceSetConfigurator<TServiceInterface>
    {
        private readonly IList<TServiceInterface> _instances = new List<TServiceInterface>();
        private readonly IList<Type> _types = new List<Type>();

        public ServiceSetConfigurator()
        {
        }

        public void RegisterInstance(TServiceInterface service)
        {
            _instances.Add(service);
        }

        public void Register<T>() where T : TServiceInterface
        {
            _types.Add(typeof(T));
        }

        public void Register(Type t)
        {
            if (typeof(TServiceInterface).IsAssignableFrom(t))
            {
                _types.Add(t);
            }
            else
            {
                throw new ArgumentException($"Type {t.FullName} is not assignable from {typeof(TServiceInterface).FullName}");
            }
        }

        /// <summary>
        /// Получить все зарегистрированные сервисы
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TServiceInterface> GetServices(IServiceProvider scopedProvider)
        {
            if (_types.Any())
            {
                return _instances.Concat(_types.Select(t => (TServiceInterface)scopedProvider.GetRequiredService(t))).ToList();
            }

            return _instances;
        }
    }
}
