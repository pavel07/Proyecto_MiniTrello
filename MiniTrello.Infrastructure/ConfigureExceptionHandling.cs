using System;
using Autofac;

namespace MiniTrello.Infrastructure
{
    public class ConfigureExceptionHandling : IBootstrapperTask
    {
        readonly ContainerBuilder _containerBuilder;

        public ConfigureExceptionHandling(ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
        }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}