using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Params
{
    public class MessageParams: ParamPages
    {
        public string Username { get; set; }
        public string Container { get; set; }="Inbox";
    }
}
