using FluentValidation;
using Techcore_Internship.Contracts.DTOs.Requests;

namespace Techcore_Internship.Application.Validators;

public class AddBooksToAuthorRequestDtoValidator : AbstractValidator<AddBooksToAuthorRequestDto>
{
    public AddBooksToAuthorRequestDtoValidator()
    {
        RuleFor(x => x.BookIds)
            .NotEmpty()
            .WithMessage("Список книг не может быть пустым.")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Список книг содержит дублирующиеся идентификаторы.");
    }
}