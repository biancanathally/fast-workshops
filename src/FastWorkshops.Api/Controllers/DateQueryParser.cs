using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FastWorkshops.Api.Controllers;

internal static class DateQueryParser
{
    private const string Formato = "yyyy-MM-dd";

    public static bool TentarConverter(
        string? entrada, ModelStateDictionary modelState, out DateOnly? resultado)
    {
        resultado = null;
        if (string.IsNullOrWhiteSpace(entrada)) return true;

        if (!DateOnly.TryParseExact(entrada, Formato,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            modelState.AddModelError("data",
                $"Formato inválido. Utilize {Formato} (ex.: 2025-06-12).");
            return false;
        }

        resultado = parsed;
        return true;
    }
}
