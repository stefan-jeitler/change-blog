namespace ChangeBlog.Management.Api.DTOs.V1;

public record CultureDto(string Culture, string Language, string Country, string ShortDateFormat, ushort FirstDayOfWeek);