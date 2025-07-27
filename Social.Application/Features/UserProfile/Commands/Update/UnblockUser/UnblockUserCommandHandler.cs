﻿using Cypherly.Application.Abstractions;
using Social.Application.Contracts;
using Cypherly.UserManagement.Domain.Common;
using Cypherly.UserManagement.Domain.Services;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Commands.Update.UnblockUser;

public class UnblockUserCommandHandler(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork unitOfWork,
    IUserBlockingService userBlockingService,
    ILogger<UnblockUserCommandHandler> logger)
    : ICommandHandler<UnblockUserCommand>
{
    public async Task<Result> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.Id);
            if (userProfile is null)
            {
                logger.LogCritical("User with {ID} not found", request.Id);
                return Result.Fail(Errors.General.NotFound(request.Id));
            }

            var userToUnblock = await userProfileRepository.GetByUserTag(request.Tag);
            if (userToUnblock is null)
            {
                logger.LogCritical("User with tag {Tag} not found", request.Tag);
                return Result.Fail(Errors.General.NotFound(request.Tag));
            }

            userBlockingService.UnblockUser(userProfile, userToUnblock);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "An exception occured while user with {ID} tried to unblock user with tag {Tag}", request.Id, request.Tag);
            return Result.Fail(Errors.General.UnspecifiedError("An exception occured while attempting to unblock"));
        }
    }
}