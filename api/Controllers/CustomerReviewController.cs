using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using api.Filters;
using api.TransferModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using service;

namespace library.Controllers;

public class CustomerReviewController : ControllerBase
{
    private readonly ILogger<CustomerReviewController> _logger;
    private readonly CustomerReviewService _customerReviewService;

    public CustomerReviewController(ILogger<CustomerReviewController> logger, CustomerReviewService customerReviewService)
    {
        _logger = logger;
        _customerReviewService = customerReviewService;
    }

    [HttpGet]
    [Route("/api/customer_reviews")]
    public ResponseDto Get()
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _customerReviewService.GetCustomerReviewForFeed()
        };
    }

    [HttpPost]
    [ValidateModel]
    [Route("/api/customer_reviews")]
    public ResponseDto Post([FromBody] CreateCustomerReviewRequestDto dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
            MessageToClient = "Successfully created a review",
            ResponseData = _customerReviewService.CreateCustomerReview(dto.account_id , dto.product_id , dto.content, dto.vote, dto.created_date)
        };
    }

    [HttpPut]
    [ValidateModel]
    [Route("/api/customer_reviews/{id}")]
    public ResponseDto Put([FromRoute] Guid id, [FromBody] UpdateCustomerReviewRequestDto dto)
    {
        HttpContext.Response.StatusCode = 201;
        return new ResponseDto()
        {
            MessageToClient = "Successfully updated",
            ResponseData = _customerReviewService.UpdateCustomerReview(id, dto.account_id , dto.product_id , dto.content, dto.vote, dto.created_date)
        };
    }

    [HttpDelete]
    [Route("/api/customer_reviews/{id}")]
    public ResponseDto Delete([FromRoute] Guid id)
    {
        _customerReviewService.DeleteCustomerReview(id);
        return new ResponseDto()
        {
            MessageToClient = "Successfully deleted"
        };
    }
}
