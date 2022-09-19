using System.Net;
using DotnetHsdpSdk.TDR.Internal;
using DotnetHsdpSdk.Utils;
using NUnit.Framework;

namespace DotnetHsdpSdkTests.TDR.Internal;

[TestFixture]
public class TdrResponseFactoryTests
{
    private readonly ITdrResponseFactory _factory = new TdrResponseFactory();
    
    #region SearchData
    
    [Test]
    public void CreateSearchDataResponseForSuccessfulSearchShouldReturnStatus200AndTheSearchResults()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>
            {
                KeyValuePair.Create("HSDP-Request-ID", "3456")
            },
            body: TestData.DataItemsBundleJson
        );
        
        var response = _factory.CreateTdrSearchDataResponse(hsdpResponse);
        
        Assert.Multiple(() =>
        {
            Assert.AreEqual(200, response.Status);
            Assert.IsInstanceOf<List<DotnetHsdpSdk.TDR.DataItem>>(response.DataItems);
            Assert.AreEqual(1, response.DataItems.Count);
            Assert.AreEqual("Xyz", response.DataItems[0].Organization);
            Assert.AreEqual(1, response.Pagination.Limit);
            Assert.AreEqual(3, response.Pagination.Offset);
            Assert.AreEqual("3456", response.RequestId);
        });
    }

    [Test]
    public void CreateSearchDataResponseForUnauthorizedResponseShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.Unauthorized,
            headers: new List<KeyValuePair<string, string>>(),
            body: null!
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateTdrSearchDataResponse(hsdpResponse);
        });
    }

    [Test]
    public void CreateSearchDataResponseForNonCompliantJsonResponseShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>
            {
                KeyValuePair.Create("HSDP-Request-ID", "3456")
            },
            body: @"{""foo"":""bar""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateTdrSearchDataResponse(hsdpResponse);
        });
    }

    #endregion
}
