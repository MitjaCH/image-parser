public class Asset
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public List<string> SupportedFileTypes { get; set; } = new List<string>();
    public List<string> SupportedSizes { get; set; } = new List<string>();
}