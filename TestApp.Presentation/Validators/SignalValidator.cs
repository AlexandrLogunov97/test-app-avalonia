using FluentValidation;
using TestApp.Application.Services;
using TestApp.Presentation.Models;

namespace TestApp.Presentation.Validators;

public class SignalValidator : AbstractValidator<SignalModel>
{
    public SignalValidator(IDataTypesService dataTypesService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name should be not empty!");
        RuleFor(x => x.Shape).NotEmpty().WithMessage("Shape should be selected!");
        RuleFor(x => x.Value)
            .Must((model, value) => BeValueInRangeForDataType(model.DataType, value))
            .WithMessage((model, value) => GetRangeDescription(model.DataType))
            .When(x => !string.IsNullOrEmpty(x.DataType));
        RuleFor(x => x.Value)
            .Must(value => value % 1 == 0)
            .WithMessage("Value should be Int32")
            .When(x => x.DataType == "Int32");
        RuleFor(x => x.Value)
            .Must(value => value == 0 || value == 1)
            .WithMessage("Should be true or false")
            .When(x => x.DataType == "Boolean");
        RuleFor(x => x.DataType).NotEmpty().WithMessage("Data type should be not empty!");
    }

    private bool BeValueInRangeForDataType(string dataType, double value)
    {
        var range = dataType switch
        {
            "Int32" => (Min: (double)int.MinValue, Max: (double)int.MaxValue),
            "Double" => (Min: double.MinValue, Max: double.MaxValue),
            "Boolean" => (Min: 0.0, Max: 1.0),
            _ => (Min: double.MinValue, Max: double.MaxValue)
        };

        return value >= range.Min && value <= range.Max;
    }

    private string GetRangeDescription(string dataType)
    {
        return dataType switch
        {
            "Int32" => $"{int.MinValue} - {int.MaxValue}",
            "Double" => $"{double.MinValue} - {double.MaxValue}",
            "Boolean" => "0 or 1",
            _ => ""
        };
    }
}