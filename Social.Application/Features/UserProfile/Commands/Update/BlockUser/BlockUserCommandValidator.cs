using Social.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Commands.Update.BlockUser;

public class BlockUserCommandValidator : AbstractValidator<BlockUserCommand>
{
    public BlockUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(Errors.General.ValueIsRequired(nameof(BlockUserCommand.Id)).Message);

        RuleFor(x => x.BlockedUserTag)
            .NotEmpty().WithMessage(Errors.General.ValueIsRequired(nameof(BlockUserCommand)).Message);
    }
}