using CarbonFooter.Api.Models;
using CarbonFooter.Api.Repositories;

namespace CarbonFooter.Api.Services;

public class RegistroEsgService
{
    private readonly IRegistroEsgRepository _repository;

    public RegistroEsgService(IRegistroEsgRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RegistroEsg>> ListarTodosAsync() =>
        await _repository.ListarTodosAsync();

    public async Task<RegistroEsg?> BuscarPorIdAsync(string id) =>
        await _repository.BuscarPorIdAsync(id);

    public async Task<RegistroEsg> SalvarAsync(RegistroEsg registro)
    {
        if (registro.EmissoesCo2Kg == null && registro.ConsumoKwh != null)
        {
            registro.EmissoesCo2Kg = registro.ConsumoKwh * 0.085;
        }

        return await _repository.SalvarAsync(registro);
    }

    public async Task DeletarAsync(string id) =>
        await _repository.DeletarAsync(id);
}