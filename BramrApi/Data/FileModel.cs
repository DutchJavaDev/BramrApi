using BramrApi.Database.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BramrApi.Data
{
    public class FileModel : DatabaseModel
    {
        [Required]
        public virtual string UserName { get; set; }

        [Required]
        public virtual string FilePath { get; set; }

        [Required]
        public virtual string FileName { get; set; }

        [Required]
        public virtual string FileUri { get; set; }

        public virtual string CreateUri()
        {
            string Uri = Guid.NewGuid().ToString().Replace("-", "");
            return Uri;
        }
    }
}
