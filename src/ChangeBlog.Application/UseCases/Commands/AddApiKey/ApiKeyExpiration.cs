using System;

namespace ChangeBlog.Application.UseCases.Commands.AddApiKey;

public enum ApiKeyExpiration
{
    OneWeek,
    OneMonth,
    SixMonths,
    OneYear,
    TwoYears
}