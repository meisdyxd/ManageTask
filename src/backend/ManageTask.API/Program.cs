using ManageTask.API.Extensions;
using ManageTask.Application.ServiceRegistration;
using ManageTask.Infrastructure.ServiceRegistration;
using ManageTask.Infrastructure.SignalR;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddCustomSwaggerGen();

services.AddAuthentificationRules(configuration);

services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
              .WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("Authorization", "Refresh-Token");
    });
});

services
    .AddApplication()
    .AddInfrastructure(configuration);

var app = builder.Build();

app.UseResultSharpLogging();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
//app.MapHub<TaskHub>("/taskHub");
app.MapControllers();
app.Run();
