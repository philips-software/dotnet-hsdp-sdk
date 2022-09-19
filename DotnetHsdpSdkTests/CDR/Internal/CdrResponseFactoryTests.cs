using System.Net;
using System.Text.Json;
using DotnetHsdpSdk.CDR.Internal;
using DotnetHsdpSdk.Utils;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using NUnit.Framework;

namespace DotnetHsdpSdkTests.CDR.Internal;

[TestFixture]
public class CdrResponseFactoryTests
{
    private readonly ICdrResponseFactory _factory = new CdrResponseFactory();
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        .ForFhir(typeof(Bundle).Assembly)
        .ForFhir(typeof(Observation).Assembly)
        .ForFhir(typeof(Device).Assembly)
        .ForFhir(typeof(Patient).Assembly);

    #region Read
    
    [Test]
    public void CreateReadResponseForSuccessfulReadShouldReturnStatus200AndRequestedResource()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: JsonSerializer.Serialize(TestData.Observation, _options)
        );
        
        var response = _factory.CreateCdrReadResponse(hsdpResponse);
        
        Assert.AreEqual(200, response.Status);
        Assert.IsInstanceOf<Observation>(response.Resource);
    }
    
    [Test]
    public void CreateReadResponseForFailingReadShouldReturnStatusCodeAndOperationOutcome()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.Unauthorized,
            headers: new List<KeyValuePair<string, string>>(),
            body: JsonSerializer.Serialize(TestData.TokenExpiredOperationOutcome, _options)
        );
        
        var response = _factory.CreateCdrReadResponse(hsdpResponse);
        
        Assert.AreEqual(401, response.Status);
        Assert.IsInstanceOf<OperationOutcome>(response.Resource);
    }
    
    [Test]
    public void CreateReadResponseForMissingResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: null!
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrReadResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreateReadResponseForUnexpectedFieldsInResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""unexpected"":""json response""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrReadResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreateReadResponseForInvalidJsonResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""invalid json format""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrReadResponse(hsdpResponse);
        });
    }
    
    #endregion
    
    #region Search
    
    [Test]
    public void CreateSearchResponseForSuccessfulReadShouldReturnStatus200AndRequestedResource()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: JsonSerializer.Serialize(TestData.TransactionBundle, _options)
        );
        
        var response = _factory.CreateCdrSearchResponse(hsdpResponse);
        
        Assert.AreEqual(200, response.Status);
        Assert.IsInstanceOf<Bundle>(response.Bundle);
        Assert.IsNull(response.OperationOutcome);
    }
    
    [Test]
    public void CreateSearchResponseForFailingReadShouldReturnStatusCodeAndOperationOutcome()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.Unauthorized,
            headers: new List<KeyValuePair<string, string>>(),
            body: JsonSerializer.Serialize(TestData.TokenExpiredOperationOutcome, _options)
        );
        
        var response = _factory.CreateCdrSearchResponse(hsdpResponse);
        
        Assert.AreEqual(401, response.Status);
        Assert.IsNull(response.Bundle);
        Assert.IsInstanceOf<OperationOutcome>(response.OperationOutcome);
    }
    
    [Test]
    public void CreateSearchResponseForMissingResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: null!
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrSearchResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreateSearchResponseForUnexpectedFieldsInResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""unexpected"":""json response""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrSearchResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreateSearchResponseForInvalidJsonResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""invalid json format""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrReadResponse(hsdpResponse);
        });
    }
    
    
    #endregion
    
    #region Create
    
    [Test]
    public void CreateCreateResponseForSuccessfulReadShouldReturnStatus200AndRequestedResource()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>
            {
                KeyValuePair.Create("Location", "location"),
                KeyValuePair.Create("ETag", "etag"),
                KeyValuePair.Create("Last-Modified", "lastModified")
            },
            body: JsonSerializer.Serialize(TestData.Observation, _options)
        );
        
        var response = _factory.CreateCdrCreateResponse(hsdpResponse);
        
        Assert.AreEqual(200, response.Status);
        Assert.IsInstanceOf<Observation>(response.Resource);
        Assert.AreEqual("location", response.Location);
        Assert.AreEqual("etag", response.ETag);
        Assert.AreEqual("lastModified", response.LastModified);
    }
    
    [Test]
    public void CreateCreateResponseForFailingReadShouldReturnStatusCodeAndOperationOutcome()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.Unauthorized,
            headers: new List<KeyValuePair<string, string>>(),
            body: JsonSerializer.Serialize(TestData.TokenExpiredOperationOutcome, _options)
        );
        
        var response = _factory.CreateCdrCreateResponse(hsdpResponse);
        
        Assert.AreEqual(401, response.Status);
        Assert.IsInstanceOf<OperationOutcome>(response.Resource);
        Assert.IsNull(response.Location);
        Assert.IsNull(response.ETag);
        Assert.IsNull(response.LastModified);
    }
    
    [Test]
    public void CreateCreateResponseForMissingResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: null!
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrCreateResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreateCreateResponseForUnexpectedFieldsInResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""unexpected"":""json response""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrCreateResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreateCreateResponseForInvalidJsonResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""invalid json format""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrCreateResponse(hsdpResponse);
        });
    }
    
    #endregion
    
    #region BatchOrTransaction
    
    [Test]
    public void CreateBatchOrTransactionResponseForSuccessfulReadShouldReturnStatus200AndRequestedResource()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: JsonSerializer.Serialize(TestData.TransactionBundle, _options)
        );
        
        var response = _factory.CreateCdrBatchOrTransactionResponse(hsdpResponse);
        
        Assert.AreEqual(200, response.Status);
        Assert.IsInstanceOf<Bundle>(response.Bundle);
        Assert.IsNull(response.OperationOutcome);
    }
    
    [Test]
    public void CreateBatchOrTransactionResponseForFailingReadShouldReturnStatusCodeAndOperationOutcome()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.Unauthorized,
            headers: new List<KeyValuePair<string, string>>(),
            body: JsonSerializer.Serialize(TestData.TokenExpiredOperationOutcome, _options)
        );
        
        var response = _factory.CreateCdrBatchOrTransactionResponse(hsdpResponse);
        
        Assert.AreEqual(401, response.Status);
        Assert.IsNull(response.Bundle);
        Assert.IsInstanceOf<OperationOutcome>(response.OperationOutcome);
    }
    
    [Test]
    public void CreateBatchOrTransactionResponseForMissingResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: null!
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrBatchOrTransactionResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreateBatchOrTransactionResponseForUnexpectedFieldsInResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""unexpected"":""json response""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrBatchOrTransactionResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreateBatchOrTransactionResponseForInvalidJsonResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""invalid json format""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrBatchOrTransactionResponse(hsdpResponse);
        });
    }
    
    #endregion
    
    #region Delete
    
    [Test]
    public void CreateDeleteResponseForSuccessfulReadShouldReturnStatus200AndRequestedResource()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: null!
        );
        
        var response = _factory.CreateCdrDeleteResponse(hsdpResponse);
        
        Assert.AreEqual(200, response.Status);
        Assert.IsNull(response.OperationOutcome);
    }
    
    [Test]
    public void CreateDeleteResponseForFailingReadShouldReturnStatusCodeAndOperationOutcome()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.Unauthorized,
            headers: new List<KeyValuePair<string, string>>(),
            body: JsonSerializer.Serialize(TestData.TokenExpiredOperationOutcome, _options)
        );
        
        var response = _factory.CreateCdrDeleteResponse(hsdpResponse);
        
        Assert.AreEqual(401, response.Status);
        Assert.IsInstanceOf<OperationOutcome>(response.OperationOutcome);
    }
    
    [Test]
    public void CreateDeleteResponseForMissingResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.Unauthorized,    // Use Unauthorized because OK response has no body
            headers: new List<KeyValuePair<string, string>>(),
            body: null!
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrDeleteResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreateDeleteResponseForUnexpectedFieldsInResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.Unauthorized,    // Use Unauthorized because OK response has no body
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""unexpected"":""json response""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrDeleteResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreateDeleteResponseForInvalidJsonResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.Unauthorized,    // Use Unauthorized because OK response has no body
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""invalid json format""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrDeleteResponse(hsdpResponse);
        });
    }
    
    #endregion
    
    #region Update
    
    [Test]
    public void CreateUpdateResponseForSuccessfulReadShouldReturnStatus200AndRequestedResource()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>
            {
                KeyValuePair.Create("Location", "location"),
                KeyValuePair.Create("ETag", "etag"),
                KeyValuePair.Create("Last-Modified", "lastModified")
            },
            body: JsonSerializer.Serialize(TestData.Observation, _options)
        );
        
        var response = _factory.CreateCdrUpdateResponse(hsdpResponse);
        
        Assert.AreEqual(200, response.Status);
        Assert.IsInstanceOf<Observation>(response.Resource);
        Assert.AreEqual("location", response.Location);
        Assert.AreEqual("etag", response.ETag);
        Assert.AreEqual("lastModified", response.LastModified);
    }
    
    [Test]
    public void CreateUpdateResponseForFailingReadShouldReturnStatusCodeAndOperationOutcome()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.Unauthorized,
            headers: new List<KeyValuePair<string, string>>(),
            body: JsonSerializer.Serialize(TestData.TokenExpiredOperationOutcome, _options)
        );
        
        var response = _factory.CreateCdrUpdateResponse(hsdpResponse);
        
        Assert.AreEqual(401, response.Status);
        Assert.IsInstanceOf<OperationOutcome>(response.Resource);
        Assert.IsNull(response.Location);
        Assert.IsNull(response.ETag);
        Assert.IsNull(response.LastModified);
    }
    
    [Test]
    public void CreateUpdateResponseForMissingResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: null!
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrUpdateResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreateUpdateResponseForUnexpectedFieldsInResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""unexpected"":""json response""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrUpdateResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreateUpdateResponseForInvalidJsonResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""invalid json format""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrUpdateResponse(hsdpResponse);
        });
    }
    
    #endregion
    
    #region Patch
    
    [Test]
    public void CreatePatchResponseForSuccessfulReadShouldReturnStatus200AndRequestedResource()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>
            {
                KeyValuePair.Create("Location", "location"),
                KeyValuePair.Create("ETag", "etag"),
                KeyValuePair.Create("Last-Modified", "lastModified")
            },
            body: JsonSerializer.Serialize(TestData.Observation, _options)
        );
        
        var response = _factory.CreateCdrPatchResponse(hsdpResponse);
        
        Assert.AreEqual(200, response.Status);
        Assert.IsInstanceOf<Observation>(response.Resource);
        Assert.AreEqual("location", response.Location);
        Assert.AreEqual("etag", response.ETag);
        Assert.AreEqual("lastModified", response.LastModified);
    }
    
    [Test]
    public void CreatePatchResponseForFailingReadShouldReturnStatusCodeAndOperationOutcome()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.Unauthorized,
            headers: new List<KeyValuePair<string, string>>(),
            body: JsonSerializer.Serialize(TestData.TokenExpiredOperationOutcome, _options)
        );
        
        var response = _factory.CreateCdrPatchResponse(hsdpResponse);
        
        Assert.AreEqual(401, response.Status);
        Assert.IsInstanceOf<OperationOutcome>(response.Resource);
        Assert.IsNull(response.Location);
        Assert.IsNull(response.ETag);
        Assert.IsNull(response.LastModified);
    }
    
    [Test]
    public void CreatePatchResponseForMissingResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: null!
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrPatchResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreatePatchResponseForUnexpectedFieldsInResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""unexpected"":""json response""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrPatchResponse(hsdpResponse);
        });
    }
    
    [Test]
    public void CreatePatchResponseForInvalidJsonResponseBodyShouldThrow()
    {
        var hsdpResponse = new HsdpResponse<string>(
            statusCode: HttpStatusCode.OK,
            headers: new List<KeyValuePair<string, string>>(),
            body: @"{""invalid json format""}"
        );

        Assert.Throws<HsdpRequestException>(() =>
        {
            _factory.CreateCdrPatchResponse(hsdpResponse);
        });
    }
    
    #endregion
    
}
