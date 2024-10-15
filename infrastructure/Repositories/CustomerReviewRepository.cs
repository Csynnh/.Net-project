using Dapper;
using infrastructure.DataModels;
using infrastructure.EnumVariables;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;

public class CustomerReviewRepository
{
    private NpgsqlDataSource _dataSource;

    public CustomerReviewRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<CustomerReview> GetCustomerReviewForFeed()
    {
        var sql = $@"
SELECT id as {nameof(CustomerReviewFeedQuery.id)}, 
       account_id as {nameof(CustomerReviewFeedQuery.account_id)}, 
       product_id as {nameof(CustomerReviewFeedQuery.product_id)}, 
       content as {nameof(CustomerReviewFeedQuery.content)}, 
       vote as {nameof(CustomerReviewFeedQuery.vote)}, 
       created_at as {nameof(CustomerReviewFeedQuery.created_date)}
FROM customerreviews;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<CustomerReview>(sql);
        }
    }

    public CustomerReview CreateCustomerReview(Guid accountId, Guid productId, string content, Rating vote, DateTime createdAt)
    {
        var sql = $@"
INSERT INTO customerreviews (account_id, product_id, content, vote, created_at)
VALUES (@accountId, @productId, @content, @vote, @createdAt)
RETURNING id as {nameof(CustomerReview.id)}, 
          account_id as {nameof(CustomerReview.account_id)}, 
          product_id as {nameof(CustomerReview.product_id)}, 
          content as {nameof(CustomerReview.content)}, 
          vote as {nameof(CustomerReview.vote)}, 
          created_at as {nameof(CustomerReview.created_date)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<CustomerReview>(sql, new { accountId, productId, content, vote, createdAt });
        }
    }

    public CustomerReview UpdateCustomerReview(Guid reviewId, string content, Rating vote, DateTime createdAt)
    {
        var sql = $@"
UPDATE customerreviews
SET content = @content, vote = @vote, created_at = @createdAt
WHERE id = @reviewId
RETURNING id as {nameof(CustomerReview.id)}, 
          account_id as {nameof(CustomerReview.account_id)}, 
          product_id as {nameof(CustomerReview.product_id)}, 
          content as {nameof(CustomerReview.content)}, 
          vote as {nameof(CustomerReview.vote)}, 
          created_at as {nameof(CustomerReview.created_date)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<CustomerReview>(sql, new { reviewId, content, vote, createdAt });
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
