using System.Security.Cryptography;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Adapter.OutBound.AdapterSQL.Settings
{
    public record ConnectionSql
    {
        public string Cluster { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int CommandTimeout { get; set; } = 30;
        public int ConnectTimeout { get; set; } = 10;

        private readonly string _key = "MinhaChaveSecreta123456789012345";
        private readonly string _iv = "A1B2C3D4E5F6G7H8";

        public IDbConnection ConnectDataBase(string dataBaseName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(dataBaseName);

            var decryptedPassword = DecryptAES(Password, _key, _iv);

            var connectionString = $"Data Source={Cluster};" +
                                  $"Initial Catalog={dataBaseName};" +
                                  $"User ID={Username};" +
                                  $"Password={decryptedPassword};" +
                                  $"Connect Timeout={ConnectTimeout};" +
                                  "Persist Security Info=False;" + // talvez não seja preciso
                                  "MultipleActiveResultSets=true;";

            return new SqlConnection(connectionString);
        }

        private string DecryptAES(string encryptedText, string key, string iv)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(encryptedText);

            try
            {
                using var aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32)[..32]); 
                aes.IV = Encoding.UTF8.GetBytes(iv.PadRight(16)[..16]);

                using var decryptor = aes.CreateDecryptor();
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to decrypt password", ex);
            }
        }
    }

}
