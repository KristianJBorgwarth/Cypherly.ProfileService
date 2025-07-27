﻿using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Social.Domain.Common;
using Social.Domain.ValueObjects;
using Envelope = Social.API.Common.Envelope;

namespace Social.API.Controllers;

public class BaseController : ControllerBase
{
    protected new ActionResult Ok()
    {
        return base.Ok(Envelope.Ok());
    }

    protected ActionResult Ok<T>(T result)
    {
        return base.Ok(Envelope.Ok(result));
    }

    protected ActionResult Error(List<string> errorMessages)
    {
        var errors = string.Join(";", errorMessages);
        return BadRequest(Envelope.Error(errors));
    }

    protected ActionResult Error(string errorMessage)
    {
        return BadRequest(Envelope.Error(errorMessage));
    }

    protected ActionResult Error(List<ValidationFailure> validationErrors)
    {
        var errors = validationErrors.Select(x => x.ErrorMessage + "(" + x.PropertyName + ")").ToList();
        return Error(errors);
    }

    protected ActionResult Error(Error error)
    {
        return BadRequest(Envelope.Error(error.Message + " (" + error.Code + ")"));
    }

    protected IActionResult FromResult(Result result)
    {
        return result.Failure ? StatusCodeFromResult(result) : base.Ok(Envelope.Ok());
    }

    protected IActionResult FromResult<T>(Result<T> result)
    {
        return result.Failure ? StatusCodeFromResult(result) : base.Ok(Envelope.Ok(result.Value));
    }

    private IActionResult StatusCodeFromResult(Result result)
        => StatusCode(result.Error.StatusCode, Envelope.Error(result.Error.Code));
}