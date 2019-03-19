using FluentValidation;

namespace Kanbanboard.ViewModels.Auth.Validations
{
    public class RegistrationViewModelValidator : AbstractValidator<RegistrationViewModel>
    {
        public RegistrationViewModelValidator()
        {
            RuleFor(vm => vm.Username).NotEmpty().WithMessage("Username cannot be empty");
            RuleFor(vm => vm.Password).NotEmpty().WithMessage("Password cannot be empty");
            RuleFor(vm => vm.Email).NotEmpty().WithMessage("Email cannot be empty");
        }
    }
}