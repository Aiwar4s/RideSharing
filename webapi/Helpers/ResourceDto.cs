namespace webapi.Helpers;

public record ResourceDto<T>(T resource, IReadOnlyCollection<LinkDto> Links);