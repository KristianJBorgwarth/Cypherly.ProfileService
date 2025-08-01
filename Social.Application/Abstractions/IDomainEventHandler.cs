﻿using MediatR;
using Social.Domain.Abstractions;

namespace Social.Application.Abstractions;

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{

}