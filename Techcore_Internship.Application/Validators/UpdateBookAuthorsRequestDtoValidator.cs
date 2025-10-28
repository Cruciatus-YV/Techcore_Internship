using FluentValidation;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Requests;

namespace Techcore_Internship.Application.Validators;

public class UpdateBookAuthorsRequestValidator : AbstractValidator<UpdateBookAuthorsRequest>
{
    public UpdateBookAuthorsRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.NewAuthors?.Any() == true || x.ExistingAuthorIds?.Any() == true)
            .WithMessage("Должен быть указан хотя бы один автор (новый или существующий).");

        RuleForEach(x => x.NewAuthors)
            .SetValidator(new CreateAuthorRequestValidator())
            .When(x => x.NewAuthors != null);
    }
}