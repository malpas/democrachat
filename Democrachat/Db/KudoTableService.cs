using System.Collections.Generic;
using Democrachat.Db.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Democrachat.Db
{
    public class KudoTableService : IKudoTableService
    {
        private IConfiguration _config;

        public KudoTableService(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<KudoListing> GetPossibleKudoItems()
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            return conn.Query<KudoListing>("SELECT * from kudo_item");
        }
    }
}