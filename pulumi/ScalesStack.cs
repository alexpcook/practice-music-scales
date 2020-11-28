using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Lambda;
using Pulumi.Aws.ApiGateway;

class ScalesStack : Stack
{
    [Output]
    public Output<string> LambdaArn { get; set; }
    [Output]
    public Output<string> GatewayUrl { get; set; }
    
    public ScalesStack()
    {
        var account = Output.Create(GetCallerIdentity.InvokeAsync());
        var accountId = account.Apply(a => a.AccountId);
        var region = Output.Create(GetRegion.InvokeAsync());
        var regionName = region.Apply(r => r.Name);

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
            PathPart = "scales",
            ParentId = apiGateway.RootResourceId,
        }, new CustomResourceOptions
        {
            DependsOn = {apiGateway},
        });

        var apiMethod = new Method("scalesGET", new MethodArgs
        {
            HttpMethod = "GET",
            Authorization = "NONE",
            RestApi = apiGateway.Id,
            ResourceId = apiResource.Id,
        }, new CustomResourceOptions
        {
            DependsOn = {apiGateway, apiResource},
        });

        var apiIntegration = new Integration("scalesLambdaIntegration", new IntegrationArgs
        {
            HttpMethod = "GET",
            IntegrationHttpMethod = "POST",
            ResourceId = apiResource.Id,
            RestApi = apiGateway.Id,
            Type = "AWS_PROXY",
            Uri = lambda.InvokeArn,
        }, new CustomResourceOptions
        {
            DependsOn = {apiGateway, apiResource, lambda},
        });

        var apiPermission = new Permission("scalesAPIPermission", new PermissionArgs
        {
            Action = "lambda:InvokeFunction",
            Function = lambda.Name,
            Principal = "apigateway.amazonaws.com",
            SourceArn = Output.Format($"arn:aws:execute-api:{regionName}:{accountId}:{apiGateway.Id}/*/*/*")
        }, new CustomResourceOptions
        {
            DependsOn = {apiGateway, apiResource, lambda},
        });

        var apiDeployment = new Pulumi.Aws.ApiGateway.Deployment("scalesDeployment", new DeploymentArgs
        {
            Description = "Scales API deployment",
            RestApi = apiGateway.Id,
            StageDescription = "Development",
            StageName = "dev",
        }, new CustomResourceOptions
        {
            DependsOn = {apiGateway, apiResource, lambda, apiPermission},
        });

        GatewayUrl = Output.Format($"https://{apiGateway.Id}.execute-api.{regionName}.amazonaws.com/{apiDeployment.StageName}/");
    }

    private static string LambdaGoZipFilePath { get; } = "../lambda/dist/handler.zip";
    private static string LambdaGoEntryPoint { get; } = "dist/handler";

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
