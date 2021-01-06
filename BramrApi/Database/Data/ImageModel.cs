using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BramrApi.Database.Data
{
    public class ImageModel : DatabaseModel
    {
        [Required]
        public virtual string UserName { get; set; }
        [Required]
        public virtual string FileUri { get; set; }
        [Required]
        public virtual int Location { get; set; }
        [Required]
        public virtual int Width { get; set; }
        [Required]
        public virtual int Height { get; set; }
        [Required]
        public virtual string Alt { get; set; }
        [Required]
        public virtual int Border { get; set; }
        [Required]
        public virtual string FloatSet { get; set; }
        [Required]
        public virtual double Opacity { get; set; }
        [Required]
        public virtual string ObjectFitSet { get; set; }
        [Required]
        public virtual int Padding { get; set; }
        [Required]
        public virtual string TemplateType { get; set; }
    }
}
