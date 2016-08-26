using System;
using System.ComponentModel.DataAnnotations;

namespace McpSmyrilLine.DbModels.Mcp
{
    public class BulletinTime
    {
        public int Id { get; set; }
        public int BulletinId { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime SendTime { get; set; }

        //Setting Default value
        public BulletinTime()
        {
            SendTime = DateTime.Now;
        }

        //public virtual List<Bulletin> Bulletin { get; set; }
    }
}
