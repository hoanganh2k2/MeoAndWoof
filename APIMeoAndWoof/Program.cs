using BusinessObject.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;

namespace APIMeoAndWoof
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddCors(option =>
            {
                option.AddDefaultPolicy(p =>
                        p.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<PostgresContext>(option =>
            {
                option.UseNpgsql(builder.Configuration.GetConnectionString("MeowWoofDb"));
            });
            builder.Services.AddControllers().AddOData(option => option.Select().Filter().Count().OrderBy().Expand());

            WebApplication app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}