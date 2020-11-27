using Pulumi;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Lambda;

class Lambda : Stack
{
    private static string LambdaGoZipFilePath { get; } = "../lambda/dist/handler.zip";
    private static string LambdaGoEntryPoint { get; } = "dist/handler";

    public Lambda()
    {
        var lambda = new Function("scalesLambda", new FunctionArgs
        {
            Runtime = "go1.x",
            Code = new FileArchive(LambdaGoZipFilePath),
            Handler = LambdaGoEntryPoint,
            Role = CreateLambdaRole().Arn,
        });

        this.LambdaArn = lambda.Arn;
    }

    [Output]
    public Output<string> LambdaArn { get; set; }
    
    private static Role CreateLambdaRole()
    {
        var lambdaRole = new Role("lambdaRole", new RoleArgs
        {
            AssumeRolePolicy = @"{
                ""Version"": ""2012-10-17"",
                ""Statement"": [{
                    ""Action"": ""sts:AssumeRole"",
                    ""Principal"": {
                        ""Service"": ""lambda.amazonaws.com""
                    },
                    ""Effect"": ""Allow"",
                    ""Sid"": """"
                }]
            }"
        });

        var logPolicy = new RolePolicy("lambdaLogPolicy", new RolePolicyArgs
        {
            Role = lambdaRole.Id,
            Policy = @"{
                ""Version"": ""2012-10-17"",
                ""Statement"": [{
                    ""Effect"": ""Allow"",
                    ""Action"": [
                        ""logs:CreateLogGroup"",
                        ""logs:CreateLogStream"",
                        ""logs:PutLogEvents""
                    ],
                    ""Resource"": ""arn:aws:logs:*:*:*""
                }]
            }"
        });

        return lambdaRole;
    }
}
