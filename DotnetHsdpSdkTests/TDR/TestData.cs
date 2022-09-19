namespace DotnetHsdpSdkTests.TDR;

public static class TestData
{
    public static string DataItemsBundleJson = @"
    {
        ""type"": ""searchset"",
        ""total"": 1,
        ""entry"": [
            {
                ""fullUrl"": ""https://blabla.com/store/tdr/DataItem?organization=Xyz&user=%7C123abc&_id=abcdef1234567890"",
                ""resource"": {
                    ""deleteTimestamp"": ""2022-12-16T05:53:23.750Z"",
                    ""id"": ""1234567890"",
                    ""meta"": {
                        ""versionId"": ""1"",
                        ""lastUpdated"": ""2022-09-16T05:53:23.750Z""
                    },
                    ""organization"": ""Xyz"",
                    ""dataType"": {
                        ""system"": ""Tenant1"",
                        ""code"": ""FHIR-1""
                    },
                    ""timestamp"": ""2022-09-16T05:53:22.969Z"",
                    ""user"": {
                        ""system"": """",
                        ""value"": ""123abc""
                    },
                    ""device"": {
                        ""system"": """",
                        ""value"": ""0987654321""
                    },
                    ""data"": {
                        ""key"": ""value""
                    },
                    ""creationTimestamp"": ""2022-09-16T05:53:23.750Z"",
                    ""resourceType"": ""DataItem""
                }
            }
        ],
        ""link"": [
            {
                ""relation"": ""next"",
                ""url"": ""https://blabla.com/store/tdr/DataItem?organization=Xyz&_count=1&_startAt=3""
            }
        ],
        ""_startAt"": 2,
        ""resourceType"": ""Bundle""
    }
    ";
}
