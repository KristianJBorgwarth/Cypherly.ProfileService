﻿using Social.Application.Features.UserProfile.Commands.Update.BlockUser;
using FluentValidation.TestHelper;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.CommandTest.UpdateTest.BlockUser;

public class BlockUserCommandValidatorTest
{
    private readonly BlockUserCommandValidator _sut = new();

    [Fact]
    public void ShouldHaveErrorWhenUserIdIsEmpty()
    {
        // Arrange
        var command = new BlockUserCommand { TenantId = Guid.Empty, BlockedUserTag = "test" };

        // Act
        var result = _sut.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TenantId);
    }

    [Fact]
    public void ShouldHaveErrorWhenBlockedUserTagIsEmpty()
    {
        // Arrange
        var command = new BlockUserCommand { TenantId = Guid.NewGuid(), BlockedUserTag = "" };

        // Act
        var result = _sut.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BlockedUserTag);
    }

    [Fact]
    public void ShouldNotHaveErrorWhenCommandIsValid()
    {
        // Arrange
        var command = new BlockUserCommand { TenantId = Guid.NewGuid(), BlockedUserTag = "test" };

        // Act
        var result = _sut.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}