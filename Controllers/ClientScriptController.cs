using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace JSRunner.Controllers
{
	public class ClientScriptController : Controller
	{
		protected virtual string ScriptPath => ".\\ClientScript";
		protected virtual string FileExtension => "js";

		IHostEnvironment hostingEnvironment;

		public ClientScriptController(IHostEnvironment hostingEnvironment)
		{
			this.hostingEnvironment = hostingEnvironment;
		}

		string getScriptPath()
		{
			return Path.Combine(hostingEnvironment.ContentRootPath, ScriptPath, User.Identity.Name);
		}

		string getScriptFile(string key)
		{
			return Path.Combine(getScriptPath(), $"{key}.{FileExtension}");
		}

		public async virtual Task<IActionResult> List()
		{
			DirectoryInfo dir = new DirectoryInfo(getScriptPath());
			if (!dir.Exists)
				dir.Create();

			List<ClientScript> scripts = new List<ClientScript>();

			foreach (var f in dir.GetFiles($"*.{FileExtension}", SearchOption.TopDirectoryOnly))
			{
				var script = new ClientScript { Key = Path.GetFileNameWithoutExtension(f.FullName) };
				using (var fs = f.OpenRead())
				using (var reader = new StreamReader(fs))
				{
					script.Name = await reader.ReadLineAsync();
				}
				scripts.Add(script);
			}

			return Json(scripts);
		}

		public async virtual Task<IActionResult> Get(string key)
		{
			var f = new FileInfo(getScriptFile(key));
			var script = new ClientScript { Key = Path.GetFileNameWithoutExtension(f.FullName) };

			if (f.Exists)
			{
				using (var fs = f.OpenRead())
				using (var reader = new StreamReader(fs))
				{
					script.Name = await reader.ReadLineAsync();
					script.Script = await reader.ReadToEndAsync();
				}
			}

			return Json(script);
		}

		[HttpDelete]
		public virtual IActionResult Delete(string key)
		{
			var f = new FileInfo(getScriptFile(key));

			if (f.Exists)
			{
				f.Delete();
			}

			return Json(new OperationResult { Success = true });
		}

		[HttpPost]
		public async virtual Task<IActionResult> Save([FromBody] ClientScript model)
		{
			try
			{
				await System.IO.File.WriteAllTextAsync(getScriptFile(model.Key),
					string.Join(Environment.NewLine, model.Name, model.Script));
				return Json(new OperationResult { Success = true });
			}
			catch (Exception e)
			{
				return Json(new OperationResult { Success = true, Message = e.Message });
			}
		}
	}
}
