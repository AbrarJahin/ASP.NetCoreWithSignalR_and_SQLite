using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.DbModels.Mcp
{
    public class Bulletin
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserId { get; set; }

        //public User User { get; set; }

        public string Content { get; set; }
    }
}
