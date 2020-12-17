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
using BramrApi.Data;

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

        public async Task AddOrUpdateModel<T>(T model) where T : DatabaseModel
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            model.CreationDate = DateTime.Now;
            await session.SaveOrUpdateAsync(model);
            await transaction.CommitAsync();
        }

        public async Task UpdateModel<T>(T model) where T : DatabaseModel
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

        public T GetModelByIdentity<T>(string identity) where T : DatabaseModel
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            return session.Query<T>().Where(m => m.Identity == identity).FirstOrDefault();
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

        public UserProfile GetModelByUserName (string username)
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            return session.Query<UserProfile>().Where(m => m.UserName == username).FirstOrDefault();
        }

        public FileModel GetFileModel(string username, string filename)
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            return session.Query<FileModel>().Where(m => m.UserName == username).Where(e => e.FileName == filename).FirstOrDefault();
        }

        public FileModel GetFileModelByUri(string uri)
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            return session.Query<FileModel>().Where(m => m.FileUri == uri).FirstOrDefault();
        }

        public FileModel GetFileModelByPath(string path)
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            return session.Query<FileModel>().Where(m => m.FilePath == path).FirstOrDefault();
        }

        public async Task DeleteFileModelByPath(string path)
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            await session.DeleteAsync(session.Query<FileModel>().Where(m => m.FilePath == path).FirstOrDefault());
            await transaction.CommitAsync();
        }

        public HistoryModel GetHistoryModel(string username, int location)
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            return session.Query<HistoryModel>().Where(m => m.UserName == username).Where(e => e.Location == location).FirstOrDefault();
        }

        public async Task DeleteAllHistoryModelsByUsername(string username)
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            var items = session.Query<HistoryModel>().Where(m => m.UserName == username).ToList();
            foreach (var item in items)
            {
                await session.DeleteAsync(item);
            }
            await transaction.CommitAsync();
        }

        public async Task DeleteAllHistoryModelsFromLocationByUsername(string username, int location)
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            var items = session.Query<HistoryModel>().Where(m => m.UserName == username).Where(e => e.Location > location).ToList();
            foreach (var item in items)
            {
                await session.DeleteAsync(item);
            }   
            await transaction.CommitAsync();
        }

        public async Task DeleteAllTextAndImageModelsByUsername(string username)
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            var textmodels = session.Query<TextModel>().Where(m => m.UserName == username).ToList();
            var imagemodels = session.Query<ImageModel>().Where(m => m.UserName == username).ToList();
            foreach (var item in textmodels)
            {
                await session.DeleteAsync(item);
            }
            foreach (var item in imagemodels)
            {
                await session.DeleteAsync(item);
            }
            await transaction.CommitAsync();
        }

        public List<TextModel> GetAllTextModelsByUsername(string username)
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            return session.Query<TextModel>().Where(m => m.UserName == username).ToList();
        }

        public List<ImageModel> GetAllImageModelsByUsername(string username)
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            return session.Query<ImageModel>().Where(m => m.UserName == username).ToList();
        }

        public List<object> GetAllDesignElementsByUsername(string username)
        {
            using ISession session = SessionFactory.OpenSession();
            using ITransaction transaction = session.BeginTransaction();
            List<TextModel> textelements = session.Query<TextModel>().Where(m => m.UserName == username).ToList();
            List<ImageModel> images =  session.Query<ImageModel>().Where(m => m.UserName == username).ToList();
            List<object> AllElements = new List<object>();
            foreach (var item in textelements)
            {
                AllElements.Add(item);
            }
            foreach (var item in images)
            {
                AllElements.Add(item);
            }
            return AllElements;
        }

        public void SetConnectionString(string connection)
        {
            ConnectionString = connection;
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
