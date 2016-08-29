using Microsoft.AspNetCore.Mvc;
using McpSmyrilLine.DbModels.Mcp;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using McpSmyrilLine.Model.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;

namespace McpSmyrilLine.api
{
    [Route("api/bulletin/[action]")]
    public class BulletinController : Controller
    {
        private readonly McpDbContext _context;
        private IHostingEnvironment _environment;
        private string uploadDirectory;

        public BulletinController(McpDbContext context, IHostingEnvironment environment) //, IHostingEnvironment environment
        {
            _context = context;
            _environment = environment;
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            uploadDirectory = _environment.WebRootPath + $@"/{"uploads"}";
            Directory.CreateDirectory(uploadDirectory);      //Should be in startup
        }

        [HttpGet]
        public async Task<IActionResult> Get(int currentPageNo = 1,int pageSize =20)
        {
            var bulletins = await _context.Bulletin
                .Include(u => u.Descriptions)
                .Include(u => u.Images)
                .Include(u => u.BulletinTimes)
                .Skip((currentPageNo-1)*pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            var response = bulletins.Select(u => new
            {
                Id = u.Id,
                UserId = u.UserId,
                Title = u.Title,
                CategoryName = u.CategoryName,
                Descriptions = u.Descriptions.Select(p => p.Text),
                Images = u.Images.Select(p => p.Name),
                SendTimes = u.BulletinTimes.Select(p => p.SendTime),
                BaseUrl = "/uploads/"
            });

            return Ok(response);
        }

        [HttpPost]
        public IActionResult Insert(BulletinViewModel data, ICollection<IFormFile> image)
        {
            bool is_immediate_bulletin;
            List<Description> bulletinDescription   = new List<Description>();
            List<BulletinTime> bulletinTime         = new List<BulletinTime>();

            foreach (var description in data.text)
            {
                bulletinDescription.Add(new Description { Text = description });
            }

            if(data.send_time.Length>0)
            {
                is_immediate_bulletin = false;
            }
            else
            {
                is_immediate_bulletin = true;
            }

            foreach (var send_time in data.send_time)
            {
                bulletinTime.Add(new BulletinTime { SendTime = send_time });
            }
            //if "data.send_time" contains 0 element, then the bulletin should be sent immediately,
            //wtherwise will be sent by times mentioned

            //string filename1 = _environment.WebRootPath;

            List<Image> bulletinImages = new List<Image>();
            string path = Directory.GetCurrentDirectory();

            foreach (var file in image)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extention = Path.GetExtension(file.FileName);

                var filename = ( data.user_id
                                + "_"
                                + Guid.NewGuid().ToString()
                                + "_"
                                + fileName
                                +extention).Trim('"');

                bulletinImages.Add(new Image { Name = filename });

                var serverFile = uploadDirectory + $@"/{filename}";
                //file.Length;
                using (FileStream fileStream = System.IO.File.Create(serverFile))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                }
            }

            Bulletin bulletin = new Bulletin {
                UserId = data.user_id,
                Title = data.title,
                CategoryName = data.category_name,
                Descriptions = bulletinDescription,
                Images = bulletinImages,
                BulletinTimes= bulletinTime
            };

            _context.Bulletin.Add(bulletin);
            _context.SaveChanges();

            if(is_immediate_bulletin)
            {
                //Broadcust current Bulletin
            }
            
            return Ok(bulletin);
        }

        //[HttpPut]
        //public IActionResult Update(BulletinViewModel data, ICollection<IFormFile> image)
        //{
        //    //db.People.RemoveRange(db.People.Where(x => x.State == "CA"));
        //    //db.SaveChanges();
        //    using (var db = new BloggingContext())
        //    {
        //        var blog = db.Blogs.Include(b => b.Posts).First();
        //        db.Remove(blog);
        //        db.SaveChanges();
        //    }

        //    List<Description> bulletinDescription = new List<Description>();
        //    List<BulletinTime> bulletinTime = new List<BulletinTime>();

        //    foreach (var description in data.text)
        //    {
        //        bulletinDescription.Add(new Description { Text = description });
        //    }

        //    foreach (var send_time in data.send_time)
        //    {
        //        bulletinTime.Add(new BulletinTime { SendTime = send_time });
        //    }
        //    //if "data.send_time" contains 0 element, then the bulletin should be sent immediately,
        //    //wtherwise will be sent by times mentioned

        //    //string filename1 = _environment.WebRootPath;

        //    List<Image> bulletinImages = new List<Image>();
        //    string path = Directory.GetCurrentDirectory();

        //    foreach (var file in image)
        //    {
        //        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
        //        string extention = Path.GetExtension(file.FileName);

        //        var filename = (data.user_id
        //                        + "_"
        //                        + Guid.NewGuid().ToString()
        //                        + "_"
        //                        + fileName
        //                        + extention).Trim('"');

        //        bulletinImages.Add(new Image { Name = filename });

        //        var serverFile = uploadDirectory + $@"/{filename}";
        //        //file.Length;
        //        using (FileStream fileStream = System.IO.File.Create(serverFile))
        //        {
        //            file.CopyTo(fileStream);
        //            fileStream.Flush();
        //        }
        //    }

        //    Bulletin bulletin = new Bulletin
        //    {
        //        UserId          = data.user_id,
        //        Title           = data.title,
        //        Descriptions    = bulletinDescription,
        //        Images          = bulletinImages,
        //        BulletinTimes   = bulletinTime
        //    };

        //    _context.Bulletin.Add(bulletin);
        //    _context.SaveChanges();

        //    return Ok(bulletin);
        //}

        //[HttpDelete]
        //public Delete(int bulletinID)
        //{

        //}
    }
}