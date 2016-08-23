using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.DbModels.Mcp
{
    public class Description
    {
        public int Id { get; set; }
        public int BulletinId { get; set; }
        public string Text { get; set; }
        public Bulletin Bulletin { get; set; }
    }
}
