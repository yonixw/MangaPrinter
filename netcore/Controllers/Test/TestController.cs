using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;

namespace netcore.Controllers.Test
{
    //[ApiController]
    
    public class TestController: ControllerBase
    {
        public const string PNG_URL_PREFIX = "data:image/png;base64,";

        [Route("Home")]
        [Route("Home/Index")]
        [Route("Home/Index/{id?}")]
        [HttpGet]
        public string Get(int? id)
        {
            return "OK!" + id;
        }

        [Route("Home2")]
        [HttpGet]
        public string Get2(int? id)
        {
            return "OK2!" + id;
        }

        [Route("EmptyOval")]
        [HttpGet]
        public string EmptyOval()
        {
            using (var img = new MagickImage(MagickColors.Purple, 200, 200))
            {
                img.Format = MagickFormat.Png;
                return PNG_URL_PREFIX + img.ToBase64();
            }
        }

    }
}
