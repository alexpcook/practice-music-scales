using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Lambda;
using Pulumi.Aws.ApiGateway;
using Pulumi.Aws.S3;

class ScalesStack : Stack
{
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

        var s3 = new Bucket("scalesS3", new BucketArgs
        {
            Acl = "private",
        });

        foreach (string fileType in StaticFileTypes)
        {
            string id = "scales" + fileType;
            _ = new BucketObject(id, new BucketObjectArgs
            {
                Key = id,
                Bucket = s3.Id,
                Source = new FileAsset(StaticContentDirectory + fileType + "/site." + fileType),
                ContentType = (fileType == "js") ? "text/javascript" : "text/" + fileType,
            });
        }

        var apiGateway = new RestApi("scalesGateway", new RestApiArgs
        {
            Name = "ScalesGateway",
            Description = "An API gateway for returning musical scales in a random order",
            Policy = GetApiGatewayPolicy(),
        });

        var apiMethod = new Method("scalesGET", new MethodArgs
        {
            HttpMethod = "GET",
            Authorization = "NONE",
            RestApi = apiGateway.Id,
            ResourceId = apiGateway.RootResourceId,
        }, new CustomResourceOptions
        {
            DependsOn = {apiGateway},
        });

        var apiIntegration = new Integration("scalesLambdaIntegration", new IntegrationArgs
        {
            HttpMethod = "GET",
            IntegrationHttpMethod = "POST",
            ResourceId = apiGateway.RootResourceId,
            RestApi = apiGateway.Id,
            Type = "AWS_PROXY",
            Uri = lambda.InvokeArn,
        }, new CustomResourceOptions
        {
            DependsOn = {apiGateway, lambda},
        });

        var apiPermission = new Permission("scalesAPIPermission", new PermissionArgs
        {
            Action = "lambda:InvokeFunction",
            Function = lambda.Name,
            Principal = "apigateway.amazonaws.com",
            SourceArn = Output.Format($"arn:aws:execute-api:{regionName}:{accountId}:{apiGateway.Id}/*/{apiMethod.HttpMethod}/*")
        }, new CustomResourceOptions
        {
            DependsOn = {apiGateway, lambda},
        });

        var apiDeployment = new Pulumi.Aws.ApiGateway.Deployment("scalesDeployment", new DeploymentArgs
        {
            Description = "Scales API deployment",
            RestApi = apiGateway.Id,
            StageDescription = "Development",
            StageName = "dev",
        }, new CustomResourceOptions
        {
            DependsOn = {apiGateway, lambda, apiPermission},
        });

        GatewayUrl = Output.Format($"https://{apiGateway.Id}.execute-api.{regionName}.amazonaws.com/{apiDeployment.StageName}/");
    }

    private static string LambdaGoZipFilePath { get; } = "../app/api/handler.zip";
    private static string LambdaGoEntryPoint { get; } = "handler";
    private static string StaticContentDirectory { get; } = "../app/static/";
    private static string[] StaticFileTypes { get; } = new string[3] { "html", "css", "js" };

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
