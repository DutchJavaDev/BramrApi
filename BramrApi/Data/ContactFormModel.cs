using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BramrApi.Data
{
    public class ContactFormModel
    {
        public string sendersName {get;set;}
        public string sendersEmail { get; set;}
        public string recipientUsername { get; set; }
        public string service { get; set; }
        public string message { get; set; }
    }
}
