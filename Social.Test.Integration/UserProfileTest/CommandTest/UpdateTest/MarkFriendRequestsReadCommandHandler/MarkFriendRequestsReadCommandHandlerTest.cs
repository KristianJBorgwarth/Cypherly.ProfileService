﻿using Social.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Commands.Update.MarkFriendRequestAsSeen;
using Social.Domain.Aggregates;
using Social.Domain.Interfaces;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.CommandTest.UpdateTest.MarkFriendRequestsReadCommandHandler;

public class MarkFriendRequestsReadCommandHandlerTest : IntegrationTestBase
{
    private readonly Social.Application.Features.UserProfile.Commands.Update.MarkFriendRequestAsSeen.MarkFriendRequestsReadCommandHandler _sut;
    public MarkFriendRequestsReadCommandHandlerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var userProfileRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var friendshipService = scope.ServiceProvider.GetRequiredService<IFriendshipService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Social.Application.Features.UserProfile.Commands.Update.MarkFriendRequestAsSeen.MarkFriendRequestsReadCommandHandler>>();

        _sut = new(
            userProfileRepository,
            friendshipService,
            unitOfWork,
            logger);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnResultFail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new MarkFriendRequestsReadCommand()
        {
            Id = userId,
            RequestTags = new List<string> { "tag1", "tag2" }
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_When_UserFound_And_PendingFriends_Should_MarkRead()
    {
        // Arrange
        var userprofile = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        var friendProfile = new UserProfile(Guid.NewGuid(), "John", UserTag.Create("John"));
        friendProfile.AddFriendship(userprofile);

        Db.UserProfile.Add(friendProfile);
        Db.UserProfile.Add(userprofile);

        await Db.SaveChangesAsync();

        // Act
        var result = await _sut.Handle(new MarkFriendRequestsReadCommand()
        {
            Id = userprofile.Id,
            RequestTags = new List<string> { friendProfile.UserTag.Tag }
        }, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        Db.Friendship.AsNoTracking().FirstOrDefault()!.IsSeen.Should().BeTrue();
    }
}