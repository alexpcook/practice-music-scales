using Pulumi;
using Pulumi.Aws.Iam;
using Pulumi.Aws.ApiGateway;

class Gateway
{
    public Gateway()
    {
        var gateway = new RestApi("scalesGateway", new RestApiArgs
        {
            Name = "ScalesGateway",
            Description = "An API gateway for returning musical scales in a random order",
            Policy = GetApiGatewayPolicy(),
        });
    }

    private static Input<string> GetApiGatewayPolicy()
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
