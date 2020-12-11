using System;

namespace BramrApi.Database.Data
{
    public class DatabaseModel
    {
        public virtual int Id { get; set; }
        
        public virtual string Identity { get; set; }

        public virtual DateTime CreationDate { get; set; }
    }
}
