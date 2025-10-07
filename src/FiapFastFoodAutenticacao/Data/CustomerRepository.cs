using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace FiapFastFoodAutenticacao.Data
{
    public class CustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository()
        {
            _connectionString = Environment.GetEnvironmentVariable("DefaultConnection") 
                ?? Environment.GetEnvironmentVariable("RDS_CONNECTION_STRING")
                ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? "server=fastfood-rds-mysql.cdiuseg40rpb.us-east-1.rds.amazonaws.com;port=3306;database=fastfooddb;user=admin;password=admin123;SslMode=Preferred";
        }

        public async Task<CustomerModel?> GetByCpfAsync(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return null;
                
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"SELECT Id, Name, Email, Cpf, CustomerType 
                                 FROM Customers 
                                 WHERE Cpf = @cpf 
                                 LIMIT 1;";
            return await conn.QueryFirstOrDefaultAsync<CustomerModel>(sql, new { cpf });
        }

        public async Task<bool> ExistsCpfAsync(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return false;
                
            using var conn = new MySqlConnection(_connectionString);
            const string sql = "SELECT 1 FROM Customers WHERE Cpf = @cpf LIMIT 1;";
            return await conn.ExecuteScalarAsync<int?>(sql, new { cpf }) != null;
        }

        public async Task AddAsync(CustomerModel model)
        {
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"INSERT INTO Customers (Id, Name, Email, Cpf, CustomerType)
                                 VALUES (@Id, @Name, @Email, @Cpf, @CustomerType);";
            await conn.ExecuteAsync(sql, model);
        }
    }
}
