using BramrApi.Database.Data;
using FluentNHibernate.Mapping;

namespace BramrApi.Database
{
    public class AutoMapper : ClassMap<Auto>
    {
        public AutoMapper()
        {
            Table("auto_table");
            Id(model => model.Id);
            Map(model => model.Jaar);
            Map(model => model.Prijs);
            Map(model => model.Merk);
            Map(model => model.CreationDate);
            Map(model => model.BouwDatum);
        }
    }
}
