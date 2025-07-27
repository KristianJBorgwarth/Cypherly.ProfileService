﻿using Cypherly.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Update.ProfilePicture;

public sealed record UpdateUserProfilePictureCommand : ICommandId<UpdateUserProfilePictureDto>
{
    public required Guid Id { get; init; }
    public required IFormFile NewProfilePicture { get; init; }
}