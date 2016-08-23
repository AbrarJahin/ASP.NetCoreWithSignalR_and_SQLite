using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BugTracker.DbModels.Mcp
{
    public class Bulletin
    {
        
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public List<Description> Descriptions { get; set; }
        public List<Image> Images { get; set; }
    }
}
