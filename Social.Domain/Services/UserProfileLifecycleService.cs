﻿using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;

namespace Social.Domain.Services;

public interface IUserProfileLifecycleService
{
    UserProfile CreateUserProfile(Guid userId, string username);
    void SoftDelete(UserProfile userProfile);
    void RevertSoftDelete(UserProfile userProfile);
}
public class UserProfileLifecycleLifecycleService : IUserProfileLifecycleService
{
    public UserProfile CreateUserProfile(Guid userId, string username)
    {
        var tag = UserTag.Create(username);
        return new(userId, username, tag);
    }

    public void SoftDelete(UserProfile userProfile)
    {
        userProfile.SetDelete();
    }

    public void RevertSoftDelete(UserProfile userProfile)
    {
        userProfile.RevertDelete();
    }
}
