using Pulumi;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Lambda;
using Pulumi.Aws.ApiGateway;

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

        LambdaArn = lambda.Arn;

        var apiGateway = new RestApi("scalesGateway", new RestApiArgs
        {
            Name = "ScalesGateway",
            Description = "An API gateway for returning musical scales in a random order",
            Policy = GetApiGatewayPolicy(),
        });

        var apiResource = new Pulumi.Aws.ApiGateway.Resource("scalesAPI", new Pulumi.Aws.ApiGateway.ResourceArgs
        {
            RestApi = apiGateway.Id,
            PathPart = "{proxy+}",
            ParentId = apiGateway.RootResourceId,
        });

        var apiMethod = new Method("scalesGET", new MethodArgs
        {
            HttpMethod = "GET",
            Authorization = "NONE",
            RestApi = apiGateway.Id,
            ResourceId = apiResource.Id,
        });

        _ = new Integration("scalesLambdaIntegration", new IntegrationArgs
        {
            HttpMethod = "GET",
            IntegrationHttpMethod = "GET",
            ResourceId = apiResource.Id,
            RestApi = apiGateway.Id,
            Type = "AWS_PROXY",
            Uri = lambda.InvokeArn,
        });
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

    private static string GetApiGatewayPolicy()
    {
        return @"{
            ""Version"": ""2012-10-17"",
            ""Statement"": [
                {
                    ""Action"": ""sts:AssumeRole"",
                    ""Principal"": {
                        ""Service"": ""lambda.amazonaws.com""
                    },
                    ""Effect"": ""Allow"",
                    ""Sid"": """"
                },
                {
                    ""Action"": ""execute-api:Invoke"",
                    ""Resource"": ""*"",
                    ""Principal"": ""*"",
                    ""Effect"": ""Allow"",
                    ""Sid"": """"
                }
            ]
        }";
    }
}
