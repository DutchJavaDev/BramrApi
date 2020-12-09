using BramrApi.Data;
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
}
