using System.Threading.Tasks;
using System.Collections.Generic;
using BramrApi.Database.Data;
using NHibernate;
using BramrApi.Data;

namespace BramrApi.Service.Interfaces
{
    public interface IDatabase
    {
        public Task AddOrUpdateModel<T>(T model) where T : DatabaseModel;

        public Task UpdateModel<T>(T model) where T : DatabaseModel;

        public Task DeleteModel<T>(T model) where T : DatabaseModel;

        public Task DeleteModelById<T>(int id) where T : DatabaseModel;

        public bool UserNameExist(string username);

        public void SetConnectionString(string connection);

        public T GetModelById<T>(int id) where T : DatabaseModel;

        public UserProfile GetModelByUserName(string username);

        public T GetModelByIdentity<T>(string identity) where T : DatabaseModel;

        public FileModel GetFileModel(string username, string filename);

        public FileModel GetFileModelByUri(string uri);
        public FileModel GetFileModelByPath(string path);
        public Task DeleteFileModelByPath(string path);

        public HistoryModel GetHistoryModel(string username, int location);

        public Task DeleteAllHistoryModelsByUsername(string username);

        public Task DeleteAllHistoryModelsFromLocationByUsername(string username, int location);

        public Task DeleteAllTextAndImageModelsByUsername(string username);

        public List<TextModel> GetAllTextModelsByUsername(string username);

        public List<ImageModel> GetAllImageModelsByUsername(string username);

        public List<object> GetAllDesignElementsByUsername(string username);

        public List<T> GetAllModels<T>() where T : DatabaseModel;
    }
}
