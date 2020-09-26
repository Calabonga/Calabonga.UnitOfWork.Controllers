using System;
using System.Linq;
using Calabonga.Microservices.Core.Validators;
using Calabonga.UnitOfWork.Controllers.Factories;
using Calabonga.UnitOfWork.Controllers.Managers;

using Microsoft.Extensions.DependencyInjection;

namespace Calabonga.UnitOfWork.Controllers.DependencyContainer
{
    /// <summary>
    /// Dependency Injection Registration
    /// </summary>
    public static class NimbleDependencyContainer
    {
        /// <summary>
        /// Registration ViewModel factories
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureServices(IServiceCollection services)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.DefinedTypes);
            var all = types.Where(t => t.IsClass && !t.IsAbstract).ToList();

            foreach (var type in all)
            {
                foreach (var i in type.GetInterfaces())
                {
                    if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityValidator<>))
                    {
                        var interfaceType = typeof(IEntityValidator<>).MakeGenericType(i.GetGenericArguments());
                        services.AddTransient(interfaceType, type);
                    }

                    if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IViewModelFactory<,>))
                    {
                        var interfaceType = typeof(IViewModelFactory<,>).MakeGenericType(i.GetGenericArguments());
                        services.AddTransient(interfaceType, type);
                    }

                    if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityManager<,,,>))
                    {
                        var interfaceType = typeof(IEntityManager<,,,>).MakeGenericType(i.GetGenericArguments());
                        services.AddTransient(interfaceType, type);
                        services.AddTransient(typeof(IEntityManager), type);
                    }
                }
            }

            services.AddTransient<IEntityManagerFactory, EntityManagerFactory>();
        }
    }
}
