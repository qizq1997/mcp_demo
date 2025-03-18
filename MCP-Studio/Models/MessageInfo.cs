using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCP_Studio.Models
{
    public class MessageInfo
    {
        public string? Role { get; set; }
        public string? Text { get; set; }
        public string? FunctionCallInfo { get; set; }
        public string? FunctionCallResult { get; set; }

        public MessageInfo() 
        {

        }

        public MessageInfo(string Role2, string Text2)
        {
            Role = Role2;
            Text = Text2;
        }
    }
}
