using FluentValidation;

namespace Order.API.Application.Commands.AuthorizeOrder {
    public class AuthorizeOrderCommandValidator : AbstractValidator<AuthorizeOrderCommand> {
        public AuthorizeOrderCommandValidator () {
            RuleFor (p => p.UserId)
                .NotEmpty ().WithMessage ("{UserId} is required.")
                .NotNull ();

            RuleFor (p => p.OrderId)
                .NotEmpty ().WithMessage ("{OrderId} is required.")
                .NotNull ();
        }
    }
}