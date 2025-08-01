﻿using Microsoft.AspNetCore.Mvc.Filters;
using Social.API.Filters;

namespace Social.Test.Integration.Setup.Authentication;

public class MockValidateUserIdIdFilter : IValidateUserIdFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await next();
    }
}