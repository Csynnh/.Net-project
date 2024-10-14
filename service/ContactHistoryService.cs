using System;
using System.Collections.Generic;
using infrastructure.DataModels;
using infrastructure.Repositories;

namespace service;

public class ContactService
{
    private readonly ContactHistoryRepository _contactRepository;

    public ContactService(ContactHistoryRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public IEnumerable<ContactHistory> GetContactHistoryForFeed()
    {
        return _contactRepository.GetContactHistoryForFeed();
    }

    public ContactHistory CreateContactHistory(Guid account_id, string contact_details)
    {
        return _contactRepository.CreateContactHistory(account_id, contact_details);
    }

    public ContactHistory UpdateContactHistory(Guid contactHistoryId, string contactDetails)
    {
        return _contactRepository.UpdateContactHistory(contactHistoryId, contactDetails);
    }

    public void DeleteContactHistory(Guid contactHistoryId)
    {
        var result = _contactRepository.DeleteContactHistory(contactHistoryId);
        if (!result)
        {
            throw new Exception("Could not delete contact");
        }
    }
}
