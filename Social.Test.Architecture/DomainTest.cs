﻿using Social.Domain.Abstractions;
using Social.Test.Architecture.Helpers;
using Social.Domain.Aggregates;
using FluentAssertions;
using NetArchTest.Rules;

namespace Social.Test.Architecture;

public class DomainTest
{
    [Fact]
    public void Domain_Should_Not_Reference_Application()
    {
        var result = Types.InAssembly(typeof(UserProfile).Assembly)
            .That()
            .ResideInNamespace("SocialSocial.Domain")
            .ShouldNot()
            .HaveDependencyOn("Social.Application")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Domain project should not reference Application project");
    }

    [Fact]
    public void Domain_Should_Not_Reference_Infrastructure()
    {
        var result = Types.InAssembly(typeof(UserProfile).Assembly)
            .That()
            .ResideInNamespace("SocialSocial.Domain")
            .ShouldNot()
            .HaveDependencyOn("Social.Infrastructure")
            .GetResult();

        result.ShouldBeSuccessful("Domain project should not reference Infrastructure project");
    }

    [Fact]
    public void Domain_Should_Not_Reference_Presentation()
    {
        var result = Types.InAssembly(typeof(UserProfile).Assembly)
            .That()
            .ResideInNamespace("SocialSocial.Domain")
            .ShouldNot()
            .HaveDependencyOn("Social.API")
            .GetResult();

        result.ShouldBeSuccessful("Domain project should not reference Presentation project");
    }

    [Fact]
    public void All_AggregateRoots_Should_Inherit_From_AggregateRoot()
    {
        var result = Types.InAssembly(typeof(UserProfile).Assembly)
            .That()
            .AreClasses().And().ResideInNamespace("SocialSocial.Domain.AggregateRoots")
            .Should()
            .Inherit(typeof(AggregateRoot))
            .GetResult();

        result.ShouldBeSuccessful("All AggregateRoots should inherit from AggregateRoot");
    }

    [Fact]
    public void All_Entities_Should_Inherit_From_Entity()
    {
        var result = Types.InAssembly(typeof(UserProfile).Assembly)
            .That()
            .AreClasses()
            .And()
            .ResideInNamespace("SocialSocial.Domain.Entities")
            .Should()
            .Inherit(typeof(Entity))
            .GetResult();

        result.ShouldBeSuccessful("All entities should inherit from Entity");
    }

    [Fact]
    public void All_ValueObjects_Should_Inherit_From_ValueObject()
    {
        var result = Types.InAssembly(typeof(UserProfile).Assembly)
            .That()
            .AreClasses()
            .And()
            .ResideInNamespace("SocialSocial.Domain.ValueObjects")
            .Should()
            .Inherit(typeof(ValueObject))
            .GetResult();

        result.ShouldBeSuccessful("All value objects should inherit from ValueObject");
    }
}