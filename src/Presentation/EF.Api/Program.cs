using EF.Api.Commons.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfig(builder.Configuration);

var app = builder.Build();

app.UseApiConfig();

app.Run();

namespace EF.Api
{
    public class Program
    {
    }
}