using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using StudentManagementBackend.Entities;
using StudentManagementBackend.Repositories;
using StudentManagementBackend.Utilities;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:8000", "http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});


builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("StudentManagementDb"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<AttendanceLogRepository>();

var app = builder.Build();
app.UseCors("CorsPolicy");
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), $@"{builder.Configuration.GetValue<string>("UploadsFolder")}")),
    RequestPath = new PathString("/Uploads")
});

app.UseAuthorization();

app.MapControllers();

app.Run();
