namespace infrastructure.DataModels;

public class Book
{
    public string BookTitle { get; set; } = string.Empty;
    public int BookId { get; set; }
    public string CoverImgUrl { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
}