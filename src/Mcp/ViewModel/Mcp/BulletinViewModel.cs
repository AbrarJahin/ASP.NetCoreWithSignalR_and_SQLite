using Microsoft.AspNet.Http;
using System.Collections.Generic;

namespace McpSmyrilLine.Model.ViewModels
{
    public class BulletinViewModel
    {
        public int user_id { get; set; }
        public string title { get; set; }
        public string[] text { get; set; }
        public ICollection<IFormFile> images { get; set; }

        public BulletinViewModel()
        {
            images = new List<IFormFile>();
        }
    }
}
