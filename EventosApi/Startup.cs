using AutoMapper;
using EventosApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using EventosApi.Mapper;

namespace EventosApi
{
 public class Startup { 
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
            
        {
            services.AddControllers().AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c=>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventosApi", Version = "v1" });
            });
            services.AddLogging();
            services.AddHostedService<EventoService>();
            services.AddAutoMapper(typeof(MapperCode));
            //services.AddSingleton<IMapper>(mapper => new Mapper(mapper.GetRequiredService<AutoMapper.IConfigurationProvider>(), mapper.GetService));
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        
    
    }

}