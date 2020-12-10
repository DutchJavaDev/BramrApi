using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BramrApi.Database.Data;
using BramrApi.Service.Interfaces;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using System.Linq;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace BramrApi.Service
{
    public class DatabaseService : IDatabase
    {
        private readonly ISessionFactory SessionFactory;

#if DEBUG
        private string ConnectionString = "server=localhost;port=3306;database=bramr_db;uid=bramr_db;password=";
#else
        private string ConnectionString = "server=localhost;port=3306;database=bramr_db;uid=dbuser;password=MQDB23@s34!";
#endif
        public DatabaseService()
        {
            SessionFactory = GetSessionFactory();
        }

        public bool UserNameExist(string username)
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            return session.Query<UserProfile>().Count(i => i.UserName == username) > 0;
        }

        public async Task AddModel<T>(T model) where T : DatabaseModel
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            model.CreationDate = DateTime.Now;
            await session.SaveOrUpdateAsync(model);
            await transaction.CommitAsync();
        }

        public async Task DeleteModel<T>(T model) where T : DatabaseModel
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            await session.DeleteAsync(model);
            await transaction.CommitAsync();
        }

        public async Task DeleteModelById<T>(int id) where T : DatabaseModel
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            await session.DeleteAsync(session.Query<DatabaseModel>().Where(m => m.Id == id).FirstOrDefault());
            await transaction.CommitAsync();
        }

        public List<T> GetAllModels<T>() where T : DatabaseModel
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            return session.Query<T>().ToList();
        }

        public T GetModelById<T>(int id) where T : DatabaseModel
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            return session.Query<T>().Where(m => m.Id == id).FirstOrDefault();
        }

        public void SetConnectionString(string connection)
        {
            ConnectionString = connection;
        }

        public async Task UpdateModel<T>(T model) where T : DatabaseModel
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            model.CreationDate = DateTime.Now;
            await session.SaveOrUpdateAsync(model);
            await transaction.CommitAsync();
        }

        private ISessionFactory GetSessionFactory()
        {
            return Fluently
                   .Configure()
                   .Database(MySQLConfiguration.Standard.ConnectionString(ConnectionString))
                   .Mappings(m => m.FluentMappings.AddFromAssemblyOf<DatabaseModel>())
                   .ExposeConfiguration(config => new SchemaUpdate(config).Execute(true, true))
                   .BuildConfiguration()
                   .BuildSessionFactory();
        }
    }
}
