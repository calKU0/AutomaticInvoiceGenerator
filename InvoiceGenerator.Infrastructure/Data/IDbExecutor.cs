using System.Data;

namespace InvoiceGenerator.Infrastructure.Data
{
    public interface IDbExecutor
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CommandType? commandType = null);
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, CommandType? commandType = null);
        Task<TFirst?> QuerySingleOrDefaultAsync<TFirst, TSecond>(string sql, Func<TFirst, TSecond, TFirst> map, string splitOn, object? param = null, CommandType? commandType = null);
        Task<TFirst?> QuerySingleOrDefaultAsync<TFirst, TSecond, TThird>(string sql, Func<TFirst, TSecond, TThird, TFirst> map, string splitOn, object? param = null, CommandType? commandType = null);
        Task<int> ExecuteAsync(string sql, object? param = null, CommandType? commandType = null);
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, string splitOn, object? param = null, CommandType? commandType = null);
    }
}