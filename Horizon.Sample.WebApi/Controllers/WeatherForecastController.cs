using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizon.Core.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Horizon.Sample.WebApi.Controllers
{
    [ApiController]
    
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("api/steam1")]
        public IActionResult Get()
        {
            
            var tokenmodel = new TokenModel
            {
                Uid = Guid.NewGuid().ToString(),
                Project = "Horizon.Sample.WebApi",
                Role = "admin",
                TokenType = TokenType.Web,
                UserName = "admin",
                GrandType = "AccessToken"
            };

            return Ok(JwtHelper.BuildJwtToken(tokenmodel));


            //var rng = new Random();
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //})
            //.ToArray();
        }


       [Authorize]
        [Route("api/steam2")]
        public IActionResult Steam()
        {
            return Ok();
            //var tokenHeader = context.HttpContext.Request.Headers["Authorization"];
            //    tokenHeader = tokenHeader.ToString().Substring("Bearer ".Length).Trim();

            //    var tm = JwtHelper.SerializeJWT(tokenHeader);
            //    userGuid = tm.Uid;
            //}
            //var tm = JwtHelper.SerializeJWT(tokenHeader);


            return null;
            //var rng = new Random();
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //    {
            //        Date = DateTime.Now.AddDays(index),
            //        TemperatureC = rng.Next(-20, 55),
            //        Summary = Summaries[rng.Next(Summaries.Length)]
            //    })
            //    .ToArray();
        }
    }
}
