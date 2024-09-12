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
    [Route("/api/reviews")]
    public ResponseDto Get()
    {
        HttpContext.Response.StatusCode = 314;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _customerReviewService.GetCustomerReviews()
        };
    }

    [HttpPost]
    [ValidateModel]
    [Route("/api/reviews")]
    public ResponseDto Post([FromBody] CreateReviewRequestDto dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
            MessageToClient = "Successfully created a review",
            ResponseData = _customerReviewService.CreateCustomerReview(dto.ID_TaiKhoan, dto.ID_HangHoa, dto.NoiDung, dto.DanhGia, dto.NgayNhanXet)
        };
    }

    [HttpPut]
    [ValidateModel]
    [Route("/api/reviews/{id}")]
    public ResponseDto Put([FromRoute] int id, [FromBody] UpdateReviewRequestDto dto)
    {
        HttpContext.Response.StatusCode = 201;
        return new ResponseDto()
        {
            MessageToClient = "Successfully updated",
            ResponseData = _customerReviewService.UpdateCustomerReview(id, dto.ID_TaiKhoan, dto.ID_HangHoa, dto.NoiDung, dto.DanhGia, dto.NgayNhanXet)
        };
    }

    [HttpDelete]
    [Route("/api/reviews/{id}")]
    public ResponseDto Delete([FromRoute] int id)
    {
        _customerReviewService.DeleteCustomerReview(id);
        return new ResponseDto()
        {
            MessageToClient = "Successfully deleted"
        };
    }
}
