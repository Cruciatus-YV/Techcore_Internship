using FluentValidation;
using Techcore_Internship.Contracts.DTOs.Requests;

namespace Techcore_Internship.Application.Validators;

public class AddAuthorsToBookRequestDtoValidator : AbstractValidator<AddAuthorsToBookRequestDto>
{
    public AddAuthorsToBookRequestDtoValidator()
    {
        RuleFor(x => x.AuthorIds)
            .NotEmpty()
            .WithMessage("Список авторов не может быть пустым.")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Список авторов содержит дублирующиеся идентификаторы.");
    }
}