using Dapper;
using infrastructure.DataModels;
using Npgsql;

namespace infrastructure.Repositories
{
    public class UserStoredInformationRepository
    {
        private NpgsqlDataSource _dataSource;

        public UserStoredInformationRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public IEnumerable<UserStoredInformation> ListUserStoredInformation(Guid accountId)
        {
            var sql = $@"
                SELECT id as {nameof(UserStoredInformation.id)},
                    account_id as {nameof(UserStoredInformation.account_id)},
                    info as {nameof(UserStoredInformation.info)}
                FROM NOIRTEST.USERSTOREDINFOMATION
                WHERE account_id = @accountId
                ;
            ";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.Query<UserStoredInformation>(sql, new { accountId });
            }
        }

        public UserStoredInformation CreateUserStoredInformation(Guid accountId, string info)
        {
            var sql = $@"
                INSERT INTO NOIRTEST.USERSTOREDINFOMATION (account_id, info)
                VALUES (@accountId, @info::json)
                RETURNING id as {nameof(UserStoredInformation.id)},
                        account_id as {nameof(UserStoredInformation.account_id)},
                        info as {nameof(UserStoredInformation.info)};
            ";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.QueryFirst<UserStoredInformation>(sql, new { accountId, info });
            }
        }

        public UserStoredInformation UpdateUserStoredInformation(Guid userInformationId, string info)
        {
            var sql = $@"
                UPDATE NOIRTEST.USERSTOREDINFOMATION
                SET info = @info::json
                WHERE id = @userInformationId
                RETURNING id as {nameof(UserStoredInformation.id)},
                        account_id as {nameof(UserStoredInformation.account_id)},
                        info as {nameof(UserStoredInformation.info)};
            ";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.QueryFirst<UserStoredInformation>(sql, new { userInformationId, info });
            }
        }

        public bool DeleteUserStoredInformation(Guid userStoredInformationId)
        {
            var sql = @"DELETE FROM NOIRTEST.USERSTOREDINFOMATION WHERE id = @userStoredInformationId;";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.Execute(sql, new { userStoredInformationId }) == 1;
            }
        }

        public int GetLastSequence()
        {
            var sql = @"SELECT COUNT(*) FROM NOIRTEST.USERSTOREDINFOMATION;";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.ExecuteScalar<int>(sql);
            }
        }

        public UserStoredInformation GetUserStoredInformationById(Guid userStoredInformationId)
        {
            var sql = $@"
                SELECT id as {nameof(UserStoredInformation.id)},
                    account_id as {nameof(UserStoredInformation.account_id)},
                    info as {nameof(UserStoredInformation.info)}
                FROM NOIRTEST.USERSTOREDINFOMATION
                WHERE id = @userStoredInformationId
                ;
            ";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.QueryFirstOrDefault<UserStoredInformation>(sql, new { userStoredInformationId });
            }
        }
    }
}
