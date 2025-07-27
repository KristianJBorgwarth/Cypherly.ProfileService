﻿using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfilePicture;

public sealed record GetUserProfilePictureQuery : IQuery<GetUserProfilePictureDto>
{
    public required string ProfilePictureUrl { get; init; }
}