namespace GestCredit.Api.Dtos;

// Reçu via [FromQuery] : GET /api/clients?ville=Douala&triPar=ville&page=2&pageSize=5
public class ClientQueryParameters
{
    public string? Ville { get; set; }
    public string? TriPar { get; set; } = "Nom"; // "Nom" ou "Ville"
    public bool TriDescendant { get; set; } = false;

    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
