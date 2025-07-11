using Application.Queries.Comments;
using FluentValidation;

namespace Application.Validators.Queries.Comments;

public class SearchCommentsRangeByTaskIdQueryValidator : AbstractValidator<SearchCommentsRangeByTaskIdQuery>
{
    public SearchCommentsRangeByTaskIdQueryValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("TaskId is required.");
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0.");
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");
        RuleFor(x => x.SearchTerm)
            .MaximumLength(256).WithMessage("Search term must not exceed 256 characters.");
    }
}
