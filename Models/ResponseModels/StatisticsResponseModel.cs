using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ResponseModels
{
    public class StatisticsResponseModel
    {
        public List<PostResponseModel> PostsWithMostUpVotes { get; set; }

        public List<UserResponseModel> UsersWithMostPosts { get; set; }
    }
}
