﻿using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfile;

public sealed record GetUserProfileQuery : IQuery<GetUserProfileDto>
{
    public required Guid UserProfileId { get; init; }
    public required Guid ExclusiveConnectionId { get; init; }
}