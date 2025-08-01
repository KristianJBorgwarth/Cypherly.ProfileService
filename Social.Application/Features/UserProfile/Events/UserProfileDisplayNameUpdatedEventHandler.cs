﻿using Social.Application.Abstractions;
using Social.Domain.Events.UserProfile;

namespace Social.Application.Features.UserProfile.Events;

public class UserProfileDisplayNameUpdatedEventHandler : IDomainEventHandler<UserProfileDisplayNameUpdatedEvent>
{
    //TODO: implement notification logic when chat server is implemented
    public async Task Handle(UserProfileDisplayNameUpdatedEvent notification, CancellationToken cancellationToken)
    {
        Console.Write("User profile display name updated");
        await Task.CompletedTask;
    }
}