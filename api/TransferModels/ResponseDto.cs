namespace api.TransferModels;

public class ResponseDto
{
    public string MessageToClient { get; set; } = string.Empty;
    public Object? ResponseData { get; set; }
}