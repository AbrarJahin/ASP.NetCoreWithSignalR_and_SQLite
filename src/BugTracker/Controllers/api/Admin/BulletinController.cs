﻿using Microsoft.AspNetCore.Mvc;
using BugTracker.DbModels.Mcp;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BugTracker.Model.ViewModels;
using Microsoft.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;

namespace BugTracker.api
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
        public async Task<IActionResult> Get(int start=0,int end=0)
        {
            var bulletins = await _context.Bulletin
                .Include(u => u.Descriptions)
                .Include(u => u.Images)
                .ToArrayAsync();

            var response = bulletins.Select(u => new
            {
                Id = u.Id,
                UserId = u.UserId,
                Title = u.Title,
                Descriptions = u.Descriptions.Select(p => p.Text),
                Images = u.Images.Select(p => p.Name),
                BaseUrl = "http:/localhost/uploads/"
            });

            return Ok(response);
        }

        [HttpPost]
        public IActionResult Insert(BulletinViewModel data, ICollection<IFormFile> image)
        {
            List<Description> bulletinDescription = new List<Description>();

            foreach (var description in data.text)
            {
                bulletinDescription.Add(new Description{ Text = description });
            }

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
                Descriptions = bulletinDescription,
                Images = bulletinImages
            };

            _context.Bulletin.Add(bulletin);
            _context.SaveChanges();
            
            return Ok(bulletin);
        }

        //[HttpPost]
        //public async Task<IActionResult> Insert(IFormCollection data)
        //{
        //    ICollection<Description> bulletinDescription = null;
        //    ICollection<Image> bulletinImage = null;
        //    //Description
        //    Bulletin savableBulletin = new Bulletin();
        //    foreach (var description in data)
        //    {
        //        if (description.Key.Equals("user_id"))
        //        {
        //            System.Pa
        //            savableBulletin.UserId = description.Value;
        //        }
        //    }
        //    //Images
        //    foreach (string image in data["image"])
        //    {
        //        Image bul_image = new Image { Name = "asd" };
        //        bulletinImage.Add(bul_image);
        //    }

        //    /*
        //    Bulletin bultn = new Bulletin { UserId = , Title =  };
        //    _context.Bulletin.Add();
        //    await _context.SaveChangesAsync();
        //    */
        //    return Ok(data);
        //}

        //[HttpPut]
        //public Update(int bulletinID)
        //{

        //}

        //[HttpDelete]
        //public Delete(int bulletinID)
        //{

        //}
    }
}