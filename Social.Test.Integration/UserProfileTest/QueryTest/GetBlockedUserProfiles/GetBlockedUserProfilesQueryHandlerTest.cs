using Social.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Queries.GetBlockedUserProfiles;
using Social.Domain.Aggregates;
using Social.Domain.Common;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.QueryTest.GetBlockedUserProfiles;

public class GetBlockedUserProfilesQueryHandlerTest : IntegrationTestBase
{
    private readonly GetBlockedUserProfilesQueryHandler _sut;

    public GetBlockedUserProfilesQueryHandlerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetBlockedUserProfilesQueryHandler>>();
        _sut = new GetBlockedUserProfilesQueryHandler(repo, logger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Query_Should_Return_BlockedUserProfiles()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        var blockedUserProfile = new UserProfile(Guid.NewGuid(), "blocked1", UserTag.Create("blocked1"));
        var blockedUserProfile2 = new UserProfile(Guid.NewGuid(), "blocked2", UserTag.Create("blocked2"));

        userProfile.BlockUser(blockedUserProfile.Id);
        userProfile.BlockUser(blockedUserProfile2.Id);

        await Db.AddRangeAsync(blockedUserProfile, blockedUserProfile2, userProfile);
        await Db.SaveChangesAsync();

        var query = new GetBlockedUserProfilesQuery() { UserId = userProfile.Id };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull().And.NotBeEmpty();
        result.Value.Should().ContainSingle(f => f.Username == "blocked1").And
            .ContainSingle(f => f.Username == "blocked2");
    }

    [Fact]
    public async Task Handle_Given_Invalid_UserProfileId_Should_Return_Result_Fail()
    {
        // Arrange
        var query = new GetBlockedUserProfilesQuery()
        {
            UserId = Guid.NewGuid()
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().BeEquivalentTo(Errors.General.NotFound(query.UserId).Message);
    }

    [Fact]
    public async Task Handle_Given_Valid_Query_But_No_BlockedUserProfiles_Should_Return_Empty()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        await Db.AddAsync(userProfile);
        await Db.SaveChangesAsync();

        var query = new GetBlockedUserProfilesQuery() { UserId = userProfile.Id };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull().And.BeEmpty();
    }
}