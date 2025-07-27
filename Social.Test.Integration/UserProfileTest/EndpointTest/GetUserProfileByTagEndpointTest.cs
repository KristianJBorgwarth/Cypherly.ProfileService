﻿using System.Net;
using System.Net.Http.Json;
using Social.Infrastructure.Persistence.Context;
using FluentAssertions;
using Social.API.Common;
using Social.Application.Features.UserProfile.Queries.GetUserProfileByTag;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;
using Social.Test.Integration.Setup.Attributes;

namespace Social.Test.Integration.UserProfileTest.EndpointTest;

public class GetUserProfileByTagEndpointTest(IntegrationTestFactory<Program, SocialDbContext> factory)
    : IntegrationTestBase(factory)
{
    [SkipOnGitHubFact]
    public async Task Handle_Valid_Request_Should_Return_UserProfile()
    {
        // Arrange
        var requestingUser = new UserProfile(Guid.NewGuid(), "requestingUser", UserTag.Create("requestingUser"));
        var userProfile = new UserProfile(Guid.NewGuid(), "userProfile", UserTag.Create("userProfile"));
        await Db.AddRangeAsync(requestingUser, userProfile);
        await Db.SaveChangesAsync();

        var query = new GetUserProfileByTagQuery()
        {
            Id = requestingUser.Id,
            Tag = userProfile.UserTag.Tag
        };

        var encodedFriendTag = Uri.EscapeDataString(query.Tag);

        //Act
        var response =
            await Client.GetFromJsonAsync<Envelope<GetUserProfileByTagDto>>(
                $"api/userprofile/tag?Id={query.Id}&Tag={encodedFriendTag}");

        response.Should().NotBeNull();
        response.Result.Should().NotBeNull();
        response.Result.UserTag.Should().Be(userProfile.UserTag.Tag);
    }

    [SkipOnGitHubFact]
    public async Task Handle_Invalid_Request_Should_Retur_BadRequest()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "userProfile", UserTag.Create("userProfile"));
        await Db.AddAsync(userProfile);
        await Db.SaveChangesAsync();

        var query = new GetUserProfileByTagQuery()
        {
            Id = Guid.NewGuid(),
            Tag = userProfile.UserTag.Tag
        };

        var encodedFriendTag = Uri.EscapeDataString(query.Tag);

        // Act
        var response = await Client.GetAsync($"api/UserProfile/tag?Id={query.Id}&Tag={encodedFriendTag}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [SkipOnGitHubFact]
    public async Task Handle_Valid_Request_No_Data_Should_Return_NoContent()
    {
        // Arrange
        var requestingUser = new UserProfile(Guid.NewGuid(), "requestingUser", UserTag.Create("requestingUser"));

        await Db.AddAsync(requestingUser);
        await Db.SaveChangesAsync();

        var query = new GetUserProfileByTagQuery()
        {
            Id = requestingUser.Id,
            Tag = "dummyTag"
        };

        var encodedFriendTag = Uri.EscapeDataString(query.Tag);

        // Act
        var response = await Client.GetAsync($"api/UserProfile/tag?Id={query.Id}&Tag={encodedFriendTag}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

    }
}