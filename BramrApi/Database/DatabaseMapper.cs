using BramrApi.Data;
﻿using BramrApi.Database.Data;
using FluentNHibernate.Mapping;

namespace BramrApi.Database
{
    public class FileModelMapper : ClassMap<FileModel>
    {
        public FileModelMapper()
        {
            Table("file_table");
            Id(e => e.Id);
            Map(e => e.UserName);
            Map(e => e.FileName);
            Map(e => e.FilePath);
            Map(e => e.FileUri);
        }
    }
    public class UserProfileMapper : ClassMap<UserProfile>
    {
        public UserProfileMapper()
        {
            Table("userprofile_table");
            Id(m => m.Id);
            Map(m => m.Identity);
            Map(m => m.CreationDate);
            Map(m => m.UserName);
            Map(m => m.WebsiteDirectory);
            Map(m => m.ImageDirectory);
        }
    }
}
