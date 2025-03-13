using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {
        RuleFor(sale => sale.SaleNumber)
            .NotEmpty()
            .MinimumLength(1).WithMessage("SaleNumber must be at least 1 characters long.")
            .MaximumLength(50).WithMessage("SaleNumber cannot be longer than 50 characters.");

        RuleFor(sale => sale.Customer)
            .NotEmpty()
            .MinimumLength(1).WithMessage("Customer must be at least 1 characters long.")
            .MaximumLength(50).WithMessage("Customer cannot be longer than 50 characters.");

        RuleFor(sale => sale.Branch)
            .NotEmpty()
            .MinimumLength(1).WithMessage("Branch must be at least 1 characters long.")
            .MaximumLength(50).WithMessage("Branch cannot be longer than 50 characters.");

        RuleFor(sale => sale.SaleDate)
            .NotEmpty().WithMessage("SaleDate cannot be empty.");

        RuleFor(sale => sale.Items.Count)
            .NotEmpty().WithMessage("SaleItems cannot be empty.");

        RuleFor(item => item.Items)
            .NotEmpty().WithMessage("The items list cannot be empty")
            .Must(items => items.All(f => f.Quantity > 0))
            .WithMessage("The quantity of all items must be greater than 0");

        RuleFor(sale => sale.Items)
            .NotEmpty().WithMessage("SaleItems cannot be empty.")
            .ForEach(item =>
            {
                item.ChildRules(i =>
                {
                    i.RuleFor(x => x.Quantity)
                        .NotEmpty().WithMessage("The quantity of items must be greater than 0")
                        .GreaterThan(0).WithMessage("The quantity of items must be greater than 0")
                        .LessThanOrEqualTo(20).WithMessage("The quantity of items must be less than 20");
                });
            });
    }
}