using FluentValidation;
using Techcore_Internship.Contracts.DTOs.Entities.Author.Requests;

namespace Techcore_Internship.Application.Validators;

public class CreateAuthorRequestValidator : AbstractValidator<CreateAuthorRequest>
{
    public CreateAuthorRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Имя автора обязательно для заполнения.")
            .MaximumLength(100)
            .WithMessage("Имя автора не может превышать 100 символов.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Фамилия автора обязательна для заполнения.")
            .MaximumLength(100)
            .WithMessage("Фамилия автора не может превышать 100 символов.");
    }
}