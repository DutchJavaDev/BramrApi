namespace BramrApi.Database.Data
{
    public class UserProfile : DatabaseModel
    {
        public virtual string UserName { get; set; }
        public virtual string WebsiteDirectory { get; set; }
        public virtual string CvDirectory { get; set; }
        public virtual string PortfolioDirectory { get; set; }
        public virtual string IndexCvDirectory { get; set; }
        public virtual string IndexPortfolioDirectory { get; set; }
        public virtual string ImageCvDirectory { get; set; }
        public virtual string ImagePortfolioDirectory { get; set; }
        public virtual bool HasCv { get; set; }
        public virtual bool HasPortfolio { get; set; }

    }
}
