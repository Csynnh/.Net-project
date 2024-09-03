namespace infrastructure.QueryModels;

public class BookFeedQuery
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string CoverImgUrl { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
}