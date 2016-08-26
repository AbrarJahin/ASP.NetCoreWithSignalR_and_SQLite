using System.Collections.Generic;

namespace BugTracker.DbModels.Try
{
    public class User
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<Post> Posts { get; set; }
    }
}
