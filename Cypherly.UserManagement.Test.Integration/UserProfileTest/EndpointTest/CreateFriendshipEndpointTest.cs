﻿using System.Net;
using System.Net.Http.Json;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Infrastructure.Persistence.Context;
using Cypherly.UserManagement.Test.Integration.Setup;
using Cypherly.UserManagement.Test.Integration.Setup.Attributes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Social.Application.Features.UserProfile.Commands.Create.Friendship;

// ReSharper disable EntityFramework.NPlusOne.IncompleteDataQuery
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataUsage

namespace Cypherly.UserManagement.Test.Integration.UserProfileTest.EndpointTest;

public class CreateFriendshipEndpointTest(IntegrationTestFactory<Program, UserManagementDbContext> factory)
    : IntegrationTestBase(factory)
{
    [SkipOnGitHubFact]
    public async void Given_Valid_Request_Should_Create_Friendship()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        var friend = new UserProfile(Guid.NewGuid(), "John", UserTag.Create("John"));
        Db.UserProfile.AddRange(user, friend);
        await Db.SaveChangesAsync();

        var command = new CreateFriendshipCommand()
        {
            FriendTag = friend.UserTag.Tag,
            Id = user.Id
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/userprofile/friendship/create", command);


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Db.Friendship.Should().HaveCount(1);
        var userProfileResult = await Db.UserProfile.FirstOrDefaultAsync(u => u.Id == user.Id);
        userProfileResult!.FriendshipsInitiated.Should().HaveCount(1);
        var friendResult = await Db.UserProfile.FirstOrDefaultAsync(u => u.Id == friend.Id);
        friendResult!.FriendshipsReceived.Should().HaveCount(1);
    }

    [SkipOnGitHubFact]
    public async void Given_Invalid_Request_Should_Return_Error()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        var friend = new UserProfile(Guid.NewGuid(), "John", UserTag.Create("John"));
        Db.UserProfile.AddRange(user, friend);
        // Simulate existing friendship
        Db.Friendship.Add(new(user.Id, friend.Id));
        await Db.SaveChangesAsync();

        var request = new CreateFriendshipCommand()
        {
            FriendTag = friend.UserTag.Tag,
            Id = user.Id
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/userprofile/friendship/create", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Db.Friendship.Should().HaveCount(1);
    }
}