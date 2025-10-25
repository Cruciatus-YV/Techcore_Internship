using FluentValidation;
using Techcore_Internship.WebApi.Dto;

namespace Techcore_Internship.WebApi.Validators;

public abstract class BaseBookDtoValidator<T> : AbstractValidator<T>
    where T : CreateBookDto
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

public class CreateBookDtoValidator : BaseBookDtoValidator<CreateBookDto>
{
}

public class BookDtoValidator : BaseBookDtoValidator<BookDto>
{
}