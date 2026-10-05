namespace GestCredit.Api.Dtos;

public record CreateDemandeCreditDto(int ClientId, decimal Montant, decimal TauxAnnuel, int DureeMois);
