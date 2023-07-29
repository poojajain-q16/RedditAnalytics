using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MessageStatusModel
    {
        //
        // Summary:
        //     Gets or sets the code related to message responses
        public string? ResponseCode { get; set; }

        //
        // Summary:
        //     Gets or sets the description related to the status code in a human readable format
        public string? Description { get; set; }
    }
}
