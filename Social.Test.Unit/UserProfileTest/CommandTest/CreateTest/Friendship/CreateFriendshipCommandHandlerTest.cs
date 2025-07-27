﻿using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Commands.Create.Friendship;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Social.Domain.Aggregates;
using Social.Domain.Common;
using Social.Domain.Interfaces;
using Social.Domain.ValueObjects;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.CommandTest.CreateTest.Friendship;

public class CreateFriendshipCommandHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IFriendshipService _fakeService;
    private readonly IUnitOfWork _fakeUnitOfWork;
    private readonly CreateFriendshipCommandHandler _sut;

    public CreateFriendshipCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeService = A.Fake<IFriendshipService>();
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();
        var fakeLogger = A.Fake<ILogger<CreateFriendshipCommandHandler>>();
        _sut = new(_fakeRepo, _fakeService, _fakeUnitOfWork, fakeLogger);
    }

    [Fact]
    public async Task Handle_WhenFriendNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = new CreateFriendshipCommand()
        {
            FriendTag = "tag",
            Id = Guid.NewGuid()
        };
        A.CallTo(() => _fakeRepo.GetByUserTag(command.FriendTag))!.Returns<UserProfile>(null!);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = new CreateFriendshipCommand()
        {
            FriendTag = "tag",
            Id = Guid.NewGuid()
        };

        A.CallTo(() => _fakeRepo.GetByUserTag(A<string>._)).Returns(new UserProfile(Guid.NewGuid(), "eric", UserTag.Create("eric")));

        A.CallTo(() => _fakeRepo.GetByIdAsync(command.Id))!.Returns<UserProfile>(null!);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async Task Handle_WhenCreateFriendshipFails_ReturnsError()
    {
        // Arrange
        var command = new CreateFriendshipCommand()
        {
            FriendTag = "tag",
            Id = Guid.NewGuid()
        };

        var user = new UserProfile(Guid.NewGuid(), "eric", UserTag.Create("eric"));
        var friend = new UserProfile(Guid.NewGuid(), "friend", UserTag.Create("friend"));

        A.CallTo(() => _fakeRepo.GetByUserTag(A<string>._)).Returns(friend);
        A.CallTo(() => _fakeRepo.GetByIdAsync(command.Id)).Returns(user);
        A.CallTo(() => _fakeService.CreateFriendship(user, friend)).Returns(Result.Fail(Errors.General.UnspecifiedError("error")));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be("error");
    }

    [Fact]
    public async Task Handle_WhenSuccess_ReturnsOk()
    {
        // Arrange
        var command = new CreateFriendshipCommand()
        {
            FriendTag = "tag",
            Id = Guid.NewGuid()
        };

        var user = new UserProfile(Guid.NewGuid(), "eric", UserTag.Create("eric"));
        var friend = new UserProfile(Guid.NewGuid(), "friend", UserTag.Create("friend"));

        A.CallTo(() => _fakeRepo.GetByUserTag(A<string>._)).Returns(friend);
        A.CallTo(() => _fakeRepo.GetByIdAsync(command.Id)).Returns(user);
        A.CallTo(() => _fakeService.CreateFriendship(user, friend)).Returns(Result.Ok());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
    }
}