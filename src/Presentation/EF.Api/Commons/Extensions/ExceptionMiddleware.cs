using System.Net;
using EF.Core.Commons.DomainObjects;
using Microsoft.AspNetCore.Mvc;

namespace EF.Api.Commons.Extensions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException e)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Message", new[] { e.Message } }
            }));
        }
    }
}