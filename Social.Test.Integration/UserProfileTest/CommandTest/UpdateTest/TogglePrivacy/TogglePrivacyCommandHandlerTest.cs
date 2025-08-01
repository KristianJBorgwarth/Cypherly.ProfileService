﻿using Social.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Commands.Update.TogglePrivacy;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.CommandTest.UpdateTest.TogglePrivacy;

public class TogglePrivacyCommandHandlerTest : IntegrationTestBase
{
    private readonly TogglePrivacyCommandHandler _sut;

    public TogglePrivacyCommandHandlerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TogglePrivacyCommandHandler>>();
        _sut = new TogglePrivacyCommandHandler(repository, uow, logger);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldReturnResultFail()
    {
        // Arrange
        var command = new TogglePrivacyCommand
        {
            Id = Guid.NewGuid(),
            IsPrivate = true
        };

        // Act
        var result = await _sut.Handle(command, default);
        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnResultOk()
    {
        // Arrange
        var profile = new UserProfile(Guid.NewGuid(), "ValidUsername", UserTag.Create("ValidUsername"));
        Db.UserProfile.Add(profile);
        await Db.SaveChangesAsync();

        var command = new TogglePrivacyCommand
        {
            Id = profile.Id,
            IsPrivate = true
        };

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        result.Success.Should().BeTrue();
        Db.UserProfile.AsNoTracking().FirstOrDefault(x => x.Id == command.Id).IsPrivate.Should().BeTrue();
    }
}

// Arrange

// Act

// Assert