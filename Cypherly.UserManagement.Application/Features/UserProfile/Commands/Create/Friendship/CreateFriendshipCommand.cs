﻿using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Create.Friendship;

public sealed record CreateFriendshipCommand : ICommand
{
    public Guid Id { get; init; }
    public required string FriendTag { get; init; }
}