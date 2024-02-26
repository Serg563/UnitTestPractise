using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OrderApi.SignalR.Services.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace OrderApi.SignalR.Services
{
    public class ConnectionMapping
    {
        private readonly Dictionary<string, HashSet<string>> _connections =
            new Dictionary<string, HashSet<string>>();

        private readonly SignalRContext _dbContext;

        public ConnectionMapping(SignalRContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public int Count
        {
            get
            {
                //return _connections.Count;
                return _dbContext.Connections.Count();
            }
        }

        public async Task Add(string key, string connectionId)
        {
            //lock (_connections)
            //{
            //    HashSet<string> connections;
            //    if (!_connections.TryGetValue(key, out connections))
            //    {
            //        connections = new HashSet<string>(); 
            //        _connections.Add(key, connections);
            //    }

            //    lock (connections)
            //    {
            //        connections.Add(connectionId);
            //    }
            //}
            Connections newConn = new Connections { ConnectionId = connectionId, UserId = key };
            await _dbContext.Connections.AddAsync(newConn);
            await _dbContext.SaveChangesAsync();
            //var connection = _dbContext.Connections
            //    .Include(c => c.User)
            //    .FirstOrDefaultAsync(c => c.User.UserId == key);

            //if (connection == null)
            //{
            //    Connections newConn = new Connections { ConnectionId = connectionId,UserId = key};
            //    _dbContext.Connections.Add(newConn);
            //}
            //else
            //{
               
            //}
        }

        public async Task<IEnumerable<string>> GetConnections(string key)
        {
            //HashSet<string> connections;
            //if (_connections.TryGetValue(key, out connections))
            //{
            //    return connections;
            //}

            //return Enumerable.Empty<string>();
            var connection = await _dbContext.Connections
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == key);
            return connection?.User?.Connections.Select(x => x.ConnectionId) ?? Enumerable.Empty<string>();
        }

        public async Task Remove(string key, string connectionId)
        {
            //lock (_connections)
            //{
            //    HashSet<string> connections;
            //    if (!_connections.TryGetValue(key, out connections))
            //    {
            //        return;
            //    }

            //    lock (connections)
            //    {
            //        connections.Remove(connectionId);

            //        //if (connections.Count == 0)
            //        //{
            //        //    _connections.Remove(key);
            //        //}
            //    }
            //}

            //first attempt
            //var connection = await _dbContext.Connections.FirstOrDefaultAsync(x => x.UserId == key);
            //if (connection == null)
            //    return;
            //var connection2 = _dbContext.Connections.Where(x => x.UserId == key);
            //if (connection2.Count() > 1)
            //{
            //    var c = await connection2.FirstOrDefaultAsync();
            //    _dbContext.Connections.Remove(c);
            //    await _dbContext.SaveChangesAsync();
            //}

            //second attempt
            var userConnections = _dbContext.Connections.Where(x => x.UserId == key).ToList();
            //if (userConnections.Count > 1)
            //{
            //    var connectionToRemove = userConnections.FirstOrDefault(c => c.ConnectionId == connectionId);
            //    if (connectionToRemove != null)
            //    {
            //        _dbContext.Connections.Remove(connectionToRemove);
            //        await _dbContext.SaveChangesAsync();
            //    }
            //}
            var userIdParameter = new SqlParameter("@userId", key);
            var connectionIdParameter = new SqlParameter("@connectionId", connectionId);

            await _dbContext.Database.ExecuteSqlRawAsync(
                "DELETE FROM Connections WHERE UserId = @userId AND ConnectionId != @connectionId",
                userIdParameter,
                connectionIdParameter
            );
        }

        public async Task<IEnumerable<string>> GetAllUsers()
        {
            //return _connections.Keys;
            return await _dbContext.Connections.Select(x => x.UserId).ToListAsync();
        }
    }
}
