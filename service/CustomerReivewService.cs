using System.ComponentModel.DataAnnotations;
using infrastructure.DataModels;
using infrastructure.EnumVariables;
using infrastructure.QueryModels;
using infrastructure.Repositories;

namespace service;

public class CustomerReviewService
{
    private readonly CustomerReviewRepository _customerReviewRepository;

    public CustomerReviewService(CustomerReviewRepository customerReviewRepository)
    {
        _customerReviewRepository = customerReviewRepository;
    }

    public IEnumerable<CustomerReview> GetCustomerReviewForFeed()
    {
        return _customerReviewRepository.GetCustomerReviewForFeed();
    }

    public CustomerReview CreateCustomerReview(Guid account_id, Guid product_id, string content, Rating vote, DateTime created_date)
    {
        return _customerReviewRepository.CreateCustomerReview(account_id, product_id, content, vote, created_date);
    }

    public CustomerReview UpdateCustomerReview(Guid id, Guid account_id, Guid product_id, string content, Rating vote, DateTime created_date)
    {
        return _customerReviewRepository.UpdateCustomerReview(id, content, vote, created_date);
    }

    public void DeleteCustomerReview(Guid id)
    {
        var result = _customerReviewRepository.DeleteCustomerReview(id);
        if (!result)
        {
            throw new Exception("Could not delete customer review");
        }
    }
}
