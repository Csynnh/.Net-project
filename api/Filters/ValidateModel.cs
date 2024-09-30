using api.TransferModels;
using infrastructure.DataModels;
using library.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.Filters;

public class ValidateModel : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            string errorMessages = context.ModelState
                .Values
                .SelectMany(i => i.Errors.Select(e => e.ErrorMessage))
                .Aggregate((i, j) => i + "," + j);
            context.Result = new JsonResult(new ResponseDto
            {
                MessageToClient = errorMessages
            })
            {
                StatusCode = 400
            };
            return;
        }

        // Validate color field in product_request param
        if (context.ActionArguments.TryGetValue("product_request", out var productRequestObj) && productRequestObj is ProductRequest productRequest)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(productRequest.color, "^#[0-9A-Fa-f]{6}$"))
            {
                context.Result = new JsonResult(new ResponseDto
                {
                    MessageToClient = "Invalid color. It must be a hexadecimal color code starting with #."
                })
                {
                    StatusCode = 400
                };
                return;
            }
        }

        base.OnActionExecuting(context);
    }
}