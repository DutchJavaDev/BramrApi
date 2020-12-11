using BramrApi.Data;
﻿using BramrApi.Database.Data;
using FluentNHibernate.Mapping;

namespace BramrApi.Database
{
    public class UserProfileMapper : ClassMap<UserProfile>
    {
        public UserProfileMapper()
        {
            Table("userprofile_table");
            Id(m => m.Id);
            Map(m => m.CreationDate);
            Map(m => m.Identity);
            Map(m => m.UserName);
            Map(m => m.WebsiteDirectory);
            Map(m => m.ImageDirectory);
        }
    }

    public class FileModelMapper : ClassMap<FileModel>
    {
        public FileModelMapper()
        {
            Table("file_table");
            Id(e => e.Id);
            Map(e => e.CreationDate);
            Map(e => e.UserName);
            Map(e => e.FileName);
            Map(e => e.FilePath);
            Map(e => e.FileUri);
        }
    }

    public class EditHistoryModelMapper : ClassMap<HistoryModel>
    {
        public EditHistoryModelMapper()
        {
            Table("edithistory_table");
            Id(e => e.Id);
            Map(e => e.CreationDate);
            Map(e => e.UserName);
            Map(e => e.Location);
            Map(e => e.DesignElement);
            Map(e => e.EditType);
            Map(e => e.Edit);
        }
    }
}
