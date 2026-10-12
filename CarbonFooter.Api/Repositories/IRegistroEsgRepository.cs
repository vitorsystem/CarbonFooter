using CarbonFooter.Api.Models;
using MongoDB.Driver;

namespace CarbonFooter.Api.Repositories;

public interface IRegistroEsgRepository
{
    Task<List<RegistroEsg>> ListarTodosAsync();
    Task<RegistroEsg?> BuscarPorIdAsync(string id);
    Task<RegistroEsg> SalvarAsync(RegistroEsg registro);
    Task DeletarAsync(string id);
}

public class RegistroEsgRepository : IRegistroEsgRepository
{
    private readonly IMongoCollection<RegistroEsg> _collection;

    public RegistroEsgRepository(IMongoClient mongoClient, string databaseName)
    {
        var database = mongoClient.GetDatabase(databaseName);
        _collection = database.GetCollection<RegistroEsg>("registros_esg");
    }

    public async Task<List<RegistroEsg>> ListarTodosAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<RegistroEsg?> BuscarPorIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<RegistroEsg> SalvarAsync(RegistroEsg registro)
    {
        if (string.IsNullOrEmpty(registro.Id))
        {
            await _collection.InsertOneAsync(registro);
        }
        else
        {
            await _collection.ReplaceOneAsync(x => x.Id == registro.Id, registro);
        }
        return registro;
    }

    public async Task DeletarAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);
}