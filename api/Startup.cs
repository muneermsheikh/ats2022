using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using api.Extensions;
using api.Helpers;
using api.Middleware;
using core.Entities.Identity;
using infra.Data;
using infra.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;


namespace api
{
    public class Startup
    {
        private readonly IConfiguration _config;
          private readonly IWebHostEnvironment _env;
          public Startup(IConfiguration config, IWebHostEnvironment env)
          {
               _env = env;
               _config = config;
          }

          //public IConfiguration Configuration { get; }

          // This method gets called by the runtime. Use this method to add services to the container.
          public void ConfigureServices(IServiceCollection services)
          {

               services.AddAutoMapper(typeof(MappingProfiles));
               services.AddControllers();
               services.AddApplicationServices();
               services.AddIdentityServices(_config);

               /*
               if (_env.IsDevelopment()) {
                    services.AddDbContext<ATSContext>(x => x.UseSqlite(
                         _config.GetConnectionString("DefaultConnection")));
                    services.AddDbContext<AppIdentityDbContext>(x =>
                         { x.UseSqlite(_config.GetConnectionString("IdentityConnection")); });
               } else {
                    services.AddDbContext<SQLDbContext>(x => x.UseSqlServer(
                         _config.GetConnectionString("SQLServerConnection")));
                    
                    services.AddDbContext<AppIdentityDbContext>(x =>
                         { x.UseSqlServer(_config.GetConnectionString("IdentityConnection")); });
                }
               */
               services.AddDbContext<ATSContext>(x => x.UseSqlServer(
                    _config.GetConnectionString("SQLServerConnection")));
               //services.AddDbContext<AppIdentityDbContext>(x =>
                         //{ x.UseSqlite(_config.GetConnectionString("IdentityConnection")); });
               services.AddDbContext<AppIdentityDbContext>(x =>
                         { x.UseSqlServer(_config.GetConnectionString("IdentityConnection")); });

               services.AddSwaggerGen(c =>
               {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "api", Version = "v1" });
               });

               //services.AddSwaggerDocumentation();

               services.AddCors(opt =>
               {
                    opt.AddPolicy("CorsPolicy", policy =>
                    {
                         policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                    });
               });
               
               services.Configure<MailSettings>(_config.GetSection("MailSettings"));
          }

          // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
          //order in this procedure is imp
          //comment out line 85 gives error in roles
          public void Configure(IApplicationBuilder app, IWebHostEnvironment env
               , DbContextOptions<AppIdentityDbContext> identityDbContextOptions, UserManager<AppUser> userManager
               //, RoleManager<IdentityRole<int>> roleManager
               )
          {
                app.UseMiddleware<ExceptionMiddleware>();
               if (env.IsDevelopment())
               {
                    //app.UseDeveloperExceptionPage();
                    app.UseSwagger();
                    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1"));
               }

               app.UseHttpsRedirection();

               app.UseRouting();

               app.UseCors("CorsPolicy");
               
               app.UseAuthentication();
               app.UseAuthorization();

               app.UseSwagger();
               app.UseSwaggerUI(c =>
               {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ATS API V1");
               });

               app.UseEndpoints(endpoints =>
               {
                    endpoints.MapControllers();
               });
          }
    }
}
