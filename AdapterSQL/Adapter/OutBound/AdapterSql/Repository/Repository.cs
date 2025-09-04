using Adapter.OutBound.AdapterSQL.Settings;
using System.Data;
using Dapper;

namespace Adapter.OutBound.AdapterSql.Repository
{
    public class Repository : IRepository
    {
        private readonly IOptions<ConnectionSql> _connection;

        public Repository(IOptions<ConnectionSql> connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task<string> ExecutarProcedure(object transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            using var conn = _connection.Value.ConnectDataBase("DATABASE_NAME");

            var parameters = new DynamicParameters();

            #region Inputs
            parameters.Add("@vchparam1", "transaction.param1", DbType.String, ParameterDirection.Input, size: 8);
            parameters.Add("@intparam2", "transaction.param2", DbType.Int16, ParameterDirection.Input);
            parameters.Add("@intparam3", "transaction.param3", DbType.Int32, ParameterDirection.Input);
            #endregion

            #region Outputs
            parameters.Add("out1", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: int.MaxValue);
            parameters.Add("ou2", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            #endregion

            // execução da sp
            await conn.QueryAsync("SP_NAME", parameters, commandType: CommandType.StoredProcedure);

            // pegar os retornos dos outputs da sp
            var out1 = parameters.Get<string>("vchout1");
            var out2 = parameters.Get<Int32?>("intout2") ?? 0;

            //retornar o necessário
            return "";

            // nesse caso não tem try catch, pois é uma situação em que p try catch esá no método que chama o executeprocedure
        }
    }
}
