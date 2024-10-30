using infrastructure.EnumVariables;

namespace infrastructure.DataModels;

public class UpdateUserAddressRequestDto
{
    public Guid account_id { get; set; }
    public UserAddressRequest address  { get; set; }
}