using infrastructure.EnumVariables;

namespace infrastructure.DataModels;

public class UpdateUserAddressRequestDto
{
    public Guid account_id { get; set; }
    public UserInformationRequest address  { get; set; }
}