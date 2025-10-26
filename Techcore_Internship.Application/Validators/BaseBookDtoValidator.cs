using FluentValidation;
using Techcore_Internship.Contracts.DTOs.Requests;
using Techcore_Internship.Contracts.DTOs.Responses;

namespace Techcore_Internship.Application.Validators;

public abstract class BaseBookDtoValidator<T> : AbstractValidator<T>
where T : CreateBookRequestDto
{
    protected BaseBookDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(x => x.Year)
            .GreaterThan(1900)
            .WithMessage("Year must be greater than 1900.");
    }
}

public class CreateBookDtoValidator : BaseBookDtoValidator<CreateBookRequestDto>
{
}

public class BookDtoValidator : BaseBookDtoValidator<BookResponseDto>
{
}
