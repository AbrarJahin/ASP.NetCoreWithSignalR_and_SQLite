using System;
using System.ComponentModel.DataAnnotations;

namespace McpSmyrilLine.DbModels.Mcp
{
    public class BulletinTime
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        //public virtual List<Bulletin> Bulletin { get; set; }
    }
}
