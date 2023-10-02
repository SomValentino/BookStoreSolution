using FluentValidation;

namespace Order.API.Application.Commands.UpdateOrder {
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand> {
        public UpdateOrderCommandValidator () {
            RuleFor (p => p.UserName)
                .NotEmpty ().WithMessage ("{UserName} is required.")
                .NotNull ();
                
            RuleFor (p => p.EmailAddress)
                .NotEmpty ().WithMessage ("{EmailAddress} is required.");

            RuleFor (p => p.TotalPrice)
                .NotEmpty ().WithMessage ("{TotalPrice} is required.")
                .GreaterThan (0).WithMessage ("{TotalPrice} should be greater than zero.");
        }
    }
}