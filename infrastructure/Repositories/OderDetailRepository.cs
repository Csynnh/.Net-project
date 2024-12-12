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


        var updateInventorySql = $@"
            UPDATE DEV.PRODUCTVARIANTS
            SET Inventory = Inventory - odt.Quantity
            FROM DEV.ORDERDETAILS odt
            WHERE odt.product_variant_id = DEV.PRODUCTVARIANTS.id
        ";

        var sql = @"
            INSERT INTO DEV.ORDERDETAILS (order_id, product_variant_id, quantity, price)
            VALUES (@OrderId, @ProductVariantId, @Quantity, @Price)
            RETURNING id, order_id, product_variant_id, quantity, price
        ";
        await conn.ExecuteAsync(updateInventorySql);
        return await conn.QuerySingleAsync<OderDetailResponse>(sql, new {
            OrderId = oderDetail.order_id,
            ProductVariantId = oderDetail.product_variant_id,
            Quantity = oderDetail.quantity,
            Price = oderDetail.price
        });
    }
}