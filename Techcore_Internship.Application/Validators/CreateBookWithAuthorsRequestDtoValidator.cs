using FluentValidation;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Requests;

namespace Techcore_Internship.Application.Validators;

public class CreateBookWithAuthorsRequestValidator : AbstractValidator<CreateBookWithAuthorsRequest>
{
    public CreateBookWithAuthorsRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Название книги обязательно для заполнения.")
            .MaximumLength(200)
            .WithMessage("Название книги не может превышать 200 символов.");

        RuleFor(x => x.Year)
            .GreaterThan(1900)
            .WithMessage("Год издания должен быть больше 1900.")
            .LessThanOrEqualTo(DateTime.Now.Year + 1)
            .WithMessage($"Год издания не может быть больше {DateTime.Now.Year + 1}.");

        RuleFor(x => x.NewAuthors)
            .Must((dto, newAuthors) => newAuthors?.Any() == true || dto.ExistingAuthorIds?.Any() == true)
            .WithMessage("Книга должна иметь хотя бы одного автора (нового или существующего).");

        RuleForEach(x => x.NewAuthors)
            .SetValidator(new CreateAuthorRequestValidator())
            .When(x => x.NewAuthors != null);
    }
}