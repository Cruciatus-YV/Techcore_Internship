using FluentValidation;
using Techcore_Internship.Contracts.DTOs.Requests;

namespace Techcore_Internship.Application.Validators;

public class UpdateBookAuthorsRequestDtoValidator : AbstractValidator<UpdateBookAuthorsRequestDto>
{
    public UpdateBookAuthorsRequestDtoValidator()
    {
        RuleFor(x => x)
            .Must(x => x.NewAuthors?.Any() == true || x.ExistingAuthorIds?.Any() == true)
            .WithMessage("Должен быть указан хотя бы один автор (новый или существующий).");

        RuleForEach(x => x.NewAuthors)
            .SetValidator(new CreateAuthorRequestDtoValidator())
            .When(x => x.NewAuthors != null);
    }
}