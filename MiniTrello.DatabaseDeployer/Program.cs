﻿using System;
using System.Collections.Generic;
using System.Threading;
using AcklenAvenue.Data.NHibernate;
using DomainDrivenDatabaseDeployer;
using FluentNHibernate.Cfg.Db;
using MiniTrello.Data;
using NHibernate;

namespace MiniTrello.DatabaseDeployer
{
    class Program
    {
        static void Main(string[] args)
        {
            MsSqlConfiguration databaseConfiguration = MsSqlConfiguration.MsSql2008.ShowSql().
                ConnectionString(x => x.FromConnectionStringWithKey("MiniTrello.Remote"));

            DomainDrivenDatabaseDeployer.DatabaseDeployer dd = null;
            ISessionFactory sessionFactory = new SessionFactoryBuilder(new MappingScheme(), databaseConfiguration)
                .Build(cfg => { dd = new DomainDrivenDatabaseDeployer.DatabaseDeployer(cfg); });

            dd.Drop();
            Console.WriteLine("Database dropped.");
            Thread.Sleep(1000);

            dd.Create();
            Console.WriteLine("Database created.");

            ISession session = sessionFactory.OpenSession();
            using (ITransaction tx = session.BeginTransaction())
            {
                dd.Seed(new List<IDataSeeder>
                {
                    new AccountSeeder(session)
                });
                tx.Commit();
            }
            session.Close();
            sessionFactory.Close();
            Console.WriteLine("Seed data added.");
            Thread.Sleep(5000);
        }
    }
}