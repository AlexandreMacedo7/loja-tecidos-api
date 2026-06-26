namespace LojaTecidos.Api.Extensions;

internal static class DataHoraLoja
{
    private static readonly TimeZoneInfo Fuso = TimeZoneInfo.FindSystemTimeZoneById(
        OperatingSystem.IsWindows() ? "Central Brazilian Standard Time" : "America/Manaus");

    public static DateTime Agora => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Fuso);
}
