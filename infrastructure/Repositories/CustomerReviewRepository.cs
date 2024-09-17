using Dapper;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;

public class CustomerReviewRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public CustomerReviewRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<CustomerReview> GetCustomerReviews()
    {
        string sql = @"
        SELECT id as {nameof(CustomerReview.id)},
               account_id  as {nameof(CustomerReview.account_id )},
               product_id  as {nameof(CustomerReview.product_id )},
               content as {nameof(CustomerReview.content)},
               vote as {nameof(CustomerReview.vote)},
               created_date as {nameof(CustomerReview.created_date)}
        FROM noir.customer_reviews;
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<CustomerReview>(sql);
        }
    }

    public CustomerReview CreateCustomerReview(int id, int product_id, string content, int vote, DateTime created_date)
    {
        var sql = @"
        INSERT INTO noir.customer_reviews (account_id , product_id , content, vote, created_date)
        VALUES (@id, @product_id, @content, @vote, @created_date)
        RETURNING id as {nameof(CustomerReview.id)},
                   account_id  as {nameof(CustomerReview.account_id )},
                   product_id  as {nameof(CustomerReview.product_id )},
                   content as {nameof(CustomerReview.content)},
                   vote as {nameof(CustomerReview.vote)},
                   created_date as {nameof(CustomerReview.created_date)};
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<CustomerReview>(sql, new { id, product_id, content, vote, created_date });
        }
    }

    public CustomerReview UpdateCustomerReview(int id, int account_id, int product_id, string content, int vote, DateTime created_date)
    {
        var sql = @"
        UPDATE noir.customer_reviews 
        SET account_id  = @id, 
            product_id  = @product_id, 
            content = @content, 
            vote = @vote, 
            created_date = @created_date
        WHERE id = @id
        RETURNING id as {nameof(CustomerReview.id)},
                   account_id  as {nameof(CustomerReview.account_id )},
                   product_id  as {nameof(CustomerReview.product_id )},
                   content as {nameof(CustomerReview.content)},
                   vote as {nameof(CustomerReview.vote)},
                   created_date as {nameof(CustomerReview.created_date)};
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<CustomerReview>(sql, new { id, account_id, product_id, content, vote, created_date });
        }
    }

    public bool DeleteCustomerReview(int id)
    {
        var sql = @"DELETE FROM noir.customer_reviews WHERE id = @id;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { id }) == 1;
        }
    }
}


