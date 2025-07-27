using Cypherly.UserManagement.Domain.Abstractions;
using Cypherly.UserManagement.Domain.Common;
using Cypherly.UserManagement.Domain.Entities;
using Cypherly.UserManagement.Domain.Enums;
using Cypherly.UserManagement.Domain.Events.UserProfile;
using Cypherly.UserManagement.Domain.ValueObjects;

namespace Cypherly.UserManagement.Domain.Aggregates;

public partial class UserProfile : AggregateRoot
{
    public string Username { get; private set; } = null!;
    public UserTag UserTag { get; private set; } = null!;
    public string? DisplayName { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public bool IsPrivate { get; private set; }

    private readonly List<BlockedUser> _blockedUsers = [];
    public virtual IReadOnlyCollection<BlockedUser> BlockedUsers => _blockedUsers;

    private readonly List<Friendship> _friendshipsReceived = [];
    public virtual IReadOnlyCollection<Friendship> FriendshipsReceived => _friendshipsReceived;

    private readonly List<Friendship> _friendshipsInitiated = [];
    public virtual IReadOnlyCollection<Friendship> FriendshipsInitiated => _friendshipsInitiated;

    public UserProfile() : base(Guid.Empty) { } // For EF Core

    public UserProfile(Guid id, string username, UserTag userUserTag) : base(id)
    {
        Username = username;
        DisplayName = username;
        UserTag = userUserTag;
    }

    public void SetProfilePictureUrl(string profilePictureUrl)
    {
        ProfilePictureUrl = profilePictureUrl;
        AddDomainEvent(new UserProfilePictureUpdatedEvent(Id));
    }

    public Result SetDisplayName(string displayName)
    {
        switch (displayName.Length)
        {
            case < 3:
                return Result.Fail(Errors.General.ValueTooSmall(nameof(displayName), 3));
            case > 20:
                return Result.Fail(Errors.General.ValueTooLarge(nameof(displayName), 20));
        }
        if (!DisplayNameRegex().IsMatch(displayName))
            return Result.Fail(Errors.General.UnexpectedValue(nameof(displayName)));

        DisplayName = displayName;
        AddDomainEvent(new UserProfileDisplayNameUpdatedEvent(Id));
        return Result.Ok();
    }

    public void TogglePrivacy(bool isPrivate) => IsPrivate = isPrivate;

    public Result AddFriendship(UserProfile userProfile)
    {
        if (Id == userProfile.Id)
            return Result.Fail(Errors.General.UnspecifiedError("Cannot add self as friend"));

        if (FriendshipsInitiated.Any(f => f.FriendProfileId == userProfile.Id) || FriendshipsReceived.Any(f => f.UserProfileId == userProfile.Id))
            return Result.Fail(Errors.General.UnspecifiedError("Friendship already exists"));

        _friendshipsInitiated.Add(new(Id, userProfile.Id));
        return Result.Ok();
    }

    //TODO: should probaly not return Result, but just throw exceptions
    public Result<Guid> DeleteFriendship(string friendTag)
    {
        if (friendTag == UserTag.Tag)
            throw new InvalidOperationException("Cannot delete self as friend");

        var friendshipInitiated = FriendshipsInitiated.FirstOrDefault(f => f.FriendProfile.UserTag.Tag == friendTag);

        if (friendshipInitiated is not null)
        {
            _friendshipsInitiated.Remove(friendshipInitiated);
            return Result.Ok(friendshipInitiated.FriendProfileId);
        }

        var friendshipReceived = FriendshipsReceived.FirstOrDefault(f => f.UserProfile.UserTag.Tag == friendTag);

        if (friendshipReceived is not null)
        {
            _friendshipsReceived.Remove(friendshipReceived);
            return Result.Ok(friendshipReceived.UserProfileId);
        }

        return Result.Fail<Guid>(Errors.General.UnspecifiedError("Friendship not found"));
    }

    public IReadOnlyCollection<UserProfile> GetFriends()
    {
        var friends = new List<UserProfile>();

        friends.AddRange(FriendshipsInitiated
            .Where(x => x.Status == FriendshipStatus.Accepted)
            .Select(f => f.FriendProfile));

        friends.AddRange(FriendshipsReceived
            .Where(x => x.Status == FriendshipStatus.Accepted)
            .Select(f => f.UserProfile));

        return friends;
    }

    public void BlockUser(Guid blockedUserId)
    {
        if (blockedUserId == Guid.Empty)
            throw new InvalidOperationException("BlockedUserId cannot be empty");

        if (blockedUserId == Id)
            throw new InvalidOperationException("Cannot block self");

        if (_blockedUsers.Any(c => c.BlockedUserProfileId == blockedUserId))
            throw new InvalidOperationException("User already blocked");

        _blockedUsers.Add(new(blockingUserProfileId: Id, blockedUserProfileId: blockedUserId));
        AddDomainEvent(new UserBlockedEvent(Id, blockedUserId));
    }

    public void UnblockUser(Guid unblockedUserId)
    {
        if (unblockedUserId == Guid.Empty)
            throw new InvalidOperationException("UnblockedUserId cannot be empty");

        if (unblockedUserId == Id)
            throw new InvalidOperationException("Cannot unblock self");


        var blockedUser = BlockedUsers.FirstOrDefault(c => c.BlockedUserProfileId == unblockedUserId);
        if (blockedUser is null)
            throw new InvalidOperationException("User not blocked");

        _blockedUsers.Remove(blockedUser);
    }


    [System.Text.RegularExpressions.GeneratedRegex(@"^[a-zA-Z0-9]*$")]
    private static partial System.Text.RegularExpressions.Regex DisplayNameRegex();
}
