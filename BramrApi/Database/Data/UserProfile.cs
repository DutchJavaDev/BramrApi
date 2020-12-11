namespace BramrApi.Database.Data
{
    public class UserProfile : DatabaseModel
    {
        public virtual string UserName { get; set; }
        public virtual string WebsiteDirectory { get; set; }
        public virtual string ImageDirectory { get; set; }
    }
}
