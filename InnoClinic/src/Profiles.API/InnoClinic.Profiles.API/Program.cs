using InnoClinic.Profiles.API.Application.Extensions;
using InnoClinic.Profiles.API.Extensions;
using InnoClinic.Profiles.API.Infrastructure.Extensions;
using InnoClinic.Shared.Extensions;
using InnoClinic.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseAppSwagger("InnoClinic.Profiles.API");
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
