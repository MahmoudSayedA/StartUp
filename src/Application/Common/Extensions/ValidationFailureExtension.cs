using Application.Common.Exceptions;
using FluentValidation.Results;

namespace Application.Common.Extensions;
public static class ValidationFailureExtension
{
    public static List<ErrorDetail> ToErrorDetailsList(this List<ValidationFailure> validationFailure)
    {
        return validationFailure.Select(v => new ErrorDetail(v.PropertyName, v.ErrorMessage)).ToList();
    }
}
