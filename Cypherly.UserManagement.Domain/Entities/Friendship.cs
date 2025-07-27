﻿using Cypherly.UserManagement.Domain.Abstractions;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Enums;

namespace Cypherly.UserManagement.Domain.Entities;

public class Friendship : Entity
{
    public Guid UserProfileId { get; private set; }
    public Guid FriendProfileId { get; private set; }
    public FriendshipStatus Status { get; private set; }
    public bool IsSeen { get; private set; }
    public virtual UserProfile UserProfile { get; private set; } = null!;
    public virtual UserProfile FriendProfile { get; private set; } = null!;

    public Friendship() { } // Required for EF Core

    public Friendship(Guid userProfileId, Guid friendProfileId) : base(Guid.Empty)
    {
        UserProfileId = userProfileId;
        FriendProfileId = friendProfileId;
        Status = FriendshipStatus.Pending;
    }

    public void AcceptFriendship()
    {
        Status = FriendshipStatus.Accepted;
    }

    public void MarkAsSeen()
    {
        IsSeen = true;
    }
}