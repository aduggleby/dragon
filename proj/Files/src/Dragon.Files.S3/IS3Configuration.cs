namespace Dragon.Files.S3
{
    public interface IS3Configuration
    {
        string AccessKeyID { get; }
        string AccessKeySecret { get; }
        string Bucket { get; }
        string Region { get; }

    }
}