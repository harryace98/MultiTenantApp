using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace MultiTenantApp.API.Infrastructure;
public static class CustomResults
{
    public static IActionResult Problem(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }

        var problemDetails = new ProblemDetails
        {
            Title = GetTitle(result.Error),
            Detail = GetDetail(result.Error),
            Type = GetType(result.Error.Type),
            Status = GetStatusCode(result.Error.Type),
            Extensions = GetErrors(result) ?? new Dictionary<string, object?>(),
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }
    public static IActionResult Success<T>(T? result = default, string? title = null, int? status = null)
    {
        ResultDTO response = new()
        {
            status = status ?? StatusCodes.Status200OK,
            title = title ?? "Operation successful",
            data = result
        };

        return new ObjectResult(response)
        {
            StatusCode = status ?? StatusCodes.Status200OK
        };
    }

    //public static IActionResult Success(object? result = null, string title = "success.",
    //    int status = StatusCodes.Status200OK)
    //{
    //    ResultDTO resultDTO = new()
    //    {
    //        status = status,
    //        data = result,
    //        title = title,
    //    };

    //    return new ObjectResult(resultDTO)
    //    {
    //        StatusCode = resultDTO.status
    //    };
    //}

    private static string GetTitle(Error error) =>
        error.Type switch
        {
            ErrorType.Failure => error.Code,
            ErrorType.Validation => error.Code,
            ErrorType.Problem => error.Code,
            ErrorType.NotFound => error.Code,
            ErrorType.Conflict => error.Code,
            ErrorType.DatabaseTransaction => error.Code,
            ErrorType.Invalid => error.Code,
            ErrorType.UnsupportedMediaType => error.Code,
            _ => "Server failure"
        };

    private static string GetDetail(Error error) =>
        error.Type switch
        {
            ErrorType.Failure => error.Description,
            ErrorType.Validation => error.Description,
            ErrorType.Problem => error.Description,
            ErrorType.NotFound => error.Description,
            ErrorType.Conflict => error.Description,
            ErrorType.DatabaseTransaction => error.Description,
            ErrorType.Invalid => error.Description,
            ErrorType.UnsupportedMediaType => error.Description,
            _ => "An unexpected error occurred"
        };

    private static string GetType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Failure => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            ErrorType.Validation => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            ErrorType.Problem => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            ErrorType.Conflict => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
            ErrorType.DatabaseTransaction => "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            ErrorType.Invalid => "https://tools.ietf.org/html/rfc9110#section-15.5.21",
            ErrorType.UnsupportedMediaType => "https://tools.ietf.org/html/rfc9110#section-15.5.16",
            _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
        };

    private static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Failure => StatusCodes.Status400BadRequest,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Problem => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.DatabaseTransaction => StatusCodes.Status500InternalServerError,
            ErrorType.Invalid => StatusCodes.Status422UnprocessableEntity,
            ErrorType.UnsupportedMediaType => StatusCodes.Status415UnsupportedMediaType,
            _ => StatusCodes.Status500InternalServerError
        };

    private static IDictionary<string, object?>? GetErrors(Result result)
    {
        if (result.Error is not ValidationError validationError)
        {
            return null;
        }

        return new Dictionary<string, object?>
        {
            { "errors", validationError.Errors }
        };
    }
}

