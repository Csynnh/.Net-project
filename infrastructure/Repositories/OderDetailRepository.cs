using Dapper;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;

public interface IOderDetailRepository
{
    Task<OderDetailResponse> CreateOderDetail(OderDetailRequest oderDetail);
}

public class OderDetailRepository: IOderDetailRepository
{
    private NpgsqlDataSource _dataSource;

    public OderDetailRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<OderDetailResponse> CreateOderDetail(OderDetailRequest oderDetail)
    {
        using var conn = _dataSource.OpenConnection();

        var sql = @"
            INSERT INTO NOIRTEST.ORDERDETAILS (order_id, product_variant_id, quantity, price)
            VALUES (@OrderId, @ProductVariantId, @Quantity, @Price)
            RETURNING id, order_id, product_variant_id, quantity, price
        ";
        return await conn.QuerySingleAsync<OderDetailResponse>(sql, new {
            OrderId = oderDetail.order_id,
            ProductVariantId = oderDetail.product_variant_id,
            Quantity = oderDetail.quantity,
            Price = oderDetail.price
        });
    }
}