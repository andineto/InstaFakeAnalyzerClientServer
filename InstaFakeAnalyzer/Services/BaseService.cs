using Microsoft.AspNetCore.Http;
using MySqlConnector;

namespace InstaFakeAnalyzer.Services
{
    public partial class Service
    {
        protected MySqlConnection Connection => _session.Connection;
        protected MySqlTransaction? Transaction => _session.Transaction;
        private readonly IDbSession _session;
        private readonly GeminiService _geminiService;


        public Service(GeminiService geminiService)
        {
            _geminiService = geminiService ?? throw new ArgumentNullException(nameof(geminiService));
            if (string.IsNullOrWhiteSpace(Program.connectionStringMySql))
            {
                throw new InvalidOperationException("Connection string não configurada");
            }
            _session = new DbSession(Program.connectionStringMySql);
        }

        protected async Task BeginTransactionAsync()
        {
            if (_session.Transaction == null)
            {
                await _session.BeginTransactionAsync();
            }
        }

        protected async Task CommitAsync()
        {
            if (_session.Transaction != null)
            {
                await _session.CommitAsync();
            }
        }

        protected async Task RollbackAsync()
        {
            if (_session.Transaction != null)
            {
                await _session.RollbackAsync();
            }
        }
    }
}
