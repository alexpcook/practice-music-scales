using Pulumi;
using Pulumi.Aws.S3;

class Lambda : Stack
{
    public Lambda()
    {
        // Create an AWS resource (S3 Bucket)
        var bucket = new Bucket("my-bucket");

        // Export the name of the bucket
        this.BucketName = bucket.Id;
    }

    [Output]
    public Output<string> BucketName { get; set; }
}
