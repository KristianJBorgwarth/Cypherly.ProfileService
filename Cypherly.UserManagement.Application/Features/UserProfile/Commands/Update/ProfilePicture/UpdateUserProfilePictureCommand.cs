﻿using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.ProfilePicture;

public sealed record UpdateUserProfilePictureCommand : ICommandId<UpdateUserProfilePictureDto>
{
    public required Guid Id { get; init; }
    public required IFormFile NewProfilePicture { get; init; }
}