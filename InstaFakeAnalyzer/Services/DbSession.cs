using InstaFakeAnalyzer.Services;
using MySqlConnector;
using System;
using System.Threading.Tasks;

namespace InstaFakeAnalyzer.Services
{
    public class DbSession : IDbSession
    {
        public MySqlConnection Connection { get; }
        public MySqlTransaction? Transaction { get; private set; }

        public DbSession(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
            Connection.Open();
        }

        public async Task BeginTransactionAsync()
        {
            if (Transaction == null)
                Transaction = await Connection.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (Transaction != null)
            {
                await Transaction.CommitAsync();
                await Transaction.DisposeAsync();
                Transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (Transaction != null)
            {
                await Transaction.RollbackAsync();
                await Transaction.DisposeAsync();
                Transaction = null;
            }
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            Connection.Dispose();
            Transaction = null;
        }
    }
}
