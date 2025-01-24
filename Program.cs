using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StudentsMinimalApi.Data;
using StudentsMinimalApi.Models;
using StudentsMinimalApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var connStr = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<SchoolDbContext>(option => option.UseSqlite(connStr));

// Add Cors
builder.Services.AddCors(o => o.AddPolicy("Policy", builder => {
  builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
}));

var app = builder.Build();

app.UseCors("Policy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
//     app.UseSwaggerUI(options =>  {
//     options.SwaggerEndpoint("/openapi/v1.json", "My WebAPI");
// });
app.UseHttpsRedirection(); 

app.MapScalarApiReference(options => {
    options
        .WithTitle("My WebAPI")
        .WithTheme(ScalarTheme.Mars)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});
}
var studentsRoute = app.MapGroup("/api/students");

studentsRoute.MapGet("/", StudentService.GetAllStudents);
studentsRoute.MapGet("/school/{school}", StudentService.GetStudentsBySchool);
studentsRoute.MapGet("/{id}", StudentService.GetStudent);
studentsRoute.MapPost("/", StudentService.CreateSttudent);
studentsRoute.MapPut("/{id}", StudentService.UpdateStudent);
studentsRoute.MapDelete("/{id}", StudentService.DeleteStudent);

using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<SchoolDbContext>();    
    context.Database.Migrate();
}

app.Run();


