using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public class ErrorHandler: EventArgs
    {
        public string message { private set; get; }
        public ErrorHandler(string _message)
        {
            message = _message;
        }
    }
}
