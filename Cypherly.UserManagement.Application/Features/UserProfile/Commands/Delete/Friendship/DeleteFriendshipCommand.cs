﻿using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Delete.Friendship;

public sealed record DeleteFriendshipCommand : ICommandId
{
    public required Guid Id { get; init; }
    public required string FriendTag { get; init; }
}