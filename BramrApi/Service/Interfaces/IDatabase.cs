﻿using System.Threading.Tasks;
using System.Collections.Generic;
using BramrApi.Database.Data;
using NHibernate;
using BramrApi.Data;

namespace BramrApi.Service.Interfaces
{
    public interface IDatabase
    {
        public Task AddModel<T>(T model) where T : DatabaseModel;

        public Task UpdateModel<T>(T model) where T : DatabaseModel;

        public Task DeleteModel<T>(T model) where T : DatabaseModel;

        public Task DeleteModelById<T>(int id) where T : DatabaseModel;

        public bool UserNameExist(string username);

        public void SetConnectionString(string connection);

        public T GetModelById<T>(int id) where T : DatabaseModel;

        public UserProfile GetModelByUserName(string username);

        public FileModel GetFileModel(string username, string filename);

        public FileModel GetFileModelByUri(string uri);

        public List<T> GetAllModels<T>() where T : DatabaseModel;
    }
}
