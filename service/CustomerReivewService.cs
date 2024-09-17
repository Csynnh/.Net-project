using System.ComponentModel.DataAnnotations;
using infrastructure.DataModels;
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

    public IEnumerable<CustomerReview> GetCustomerReviews()
    {
        return _customerReviewRepository.GetCustomerReviews();
    }

    public CustomerReview CreateCustomerReview(int account_id, int product_id, string content, int vote, DateTime created_date)
    {
        return _customerReviewRepository.CreateCustomerReview(account_id, product_id, content, vote, created_date);
    }

    public CustomerReview UpdateCustomerReview(int id, int account_id, int product_id, string content, int vote, DateTime created_date)
    {
        return _customerReviewRepository.UpdateCustomerReview(id, account_id, product_id, content, vote, created_date);
    }

    public void DeleteCustomerReview(int id)
    {
        var result = _customerReviewRepository.DeleteCustomerReview(id);
        if (!result)
        {
            throw new Exception("Could not delete customer review");
        }
    }
}
