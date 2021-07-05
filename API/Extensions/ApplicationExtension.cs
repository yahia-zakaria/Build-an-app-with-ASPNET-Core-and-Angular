using API.ActionFilters;
using API.Data;
using API.Data.Repos;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace API.Extensions
{
    public static class ApplicationExtension
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
            services.AddDbContext<DataContext>(options =>{
            options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            services.AddCors();
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<LogUserActivity>();
            return services;
        }
    }
}