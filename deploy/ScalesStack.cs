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

        var lambda = new Function("lambda", new FunctionArgs
        {
            Runtime = "go1.x",
            Code = new FileArchive(LambdaGoZipFilePath),
            Handler = LambdaGoEntryPoint,
            Role = CreateLambdaRole().Arn,
        });

        var s3 = new Bucket("s3bucket", new BucketArgs
        {
            Acl = "private",
        });

        foreach (string fileType in StaticFileTypes)
        {
            string id = "s3object-" + fileType;
            _ = new BucketObject(id, new BucketObjectArgs
            {
                Key = id,
                Bucket = s3.Id,
                Source = new FileAsset(StaticContentDirectory + fileType + "/site." + fileType),
                ContentType = (fileType == "js") ? "text/javascript" : "text/" + fileType,
            }, new CustomResourceOptions
            {
                Parent = s3,
            });
        }

        var gateway = new RestApi("gateway", new RestApiArgs
        {
            Name = "practice-music-scales",
            Description = "an api gateway that returns musical scales in a random order",
            Policy = GetApiGatewayPolicy(),
        });

        var apiResource = new Pulumi.Aws.ApiGateway.Resource("resource-api", new Pulumi.Aws.ApiGateway.ResourceArgs
        {
            RestApi = gateway.Id,
            PathPart = "api",
            ParentId = gateway.RootResourceId,
        }, new CustomResourceOptions
        {
            DependsOn = { gateway },
            Parent = gateway,
        });

        var scalesResource = new Pulumi.Aws.ApiGateway.Resource("resource-scales", new Pulumi.Aws.ApiGateway.ResourceArgs
        {
            RestApi = gateway.Id,
            PathPart = "scales",
            ParentId = apiResource.Id,
        }, new CustomResourceOptions
        {
            DependsOn = { gateway, apiResource },
            Parent = apiResource,
        });

        var scalesResourceMethod = new Method("method-get", new MethodArgs
        {
            HttpMethod = "GET",
            Authorization = "NONE",
            RestApi = gateway.Id,
            ResourceId = scalesResource.Id,
        }, new CustomResourceOptions
        {
            DependsOn = { gateway, apiResource, scalesResource },
            Parent = scalesResource,
        });

        var scalesResourceIntegration = new Integration("integration-get", new IntegrationArgs
        {
            HttpMethod = "GET",
            IntegrationHttpMethod = "POST",
            ResourceId = scalesResource.Id,
            RestApi = gateway.Id,
            Type = "AWS_PROXY",
            Uri = lambda.InvokeArn,
        }, new CustomResourceOptions
        {
            DependsOn = { gateway, apiResource, scalesResource, lambda },
            Parent = scalesResource,
        });

        var lambdaPermission = new Permission("gateway-permission", new PermissionArgs
        {
            Action = "lambda:InvokeFunction",
            Function = lambda.Name,
            Principal = "apigateway.amazonaws.com",
            SourceArn = Output.Format($"arn:aws:execute-api:{regionName}:{accountId}:{gateway.Id}/*/{scalesResourceMethod.HttpMethod}{scalesResource.Path}")
        }, new CustomResourceOptions
        {
            DependsOn = { gateway, apiResource, scalesResource, lambda },
            Parent = lambda,
        });

        var deployment = new Pulumi.Aws.ApiGateway.Deployment("deployment-dev", new DeploymentArgs
        {
            Description = "Scales API deployment",
            RestApi = gateway.Id,
            StageDescription = "Development",
            StageName = "dev",
        }, new CustomResourceOptions
        {
            DependsOn = { gateway, apiResource, scalesResource, lambda, lambdaPermission },
            Parent = gateway,
        });

        GatewayUrl = Output.Format($"https://{gateway.Id}.execute-api.{regionName}.amazonaws.com/{deployment.StageName}/");
    }

    private static string LambdaGoZipFilePath { get; } = "../app/api/handler.zip";
    private static string LambdaGoEntryPoint { get; } = "handler";
    private static string StaticContentDirectory { get; } = "../app/static/";
    private static string[] StaticFileTypes { get; } = new string[3] { "html", "css", "js" };

    private static Role CreateLambdaRole()
    {
        var lambdaRole = new Role("iam-role", new RoleArgs
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

        _ = new RolePolicy("iam-role-policy", new RolePolicyArgs
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
        }, new CustomResourceOptions
        {
            Parent = lambdaRole,
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
