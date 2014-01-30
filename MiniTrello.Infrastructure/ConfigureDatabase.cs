using System;
using Autofac;

namespace MiniTrello.Infrastructure
{
    public class ConfigureDatabase : IBootstrapperTask
    {
        readonly ContainerBuilder _containerBuilder;

        public ConfigureDatabase(ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
        }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}