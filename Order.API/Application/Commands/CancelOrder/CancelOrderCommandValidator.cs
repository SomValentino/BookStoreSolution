using FluentValidation;

namespace Order.API.Application.Commands.CancelOrder;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand> {
    public CancelOrderCommandValidator () {
        RuleFor (p => p.OrderId)
            .NotEmpty ().WithMessage ("{OrderId} is required.")
            .NotNull ();
        
    }
}