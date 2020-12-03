using System;

namespace BramrApi.Database.Data
{
    public class Auto : DatabaseModel
    {
        public virtual string Merk { get; set; }
        public virtual double Prijs { get; set; }
        public virtual int Jaar { get; set; }
        public virtual DateTime BouwDatum { get; set; }
    }
}
