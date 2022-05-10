using System.ComponentModel.DataAnnotations;
using ChangeBlog.Api.Localization.Resources;

namespace ChangeBlog.Management.Api.DTOs.V1;

public enum ApiKeyExpirationIntervals
{
    [Display(Name = "OneWeek", ResourceType = typeof(ChangeBlogStrings))]
    OneWeek,
    [Display(Name = "OneMonth", ResourceType = typeof(ChangeBlogStrings))]
    OneMonth,
    [Display(Name = "SixMonths", ResourceType = typeof(ChangeBlogStrings))]
    SixMonth,
    [Display(Name = "OneYear", ResourceType = typeof(ChangeBlogStrings))]
    OneYear,
    [Display(Name = "TwoYears", ResourceType = typeof(ChangeBlogStrings))]
    TowYears
}