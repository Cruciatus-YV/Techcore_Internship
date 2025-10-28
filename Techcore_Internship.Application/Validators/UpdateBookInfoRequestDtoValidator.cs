using FluentValidation;
using Techcore_Internship.Contracts.DTOs.Entities.Book.Requests;

namespace Techcore_Internship.Application.Validators;

public class UpdateBookInfoRequestValidator : AbstractValidator<UpdateBookInfoRequest>
{
    public UpdateBookInfoRequestValidator()
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
    }
}