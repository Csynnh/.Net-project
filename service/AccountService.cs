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

    public Account CreateAccount(string username , string password , string name, string email, string phone_number, string role )
    {
        var doesAccountExist = _accountRepository.DoesAccountWithUsernameExist(username );
        if (doesAccountExist)
        {
            throw new ValidationException("Account already exists with username " + username );
        }

        return _accountRepository.CreateAccount(username , password , name, email, phone_number, role );
    }

    public Account UpdateAccount(int id, string username , string password , string name, string email, string phone_number, string role )
    {
        return _accountRepository.UpdateAccount(id, username , password , name, email, phone_number , role );
    }

    public void DeleteAccount(int id)
    {
        var result = _accountRepository.DeleteAccount(id);
        if (!result)
        {
            throw new Exception("Could not delete account");
        }
    }
}
