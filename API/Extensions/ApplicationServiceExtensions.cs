namespace API.Extensions;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt => 
        {
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRespository>();
        services.AddScoped<ILikesRepository, LikesRepository>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<IMessageRespository, MessageRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<LogUserActivity>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();
        return services;
    }
}