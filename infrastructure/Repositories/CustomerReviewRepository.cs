using Dapper;
using infrastructure.DataModels;
using Npgsql;

namespace infrastructure.Repositories;
public class CustomerReviewRepository
{
    private NpgsqlDataSource _dataSource;

    public CustomerReviewRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<CustomerReview> GetAllCustomerReviews()
    {
        var sql = @"
SELECT id as Id, account_id as AccountId, product_id as ProductId, content as Content, 
       vote as Vote, created_at as CreatedAt
FROM customerreviews;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<CustomerReview>(sql);
        }
    }

    public CustomerReview CreateCustomerReview(Guid accountId, Guid productId, string content, int vote)
    {
        var sql = @"
INSERT INTO customerreviews (account_id, product_id, content, vote)
VALUES (@accountId, @productId, @content, @vote)
RETURNING id as Id, account_id as AccountId, product_id as ProductId, 
          content as Content, vote as Vote, created_at as CreatedAt;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<CustomerReview>(sql, new { accountId, productId, content, vote });
        }
    }

    public CustomerReview UpdateCustomerReview(Guid reviewId, string content, int vote)
    {
        var sql = @"
UPDATE customerreviews
SET content = @content, vote = @vote
WHERE id = @reviewId
RETURNING id as Id, account_id as AccountId, product_id as ProductId, 
          content as Content, vote as Vote, created_at as CreatedAt;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<CustomerReview>(sql, new { reviewId, content, vote });
        }
    }

    public bool DeleteCustomerReview(Guid reviewId)
    {
        var sql = @"DELETE FROM customerreviews WHERE id = @reviewId;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { reviewId }) == 1;
        }
    }
}