using System;
using System.Collections.Generic;
using infrastructure.DataModels;
using infrastructure.Repositories;

namespace service;

public class CartService
{
    private readonly CartRepository _cartRepository;

    public CartService(CartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public void CreateCart(Guid account_id, Guid product_id, int quantity)
    {
        try
        {
            _cartRepository.CreateCart(account_id, product_id, quantity);
        }
        catch (Exception ex) // Catch other general exceptions
        {
            // Handle other errors such as general exceptions or unexpected errors
            throw new Exception(ex.Message);
        }
    }


    public IEnumerable<CartInQueryResult> GetCartForFeed(Guid account_id)
    {
        try
        {
            return _cartRepository.GetListCart(account_id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public void UpdateCart(Guid cart_id, int quantity)
    {
        try
        {
            _cartRepository.UpdateCart(cart_id, quantity);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    public void DeleteCart(Guid cart_id)
    {
        try
        {
            _cartRepository.DeleteCart(cart_id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
