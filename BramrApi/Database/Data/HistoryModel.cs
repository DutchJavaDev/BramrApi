using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BramrApi.Database.Data
{
    public class HistoryModel : DatabaseModel
    {
        public virtual string UserName { get; set; }

        [Required]
        public virtual int Location { get; set; }

        [Required]
        public virtual int DesignElement { get; set; }

        [Required]
        public virtual string EditType { get; set; }

        [Required]
        public virtual string Edit { get; set; }
    }
}
