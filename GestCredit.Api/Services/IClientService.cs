using GestCredit.Api.Dtos;

namespace GestCredit.Api.Services;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetClientsAsync();
    Task<ClientDto?> GetClientAsync(int id);
    Task<ClientDto> CreateClientAsync(CreateClientDto dto);
    Task<bool> UpdateClientAsync(int id, UpdateClientDto dto);
    Task<bool> DeleteClientAsync(int id);
}