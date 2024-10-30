using Dapper;
using infrastructure.DataModels;
using Npgsql;

namespace infrastructure.Repositories
{
    public class UserAddressRepository
    {
        private NpgsqlDataSource _dataSource;

        public UserAddressRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public IEnumerable<UserAddress> ListUserAddress(Guid accountId)
        {
            var sql = $@"
                SELECT id as {nameof(UserAddress.id)},
                    account_id as {nameof(UserAddress.account_id)},
                    address as {nameof(UserAddress.address)}
                FROM NOIRTEST.USERADDRESS
                WHERE account_id = @accountId
                ;
            ";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.Query<UserAddress>(sql, new { accountId });
            }
        }

        public UserAddress CreateUserAddress(Guid accountId, string address)
        {
            var sql = $@"
                INSERT INTO NOIRTEST.USERADDRESS (account_id, address)
                VALUES (@accountId, @address::json)
                RETURNING id as {nameof(UserAddress.id)},
                        account_id as {nameof(UserAddress.account_id)},
                        address as {nameof(UserAddress.address)};
            ";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.QueryFirst<UserAddress>(sql, new { accountId, address });
            }
        }

        public UserAddress UpdateUserAddress(Guid userAddressId, string address)
        {
            var sql = $@"
                UPDATE NOIRTEST.USERADDRESS
                SET address = @address::json
                WHERE id = @userAddressId
                RETURNING id as {nameof(UserAddress.id)},
                        account_id as {nameof(UserAddress.account_id)},
                        address as {nameof(UserAddress.address)};
            ";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.QueryFirst<UserAddress>(sql, new { userAddressId, address });
            }
        }

        public bool DeleteUserAddress(Guid userAddressId)
        {
            var sql = @"DELETE FROM NOIRTEST.USERADDRESS WHERE id = @userAddressId;";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.Execute(sql, new { userAddressId }) == 1;
            }
        }

        public int GetLastSequence()
        {
            var sql = @"SELECT COUNT(*) FROM NOIRTEST.USERADDRESS;";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.ExecuteScalar<int>(sql);
            }
        }

        public UserAddress GetUserAddressById(Guid userAddressId)
        {
            var sql = $@"
                SELECT id as {nameof(UserAddress.id)},
                    account_id as {nameof(UserAddress.account_id)},
                    address as {nameof(UserAddress.address)}
                FROM NOIRTEST.USERADDRESS
                WHERE id = @userAddressId
                ;
            ";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.QueryFirstOrDefault<UserAddress>(sql, new { userAddressId });
            }
        }
    }
}
