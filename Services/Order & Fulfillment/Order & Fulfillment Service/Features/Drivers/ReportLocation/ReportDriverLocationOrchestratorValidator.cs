using FluentValidation;

namespace Order___Fulfillment_Service.Features.Drivers.ReportLocation
{
    public class ReportDriverLocationOrchestratorValidator
    : AbstractValidator<ReportDriverLocationOrchestrator>
    {
        public ReportDriverLocationOrchestratorValidator()
        {
            RuleFor(x => x.DriverId)
                .NotEmpty()
                .WithMessage("DriverId is required.");
            RuleFor(x => x.Lat)
                .InclusiveBetween(-90.0, 90.0)
                .WithMessage("Latitude must be between -90 and 90.");
            RuleFor(x => x.Lng)
                .InclusiveBetween(-180.0, 180.0)
                .WithMessage("Longitude must be between -180 and 180.");
            RuleFor(x => x.RecordedAt)
                .NotEmpty()
                .WithMessage("RecordedAt timestamp is required.")
                .LessThanOrEqualTo(_ => DateTime.UtcNow.AddMinutes(5))
                .WithMessage("RecordedAt cannot be in the future.");
        }
    }
}
