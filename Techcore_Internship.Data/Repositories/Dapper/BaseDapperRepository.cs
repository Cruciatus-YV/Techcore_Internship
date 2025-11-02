using Microsoft.Extensions.Configuration;
using Techcore_Internship.Data.Repositories.Dapper.Interfaces;

namespace Techcore_Internship.Data.Repositories.Dapper;

public class BaseDapperRepository : IBaseDapperRepository
{
    private protected readonly string _connectionString;
    public BaseDapperRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetSection("ConnectionStrings").GetChildren().First(x => x.Key.StartsWith("Techcore_Internship_Postgres_Connection")).Value!;
    }

    //public async Task GetAll()
    //{
    //    var cons = _connectionString;
    //}
}
