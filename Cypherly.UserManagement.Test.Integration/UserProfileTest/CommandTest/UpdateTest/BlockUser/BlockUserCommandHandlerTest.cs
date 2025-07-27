﻿using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Common;
using Cypherly.UserManagement.Domain.Services;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Infrastructure.Persistence.Context;
using Cypherly.UserManagement.Test.Integration.Setup;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Commands.Update.BlockUser;

// ReSharper disable EntityFramework.NPlusOne.IncompleteDataQuery
// ReSharper disable EntityFramework.NPlusOne.IncompleteDataUsage

namespace Cypherly.UserManagement.Test.Integration.UserProfileTest.CommandTest.UpdateTest.BlockUser;

public class BlockUserCommandHandlerTest : IntegrationTestBase
{
    private readonly BlockUserCommandHandler _sut;
    public BlockUserCommandHandlerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var service = scope.ServiceProvider.GetRequiredService<IUserBlockingService>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<BlockUserCommandHandler>>();

        _sut = new BlockUserCommandHandler(repo, service, uow, logger);
    }

    [Fact]
    public async Task Handle_Given_Invalid_Id_Should_Return_NotFound()
    {
        // Arrange
        var blockuser = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        await Db.AddAsync(blockuser);
        await Db.SaveChangesAsync();

        var command = new BlockUserCommand()
        {
            BlockedUserTag = blockuser.UserTag.Tag,
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.NotFound(command.Id));
    }

    [Fact]
    public async Task Handle_Given_Invalid_Tag_Should_Return_NotFound()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        await Db.AddAsync(user);
        await Db.SaveChangesAsync();

        var command = new BlockUserCommand()
        {
            BlockedUserTag = "John",
            Id = user.Id
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.NotFound(command.BlockedUserTag));
    }

    [Fact]
    public async Task Handle_Given_Valid_Request_Should_BlockUser_And_Delete_Friendship_And_Return_Success()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        var blockedUser = new UserProfile(Guid.NewGuid(), "John", UserTag.Create("John"));
        user.AddFriendship(blockedUser);
        await Db.AddAsync(user);
        await Db.AddAsync(blockedUser);
        await Db.SaveChangesAsync();

        var command = new BlockUserCommand()
        {
            BlockedUserTag = blockedUser.UserTag.Tag,
            Id = user.Id
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        Db.UserProfile.AsNoTracking().FirstOrDefault(u => u.Id == user.Id)!.BlockedUsers.Should().HaveCount(1);
        Db.Friendship.AsNoTracking().Should().HaveCount(0);
    }
}