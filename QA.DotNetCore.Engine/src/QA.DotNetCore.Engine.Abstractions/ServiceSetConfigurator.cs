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
        private readonly IServiceProvider _serviceProvider;

        public ServiceSetConfigurator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void RegisterInstance(TServiceInterface service)
        {
            _instances.Add(service);
        }

        public void RegisterSingleton<T>() where T : TServiceInterface
        {
            _instances.Add(_serviceProvider.GetRequiredService<T>());
        }

        public void RegisterScoped<T>() where T : TServiceInterface
        {
            _types.Add(typeof(T));
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
