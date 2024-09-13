using System.ComponentModel.DataAnnotations;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using infrastructure.Repositories;

namespace service;

public class AccountService
{
    private readonly AccountRepository _accountRepository;

    public AccountService(AccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public IEnumerable<Account> GetAccounts()
    { 
        return _accountRepository.GetAccounts();
    }

    public Account CreateAccount(string tenDangNhap, string matKhau, string hoTen, string email, string soDienThoai, string diaChi, string vaiTro)
    {
        var doesAccountExist = _accountRepository.DoesAccountWithUsernameExist(tenDangNhap);
        if (doesAccountExist)
        {
            throw new ValidationException("Account already exists with username " + tenDangNhap);
        }

        return _accountRepository.CreateAccount(tenDangNhap, matKhau, hoTen, email, soDienThoai, diaChi, vaiTro);
    }

    public Account UpdateAccount(int idTaiKhoan, string tenDangNhap, string matKhau, string hoTen, string email, string soDienThoai, string diaChi, string vaiTro)
    {
        return _accountRepository.UpdateAccount(idTaiKhoan, tenDangNhap, matKhau, hoTen, email, soDienThoai, diaChi, vaiTro);
    }

    public void DeleteAccount(int idTaiKhoan)
    {
        var result = _accountRepository.DeleteAccount(idTaiKhoan);
        if (!result)
        {
            throw new Exception("Could not delete account");
        }
    }
}
