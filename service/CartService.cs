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

    public IEnumerable<Cart> GetCartForFeed()
    {
        return _cartRepository.GetCartForFeed();
    }

    public Cart CreateCart(Guid account_id, Guid product_id, int quantity)
    {
        return _cartRepository.CreateCart( account_id, product_id, quantity);
    }

    public Cart UpdateCart(Guid cart_id, int quantity)
    {
        return _cartRepository.UpdateCart(cart_id, quantity);
    }

    public void DeleteCart(Guid cart_id)
    {
        var result = _cartRepository.DeleteCart(cart_id);
        if (!result)
        {
            throw new Exception("Could not delete cart");
        }
    }
}
