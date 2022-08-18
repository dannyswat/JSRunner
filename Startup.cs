using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSRunner
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();

			app.UseRouting();

			app.Use(async (context, next) =>
			{
				static async Task returnChallenge(HttpContext httpContext)
				{
					httpContext.Response.StatusCode = 401;
					httpContext.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Password protected\"");
					await Task.CompletedTask;
				}

				if (!context.Request.Headers.ContainsKey("Authorization"))
				{
					await returnChallenge(context);
					return;
				}

				var basic = context.Request.Headers["Authorization"].ToString();

				if (!basic.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
				{
					await returnChallenge(context);
					return;
				}

				basic = Encoding.ASCII.GetString(Convert.FromBase64String(basic.Substring(6)));

				string username = basic.Split(':')[0];
				string passwordFile = Path.Combine(env.ContentRootPath, "ClientScript", username + ".pwd");
				if (!File.Exists(passwordFile))
                {
					await returnChallenge(context);
					return;
				}

				if (basic != System.IO.File.ReadAllText(passwordFile))
				{
					await returnChallenge(context);
					return;
				}

				context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Principal.GenericIdentity(username));

				await next();
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
