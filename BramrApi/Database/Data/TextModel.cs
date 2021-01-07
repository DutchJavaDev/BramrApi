using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BramrApi.Database.Data
{
    public class TextModel : DatabaseModel
    {
        [Required]
        public virtual string UserName { get; set; }
        [Required]
        public virtual int Location { get; set; }
        [Required]
        public virtual string Text { get; set; }
        [Required]
        public virtual string TextColor { get; set; }
        [Required]
        public virtual string BackgroundColor { get; set; }
        [Required]
        public virtual bool Bold { get; set; }
        [Required]
        public virtual bool Italic { get; set; }
        [Required]
        public virtual bool Underlined { get; set; }
        [Required]
        public virtual bool Strikedthrough { get; set; }
        [Required]
        public virtual string TextAllignment { get; set; }
        [Required]
        public virtual double Fontsize { get; set; }
        [Required]
        public virtual string TemplateType { get; set; }
    }
}
