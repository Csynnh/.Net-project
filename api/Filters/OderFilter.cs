using infrastructure.Contansts;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShopAPI.Filters;

public class OderFilter : ActionFilterAttribute
{
    private readonly UserRepository _userRepository;
    public OderFilter(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        var paymentMethod = context.ActionArguments["paymentMethod"];
        var shippingMethod = context.ActionArguments["shippingMethod"];
        var account_id = context.ActionArguments["account_id"];

        if (!Enum.IsDefined(typeof(PAYMENTMETHOD), paymentMethod))
        {
            throw new Exception($"Payment method should be one of {string.Join(",", Enum.GetNames(typeof(PAYMENTMETHOD)))}");
        }

        if (!Enum.IsDefined(typeof(SHIPPINGMETHOD), shippingMethod))
        {
            throw new Exception($"Shipping method should be one of {string.Join(",", Enum.GetNames(typeof(SHIPPINGMETHOD)))}");
        }

        if (_userRepository.GetUserByAccountIdAsync((Guid)account_id!).Result == null)
        {
            throw new Exception("Account not found");
        }

    }
}