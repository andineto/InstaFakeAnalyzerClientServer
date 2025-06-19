using System;
using System.Threading.Tasks;
using MySqlConnector;

namespace InstaFakeAnalyzer.Services
{
    public interface IDbSession : IDisposable
    {
        MySqlConnection Connection { get; }
        MySqlTransaction? Transaction { get; }
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
