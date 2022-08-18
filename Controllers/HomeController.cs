using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JSRunner.Controllers
{
	public class HomeController : Controller
	{
        private readonly IWebHostEnvironment env;

        public HomeController(IWebHostEnvironment env)
        {
            this.env = env;
        }

		public IActionResult Index() => View();

		public IActionResult SignOut()
        {
			return Unauthorized();
        }

        [HttpPost]
		public IActionResult CreateUser([FromQuery] string username, [FromQuery] string password)
        {
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
				User.Identity.Name != "dannys")
				return BadRequest();

			string filePath = Path.Combine(env.ContentRootPath, "ClientScript", username + ".pwd");

			if (System.IO.File.Exists(filePath))
				return BadRequest();

			System.IO.File.WriteAllText(filePath, username + ":" + password);
			return Ok();
		}
	}
}
