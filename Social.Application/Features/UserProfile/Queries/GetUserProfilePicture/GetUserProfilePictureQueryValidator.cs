﻿using Cypherly.UserManagement.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfilePicture;

public sealed class GetUserProfilePictureQueryValidator : AbstractValidator<GetUserProfilePictureQuery>
{
    public GetUserProfilePictureQueryValidator()
    {
        RuleFor(x => x.ProfilePictureUrl)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsRequired(nameof(GetUserProfilePictureQuery.ProfilePictureUrl)).Message);
    }
}