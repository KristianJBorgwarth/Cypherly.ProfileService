﻿using Social.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Commands.Update.UnblockUser;
using Social.Domain.Aggregates;
using Social.Domain.Services;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.CommandTest.UpdateTest.UnblockUser;

public class UnblockUserCommandHandlerTest : IntegrationTestBase
{
    private readonly UnblockUserCommandHandler _sut;
    public UnblockUserCommandHandlerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var service = scope.ServiceProvider.GetRequiredService<IUserBlockingService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<UnblockUserCommandHandler>>();

        _sut = new UnblockUserCommandHandler(repo, unitOfWork, service, logger);
    }

    [Fact]
    public async void Handle_WhenUserNotFount_Returns_Fail_NotFound()
    {
        // Arrange
        var command = new UnblockUserCommand { Id = Guid.NewGuid(), Tag = "tag" };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async void Handle_WhenUserToUnblockNotFound_Returns_Fail_NotFound()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "Dave", UserTag.Create("Dave"));
        await Db.AddAsync(userProfile);
        await Db.SaveChangesAsync();

        var command = new UnblockUserCommand { Id = userProfile.Id, Tag = "tag" };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("tag");
    }

    [Fact]
    public async void Handle_WhenUserUnblocked_Returns_Ok()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "Dave", UserTag.Create("Dave"));
        var userToUnblock = new UserProfile(Guid.NewGuid(), "Blocked", UserTag.Create("Blocked"));

        userProfile.BlockUser(userToUnblock.Id);
        await Db.AddAsync(userProfile);
        await Db.AddAsync(userToUnblock);
        await Db.SaveChangesAsync();

        var command = new UnblockUserCommand { Id = userProfile.Id, Tag = userToUnblock.UserTag.Tag };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        Db.BlockedUser.Should().HaveCount(0);
    }
}