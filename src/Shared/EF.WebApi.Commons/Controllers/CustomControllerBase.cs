using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EF.WebApi.Commons.Controllers;

[ApiController]
public abstract class CustomControllerBase : ControllerBase
{
    private readonly ICollection<string> Errors = new List<string>();

    protected IActionResult Respond(object? result = null)
    {
        if (IsValidOperation()) return Ok(result);

        return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Messages", Errors.ToArray() }
        }));
    }

    protected IActionResult Respond(ModelStateDictionary modelState)
    {
        var errors = modelState.Values.SelectMany(e => e.Errors);
        foreach (var error in errors) AddError(error.ErrorMessage);

        return Respond();
    }

    protected IActionResult Respond(ReadOnlyCollection<string> errors)
    {
        AddErrors(errors);
        return Respond();
    }

    protected bool IsValidOperation()
    {
        return !Errors.Any();
    }

    protected void AddError(string error)
    {
        Errors.Add(error);
    }

    protected void AddErrors(ReadOnlyCollection<string> errors)
    {
        foreach (var error in errors) Errors.Add(error);
    }
}