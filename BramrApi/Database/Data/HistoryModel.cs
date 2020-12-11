using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BramrApi.Database.Data
{
    public class HistoryModel : DatabaseModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public int Location { get; set; }

        [Required]
        public int DesignElement { get; set; }

        [Required]
        public string EditType { get; set; }

        [Required]
        public string Edit { get; set; }
    }
}
