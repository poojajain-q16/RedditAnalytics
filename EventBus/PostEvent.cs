using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EventBus
{
    public class PostEvent
    {
        public Post redditPost { get; set; }
    }
}
