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
            Map(m => m.CvDirectory);
            Map(m => m.PortfolioDirectory);
            Map(m => m.IndexCvDirectory);
            Map(m => m.IndexPortfolioDirectory);
            Map(m => m.ImageCvDirectory);
            Map(m => m.ImagePortfolioDirectory);
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

    public class TextModelMapper : ClassMap<TextModel>
    {
        public TextModelMapper()
        {
            Table("textmodels_table");
            Id(e => e.Id);
            Map(e => e.CreationDate);
            Map(e => e.UserName);
            Map(e => e.Location);
            Map(e => e.Text);
            Map(e => e.TextColor);
            Map(e => e.BackgroundColor);
            Map(e => e.Bold);
            Map(e => e.Italic);
            Map(e => e.Underlined);
            Map(e => e.Strikedthrough);
            Map(e => e.TextAllignment);
            Map(e => e.Fontsize);
            Map(e => e.TemplateType);
        }
    }

    public class ImageModelMapper : ClassMap<ImageModel>
    {
        public ImageModelMapper()
        {
            Table("imagemodels_table");
            Id(e => e.Id);
            Map(e => e.CreationDate);
            Map(e => e.UserName);
            Map(e => e.FileUri);
            Map(e => e.Location);
            Map(e => e.Width);
            Map(e => e.Height);
            Map(e => e.Alt);
            Map(e => e.Border);
            Map(e => e.FloatSet);
            Map(e => e.Opacity);
            Map(e => e.ObjectFitSet);
            Map(e => e.Padding);
            Map(e => e.TemplateType);
        }
    }
}
