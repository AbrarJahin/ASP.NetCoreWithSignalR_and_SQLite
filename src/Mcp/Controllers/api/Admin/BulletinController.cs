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
        public async Task<IActionResult> Insert(BulletinViewModel data, ICollection<IFormFile> image)
        {
            bool is_immediate_bulletin;
            List<Description> bulletinDescription   = new List<Description>();
            List<BulletinTime> bulletinTime         = new List<BulletinTime>();

            if(data.text.Length>0)
            {
                foreach (var description in data.text)
                {
                    bulletinDescription.Add(new Description { Text = description });
                }
            }

            if (data.send_time.Length>0)
            {
                is_immediate_bulletin = false;

                foreach (var send_time in data.send_time)
                {
                    bulletinTime.Add(new BulletinTime { SendTime = send_time });
                }
            }
            else
            {
                is_immediate_bulletin = true;
            }

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

        [HttpPut]
        public async Task<IActionResult> Update(BulletinViewModel data, ICollection<IFormFile> image)
        {
            List<Description> bulletinDescriptions = new List<Description>();
            List<BulletinTime> bulletinTime = new List<BulletinTime>();
            List<Image> bulletinImages = new List<Image>();

            string path = Directory.GetCurrentDirectory();

            var bulletin = _context.Bulletin
                .Include(u => u.Descriptions)
                .Include(u => u.Images)
                .Include(u => u.BulletinTimes)
                .Where(x=> x.Id ==data.id)
                .SingleOrDefault();

            //Single Data - Update
            bulletin.UserId = data.user_id;
            bulletin.CategoryName = data.category_name;
            bulletin.Title = data.title;

            //List data - remove all previous data
            bulletin.Descriptions.RemoveAll(x => x.BulletinId == data.id);
            //Delete all files from Server
            foreach (Image imageTodelete in bulletin.Images)
            {
                //File.Delete(imageTodelete.Name);
                FileInfo file = new FileInfo(uploadDirectory + $@"/{imageTodelete.Name}");
                file.Delete();  //As we surely know, file is there, if not, we should check that
            }
            bulletin.Images.RemoveAll(x=>x.BulletinId==data.id);
            bulletin.BulletinTimes.RemoveAll(x => x.BulletinId == data.id);

            //Update List data - if there is anything available
            if (data.text.Length > 0)
            {
                foreach (var description in data.text)
                {
                    bulletinDescriptions.Add(new Description { Text = description });
                }
            }

            if (data.send_time.Length > 0)
            {
                foreach (var send_time in data.send_time)
                {
                    bulletinTime.Add(new BulletinTime { SendTime = send_time });
                }
            }

            foreach (var file in image)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extention = Path.GetExtension(file.FileName);

                var filename = (data.user_id
                                + "_"
                                + Guid.NewGuid().ToString()
                                + "_"
                                + fileName
                                + extention).Trim('"');

                bulletinImages.Add(new Image { Name = filename });

                var serverFile = uploadDirectory + $@"/{filename}";
                //file.Length;
                using (FileStream fileStream = System.IO.File.Create(serverFile))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                }
            }

            //Update Data
            bulletin.Descriptions = bulletinDescriptions;
            bulletin.Images = bulletinImages;
            bulletin.BulletinTimes = bulletinTime;

            _context.Bulletin.Update(bulletin);
            _context.SaveChanges();

            return Ok(bulletin);
        }

        //[HttpDelete]
        //public Delete(int bulletinID)
        //{

        //}
    }
}