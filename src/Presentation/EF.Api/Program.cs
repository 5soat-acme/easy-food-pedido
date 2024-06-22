using EF.Api.Commons.Config;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

builder.Services.AddApiConfig(builder.Configuration, env);

var app = builder.Build();

app.UseApiConfig();

app.Run();

namespace EF.Api
{
    public class Program
    {
    }
}