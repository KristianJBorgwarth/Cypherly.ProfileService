﻿using Social.Domain.Common;
using MediatR;

namespace Social.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }

public interface IQueryLimited<TResponse> : IRequest<Result<TResponse>>
{
    public Guid UserProfileId { get; init; }
}