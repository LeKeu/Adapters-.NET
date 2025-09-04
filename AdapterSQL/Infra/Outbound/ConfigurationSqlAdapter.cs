using Adapter.OutBound.AdapterSql.Repository;
using Adapter.OutBound.AdapterSQL.Settings;

namespace Infra.OutBound
{
    public static class ConfigurationSqlAdapter
    {
        public static void AddSQLAdapter(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.Configure<ConnectionSql>(options =>
            {
                var section = configuration.GetSection("DB_Conn");
                options.Cluster = Environment.GetEnvironmentVariable("DB_CLUSTER_SERVER") ?? section["Cluster"] ?? string.Empty;
                options.Username = Environment.GetEnvironmentVariable("DB_USER") ?? section["Username"] ?? string.Empty;
                options.Password = Environment.GetEnvironmentVariable("DB_CRIPT_PASSWORD") ?? section["Password"] ?? string.Empty;
                options.Database = Environment.GetEnvironmentVariable("DB_DB_NAME") ?? section["Database"] ?? string.Empty;
                
                options.CommandTimeout = section.GetValue<int>("CommandTimeout", 30); // valor padrão se não existir
                options.ConnectTimeout = section.GetValue<int>("ConnectTimeout", 10); // valor padrão se não existir
            });

            services.AddScoped<IRepository, Repository>();
        }
    }
}
