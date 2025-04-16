using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using GameTradeZone.Infrastructure.Configurations;
using GameTradeZone.Service.Configurations;
using GameTradeZone.Service.File;
using GameTradeZone.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using GameTradeZone.Infrastructure.Persistence;
using GameTradeZone.Services.Clients;
using GameTradeZone.Service.Services;
using GameTradeZone.Service.WebSoketHUB;
using GameTradeZone.Service.Hubs;

namespace GameTradeZone
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.WebHost.UseUrls("https://*:7232");
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.EnableDetailedErrors(); 
                options.EnableSensitiveDataLogging();
            });
            builder.Services.AddSignalR();
            builder.Services.AddControllers();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<SepayApiClient>();
            builder.Services.AddScoped<IRechargeBankService, RechargeBankService>();
            builder.Services.AddScoped<CloudinaryService>();
            builder.Services.AddScoped<IPostInfoService, PostInfoService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<IForumsCategoryService, ForumsService>();
            builder.Services.AddHttpContextAccessor();  

            // Configure Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GAMETRADEZONE 2025 WEB API",
                    Version = "v1",
                });
                options.OperationFilter<SwaggerFileOperationFilter>();
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                options.CustomSchemaIds(type => type.ToString());
            });

            // Configure CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyCorsPolicy",
                    policy => policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            // Register Application Services
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddScoped<FileUploadService>();

            var app = builder.Build();

            // Enable Swagger in development mode
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Serve static files
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(@"C:\Host\public"),
                RequestPath = "/public"
            });

            // Configure middleware pipeline
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAnyCorsPolicy");
            app.MapHub<TransactionHub>("/transactionHub");
            app.MapHub<AuctionHub>("/auctionHub");
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
