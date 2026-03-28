using Application.Queries.Chats;
using FluentValidation;

namespace Application.Validators.Queries.Chats;

public class GetChatsForUserQueryValidator : AbstractValidator<GetChatsForUserQuery>
{
    public GetChatsForUserQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
