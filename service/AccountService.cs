using System.ComponentModel.DataAnnotations;
using infrastructure.DataModels;
using infrastructure.EnumVariables;
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

    public IEnumerable<Account> GetAccountForFeed()
    { 
        return _accountRepository.GetAccountForFeed();
    }

    public Account CreateAccount(string username , string password , string name, string email, string phone_number, Role role )
    {
        var doesAccountExist = _accountRepository.DoesAccountWithUsernameExist(username);
        if (doesAccountExist)
        {
            throw new ValidationException("Account already exists with username " + username );
        }

        return _accountRepository.CreateAccount(username , password , name, email, phone_number, role );
    }

    public Account UpdateAccount(Guid id, string username , string password , string name, string email, string phone_number, Role role )
    {
        return _accountRepository.UpdateAccount(id, username , password , name, email, phone_number , role );
    }

    public void DeleteAccount(Guid id)
    {
        var result = _accountRepository.DeleteAccount(id);
        if (!result)
        {
            throw new Exception("Could not delete account");
        }
    }
}
