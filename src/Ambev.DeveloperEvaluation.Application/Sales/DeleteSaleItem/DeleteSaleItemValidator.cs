using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSaleItem;

public class DeleteSaleItemValidator : AbstractValidator<DeleteSaleItemCommand>
{
    public DeleteSaleItemValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required");
    }
}
