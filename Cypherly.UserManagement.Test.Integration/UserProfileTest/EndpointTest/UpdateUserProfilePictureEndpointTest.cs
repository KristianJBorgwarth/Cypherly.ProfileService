﻿using System.Net;
using System.Text.Json;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using Social.Infrastructure.Persistence.Context;
using Cypherly.UserManagement.Test.Integration.Setup;
using Cypherly.UserManagement.Test.Integration.Setup.Attributes;
using Cypherly.UserManagement.Test.Integration.Setup.Helpers;
using FluentAssertions;
using Social.API.Common;
using Social.Application.Features.UserProfile.Commands.Update.ProfilePicture;

namespace Cypherly.UserManagement.Test.Integration.UserProfileTest.EndpointTest;

public class UpdateUserProfilePictureEndpointTest(IntegrationTestFactory<Program, UserManagementDbContext> factory)
    : IntegrationTestBase(factory)
{
    [SkipOnGitHubFact]
    public async Task Given_Valid_Command_Should_Update_ProfilePic_And_Return_Dto()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
        Db.UserProfile.Add(user);
        await Db.SaveChangesAsync();

        var command = new UpdateUserProfilePictureCommand
        {
            Id = user.Id,
            NewProfilePicture = FormFileHelper.CreateFormFile(
                Path.Combine(DirectoryHelper.GetProjectRootDirectory(),
                    "Cypherly.UserManagement.Test.Integration/Setup/Resources/test_profile_picture.png"),
                "image/png")
        };

        // Use the helper method to create the form data
        var form = CreateMultipartFormData(command);

        // Act
        var response = await Client.PutAsync("/api/UserProfile/profile-picture", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var envelope = JsonSerializer.Deserialize<Envelope<UpdateUserProfilePictureDto>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        envelope.Should().NotBeNull();
        envelope.Result.Should().NotBeNull();
        envelope.Result.ProfilePictureUrl.Should().NotBeNullOrEmpty();
        Db.OutboxMessage.Should().HaveCount(1);
    }


    [SkipOnGitHubFact]
    public async Task Given_Invalid_Command_WrongID_Should_Return_NotFound_Fail_Result()
    {
        // Arrange
        var invalidUserId = Guid.NewGuid(); // This ID does not exist in the database

        var command = new UpdateUserProfilePictureCommand
        {
            Id = invalidUserId,
            NewProfilePicture = FormFileHelper.CreateFormFile(
                Path.Combine(DirectoryHelper.GetProjectRootDirectory(),
                    "Cypherly.UserManagement.Test.Integration/Setup/Resources/test_profile_picture.png"),
                "image/png")
        };

        // Use the helper method to create the form data
        var form = CreateMultipartFormData(command);

        // Act
        var response = await Client.PutAsync("/api/UserProfile/profile-picture", form);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    private static MultipartFormDataContent CreateMultipartFormData(UpdateUserProfilePictureCommand command)
    {
        var form = new MultipartFormDataContent();

        // Add the user ID
        form.Add(new StringContent(command.Id.ToString()), nameof(UpdateUserProfilePictureCommand.Id));

        // Add the file content
        var fileContent = new StreamContent(command.NewProfilePicture.OpenReadStream());
        fileContent.Headers.Add("Content-Type", command.NewProfilePicture.ContentType);
        form.Add(fileContent, nameof(UpdateUserProfilePictureCommand.NewProfilePicture), command.NewProfilePicture.FileName);

        return form;
    }

}