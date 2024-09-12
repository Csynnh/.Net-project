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

    public CustomerReview CreateCustomerReview(int idTaiKhoan, int idHangHoa, string noiDung, int danhGia, DateTime ngayNhanXet)
    {
        return _customerReviewRepository.CreateCustomerReview(idTaiKhoan, idHangHoa, noiDung, danhGia, ngayNhanXet);
    }

    public CustomerReview UpdateCustomerReview(int idNhanXet, int idTaiKhoan, int idHangHoa, string noiDung, int danhGia, DateTime ngayNhanXet)
    {
        return _customerReviewRepository.UpdateCustomerReview(idNhanXet, idTaiKhoan, idHangHoa, noiDung, danhGia, ngayNhanXet);
    }

    public void DeleteCustomerReview(int idNhanXet)
    {
        var result = _customerReviewRepository.DeleteCustomerReview(idNhanXet);
        if (!result)
        {
            throw new Exception("Could not delete customer review");
        }
    }
}
