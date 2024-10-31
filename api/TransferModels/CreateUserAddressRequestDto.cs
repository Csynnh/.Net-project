
namespace infrastructure.DataModels;

public class CreateUserAddressRequestDto
{
    public Guid account_id { get; set; }
    public UserInformationRequest address  { get; set; }
}