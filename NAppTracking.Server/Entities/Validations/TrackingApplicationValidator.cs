namespace NAppTracking.Server.Entities.Validations
{
    using FluentValidation;

    public class TrackingApplicationValidator : AbstractValidator<TrackingApplication>
    {
        public TrackingApplicationValidator()
        {
            this.RuleFor(x => x.Name).NotEmpty();
            this.RuleFor(x => x.ApiKey).NotEmpty();
        }
    }
}