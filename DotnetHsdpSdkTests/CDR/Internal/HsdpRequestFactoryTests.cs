using DotnetHsdpSdk.CDR;
using DotnetHsdpSdk.CDR.Internal;
using DotnetHsdpSdk.IAM;
using DotnetHsdpSdk.Utils;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using NUnit.Framework;
using Task = System.Threading.Tasks.Task;

namespace DotnetHsdpSdkTests.CDR.Internal;

[TestFixture]
public class HsdpRequestFactoryTests
{
    private const string CdrEndpoint = "https://endpoint.com";
    private const string FhirVersion = "3.0";
    private const string FhirMediaType = "application/fhir+json; charset=UTF-8";
    private const string FullFhirMediaType = $"{FhirMediaType}; fhirVersion={FhirVersion}";
    private readonly HsdpRequestFactory _hsdpRequestFactory = new(new HsdpCdrConfiguration(CdrEndpoint, FhirVersion, FhirMediaType));
    private Mock<IIamToken> _token = null!;

    [SetUp]
    public void Setup()
    {
        _token = new Mock<IIamToken>();
        _token.Setup(f => f.AccessToken).Returns("accessToken");
    }

    #region Read
    
    [Test]
    public void CreationFromACdrReadRequestWithDefaultsShouldReturnAHsdpRequestWithoutTheOptionalParameters()
    {
        var request = _hsdpRequestFactory.Create(new CdrReadRequest(
            resourceType: "Observation",
            id: "ObservationId"
        ), _token.Object);

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri("https://endpoint.com/Observation/ObservationId"), request.Path);
            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", FullFhirMediaType), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "1"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
        });
    }

    [Test]
    public void CreationFromACdrReadRequestWithModifiedSinceTimestampShouldReturnAHsdpRequestWithIfModifiedSinceHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrReadRequest(
            resourceType: "Observation",
            id: "ObservationId",
            modifiedSinceTimestamp: "someTimestamp"
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("If-Modified-Since", "someTimestamp"), request.Headers);
    }

    [Test]
    public void CreationFromACdrReadRequestWithModifiedSinceVersionShouldReturnAHsdpRequestWithIfModifiedSinceHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrReadRequest(
            resourceType: "Observation",
            id: "ObservationId",
            modifiedSinceVersion: "someVersion"
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("If-None-Match", "someVersion"), request.Headers);
    }

    #endregion
    
    #region ReadVersion
    
    [Test]
    public void CreationFromACdrReadVersionRequestWithDefaultsShouldReturnAHsdpRequestWithoutTheOptionalParameters()
    {
        var request = _hsdpRequestFactory.Create(new CdrReadVersionRequest(
            resourceType: "Observation",
            id: "ObservationId",
            versionId: "versionId"
        ), _token.Object);

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri("https://endpoint.com/Observation/ObservationId/_history/versionId"), request.Path);
            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", FullFhirMediaType), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "1"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
        });
    }

    #endregion
    
    #region Search
    
    [Test]
    public void CreationFromACdrSearchRequestWithGetAndWithoutCompartmentShouldReturnAHsdpRequestWithGetMethod()
    {
        var request = _hsdpRequestFactory.Create(new CdrSearchRequest(
            resourceType: "Observation",
            method: SearchMethod.Get
        ), _token.Object);

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri("https://endpoint.com/Observation"), request.Path);
            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual(4, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", FullFhirMediaType), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "1"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Prefer", "handling=strict"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
        });
    }

    [Test]
    public void CreationFromACdrSearchRequestWithGetAndStrictSearchHandlingShouldReturnAHsdpRequestWithStrictHandlingHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrSearchRequest(
            resourceType: "Observation",
            method: SearchMethod.Get,
            handlingPreference: SearchParameterHandling.Strict
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("Prefer", "handling=strict"), request.Headers);
    }

    [Test]
    public void CreationFromACdrSearchRequestWithGetAndLenientSearchHandlingShouldReturnAHsdpRequestWithLenienttHandlingHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrSearchRequest(
            resourceType: "Observation",
            method: SearchMethod.Get,
            handlingPreference: SearchParameterHandling.Lenient
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("Prefer", "handling=lenient"), request.Headers);
    }

    [Test]
    public void CreationFromACdrSearchRequestWithGetAndCompartmentShouldUseTheCompartmentInTheUrl()
    {
        var request = _hsdpRequestFactory.Create(new CdrSearchRequest(
            resourceType: "Observation",
            method: SearchMethod.Get,
            compartment: new Compartment("Patient", "patientId")
        ), _token.Object);

        Assert.AreEqual(new Uri("https://endpoint.com/Patient/patientId/Observation"), request.Path);
    }

    [Test]
    public void CreationFromACdrSearchRequestWithGetAndQueryShouldReturnAHsdpRequestWithGetMethodAndQueryParameters()
    {
        var request = _hsdpRequestFactory.Create(new CdrSearchRequest(
            resourceType: "Observation",
            method: SearchMethod.Get,
            queryParameters: new List<QueryParameter>
            {
                new("key1", "value1"),
                new("key2", "value2")
            }
        ), _token.Object);

        Assert.Multiple(() =>
        {
            Assert.AreEqual(2, request.QueryParameters.Count);
            Assert.Contains(KeyValuePair.Create("key1", "value1"), request.QueryParameters);
            Assert.Contains(KeyValuePair.Create("key2", "value2"), request.QueryParameters);
        });
    }

    [Test]
    public async Task CreationFromACdrSearchRequestWithPostAndWithoutCompartmentShouldReturnAHsdpRequestWithPostMethod()
    {
        var request = _hsdpRequestFactory.Create(new CdrSearchRequest(
            resourceType: "Observation",
            method: SearchMethod.Post
        ), _token.Object);

        var content = await request.Content!.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri("https://endpoint.com/Observation/_search"), request.Path);
            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual(4, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", FullFhirMediaType), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "1"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Prefer", "handling=strict"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
            Assert.AreEqual("application/x-www-form-urlencoded", request.Content?.Headers.ContentType?.MediaType);
            Assert.AreEqual("", content);
        });
    }

    [Test]
    public async Task CreationFromACdrSearchRequestWithPostAndStrictSearchHandlingShouldReturnAHsdpRequestWithStrictHandlingHeader()
    {
        var hsdpRequestFactory = new HsdpRequestFactory(new HsdpCdrConfiguration(CdrEndpoint, FhirVersion, FhirMediaType));
        var request = hsdpRequestFactory.Create(new CdrSearchRequest(
            resourceType: "Observation",
            method: SearchMethod.Post,
            handlingPreference: SearchParameterHandling.Strict
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("Prefer", "handling=strict"), request.Headers);
    }

    [Test]
    public async Task CreationFromACdrSearchRequestWithPostAndLenientSearchHandlingShouldReturnAHsdpRequestWithLenientHandlingHeader()
    {
        var hsdpRequestFactory = new HsdpRequestFactory(new HsdpCdrConfiguration(CdrEndpoint, FhirVersion, FhirMediaType));
        var request = hsdpRequestFactory.Create(new CdrSearchRequest(
            resourceType: "Observation",
            method: SearchMethod.Post,
            handlingPreference: SearchParameterHandling.Lenient
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("Prefer", "handling=lenient"), request.Headers);
    }

    [Test]
    public void CreationFromACdrSearchRequestWithPostAndCompartmentShouldUseTheCompartmentInTheUrl()
    {
        var request = _hsdpRequestFactory.Create(new CdrSearchRequest(
            resourceType: "Observation",
            method: SearchMethod.Post,
            compartment: new Compartment("Patient", "patientId")
        ), _token.Object);

        Assert.AreEqual(new Uri("https://endpoint.com/Patient/patientId/Observation/_search"), request.Path);
    }

    [Test]
    public async Task CreationFromACdrSearchRequestWithPostAndQueryShouldReturnAHsdpRequestWithPostMethodAndFormUrlEncodedParameters()
    {
        var request = _hsdpRequestFactory.Create(new CdrSearchRequest(
            resourceType: "Observation",
            method: SearchMethod.Post,
            queryParameters: new List<QueryParameter>
            {
                new("key1", "value1"),
                new("key2", "value2")
            }
        ), _token.Object);

        var content = await request.Content!.ReadAsStringAsync();
        Assert.AreEqual("key1=value1&key2=value2", content);
    }

    #endregion
    
    #region Create
    
    [Test]
    public async Task CreationFromACdrCreateRequestWithDefaultsShouldReturnAHsdpRequestWithoutTheOptionalParameters()
    {
        var request = _hsdpRequestFactory.Create(new CdrCreateRequest(
            resourceType: "Observation",
            resource: TestData.Observation
        ), _token.Object);

        var content = await request.Content!.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri("https://endpoint.com/Observation"), request.Path);
            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", FullFhirMediaType), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "1"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
            Assert.AreEqual(1 , request.Content?.Headers.Count());
            Assert.AreEqual(FullFhirMediaType , request.Content?.Headers.ContentType?.ToString());
            Assert.AreEqual(725 , content.Length);
        });
    }

    [Test]
    public void CreationFromACdrCreateRequestWithValidationShouldReturnAHsdpRequestWithXValidateResourceHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrCreateRequest(
            resourceType: "Observation",
            resource: TestData.Observation,
            shouldValidate: true
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("X-validate-resource", "true"), request.Headers);
    }

    [Test]
    public void CreationFromACdrCreateRequestWithConditionShouldReturnAHsdpRequestWithIfNoneExistsHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrCreateRequest(
            resourceType: "Observation",
            resource: TestData.Observation,
            condition: "someCondition"
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("If-None-Exists", "someCondition"), request.Headers);
    }

    [Test]
    public void CreationFromACdrCreateRequestWithPreferenceShouldReturnAHsdpRequestWithPreferHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrCreateRequest(
            resourceType: "Observation",
            resource: TestData.Observation,
            returnPreference: ReturnPreference.Representation
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("Prefer", "return=representation"), request.Headers);
    }

    #endregion
    
    #region BatchOrTransaction
    
    [Test]
    public async Task CreationFromACdrBatchOrTransactionRequestWithDefaultsShouldReturnAHsdpRequestWithoutTheOptionalParameters()
    {
        var request = _hsdpRequestFactory.Create(new CdrBatchOrTransactionRequest(
            bundle: TestData.TransactionBundle
        ), _token.Object);

        var content = await request.Content!.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri("https://endpoint.com"), request.Path);
            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", FullFhirMediaType), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "1"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
            Assert.AreEqual(1 , request.Content?.Headers.Count());
            Assert.AreEqual(FullFhirMediaType , request.Content?.Headers.ContentType?.ToString());
            Assert.AreEqual(892 , content.Length);
        });
    }

    [Test]
    public void CreationFromACdrBatchOrTransactionRequestWithPreferenceShouldReturnAHsdpRequestWithPreferHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrBatchOrTransactionRequest(
            bundle: TestData.TransactionBundle,
            returnPreference: ReturnPreference.Representation
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("Prefer", "return=representation"), request.Headers);
    }

    #endregion
    
    #region DeleteById
    
    [Test]
    public void CreationFromACdrDeleteByIdRequestWithDefaultsShouldReturnAHsdpRequest()
    {
        var request = _hsdpRequestFactory.Create(new CdrDeleteByIdRequest(
            resourceType: "Observation",
            id: "ObservationId"
        ), _token.Object);

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri("https://endpoint.com/Observation/ObservationId"), request.Path);
            Assert.AreEqual(HttpMethod.Delete, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", FullFhirMediaType), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "1"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
        });
    }

    #endregion
    
    #region DeleteByQuery
    
    [Test]
    public void CreationFromACdrDeleteByQueryRequestWithDefaultsShouldReturnAHsdpRequest()
    {
        var request = _hsdpRequestFactory.Create(new CdrDeleteByQueryRequest(
            resourceType: "Observation",
            queryParameters: new List<QueryParameter>
            {
                new("key1", "value1"),
                new("key2", "value2")
            }
        ), _token.Object);

        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri("https://endpoint.com/Observation"), request.Path);
            Assert.AreEqual(HttpMethod.Delete, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", FullFhirMediaType), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "1"), request.Headers);
            Assert.AreEqual(2, request.QueryParameters.Count);
            Assert.Contains(KeyValuePair.Create("key1", "value1"), request.QueryParameters);
            Assert.Contains(KeyValuePair.Create("key2", "value2"), request.QueryParameters);
        });
    }

    #endregion
    
    #region UpdateById
    
    [Test]
    public async Task CreationFromACdrUpdateByIdRequestWithDefaultsShouldReturnAHsdpRequest()
    {
        var request = _hsdpRequestFactory.Create(new CdrUpdateByIdRequest(
            resourceType: "Observation",
            id: "ObservationId",
            resource: TestData.Observation
        ), _token.Object);

        var content = await request.Content!.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri("https://endpoint.com/Observation/ObservationId"), request.Path);
            Assert.AreEqual(HttpMethod.Put, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", FullFhirMediaType), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "1"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
            Assert.AreEqual(725, content.Length);
        });
    }

    [Test]
    public void CreationFromACdrUpdateByIdRequestWithConditionShouldReturnAHsdpRequestWithIfNoneExistsHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrUpdateByIdRequest(
            resourceType: "Observation",
            id: "ObservationId",
            resource: TestData.Observation,
            forVersion: "someVersion"
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("If-None-Match", "someVersion"), request.Headers);
    }

    [Test]
    public void CreationFromACdrUpdateByIdRequestWithValidationShouldReturnAHsdpRequestWithXValidateResourceHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrUpdateByIdRequest(
            resourceType: "Observation",
            id: "ObservationId",
            resource: TestData.Observation,
            shouldValidate: true
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("X-validate-resource", "true"), request.Headers);
    }

    [Test]
    public void CreationFromACdrUpdateByIdRequestWithPreferenceShouldReturnAHsdpRequestWithPreferHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrUpdateByIdRequest(
            resourceType: "Observation",
            id: "ObservationId",
            resource: TestData.Observation,
            returnPreference: ReturnPreference.Representation
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("Prefer", "return=representation"), request.Headers);
    }

    #endregion
    
    #region UpdateByQuery
    
    [Test]
    public async Task CreationFromACdrUpdateByQueryRequestWithDefaultsShouldReturnAHsdpRequestWithQueryParameters()
    {
        var request = _hsdpRequestFactory.Create(new CdrUpdateByQueryRequest(
            resourceType: "Observation",
            queryParameters: new List<QueryParameter>
            {
                new("key1", "value1"),
                new("key2", "value2")
            },
            resource: TestData.Observation
        ), _token.Object);

        var content = await request.Content!.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri("https://endpoint.com/Observation"), request.Path);
            Assert.AreEqual(HttpMethod.Put, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", FullFhirMediaType), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "1"), request.Headers);
            Assert.AreEqual(2, request.QueryParameters.Count);
            Assert.Contains(KeyValuePair.Create("key1", "value1"), request.QueryParameters);
            Assert.Contains(KeyValuePair.Create("key2", "value2"), request.QueryParameters);
            Assert.AreEqual(725, content.Length);
        });
    }

    [Test]
    public void CreationFromACdrUpdateByQueryRequestWithConditionShouldReturnAHsdpRequestWithIfNoneExistsHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrUpdateByQueryRequest(
            resourceType: "Observation",
            queryParameters: new List<QueryParameter>
            {
                new("key1", "value1"),
                new("key2", "value2")
            },
            resource: TestData.Observation,
            forVersion: "someVersion"
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("If-None-Match", "someVersion"), request.Headers);
    }

    [Test]
    public void CreationFromACdrUpdateByQueryRequestWithPreferenceShouldReturnAHsdpRequestWithPreferHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrUpdateByQueryRequest(
            resourceType: "Observation",
            queryParameters: new List<QueryParameter>
            {
                new("key1", "value1"),
                new("key2", "value2")
            },
            resource: TestData.Observation,
            returnPreference: ReturnPreference.Representation
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("Prefer", "return=representation"), request.Headers);
    }

    #endregion
    
    #region PatchById
    
    [Test]
    public async Task CreationFromACdrPatchByIdRequestWithDefaultsShouldReturnAHsdpRequest()
    {
        var request = _hsdpRequestFactory.Create(new CdrPatchByIdRequest(
            resourceType: "Observation",
            id: "ObservationId",
            operations: new List<Operation>
            {
                new("a", "b", "c", "d")
            }
        ), _token.Object);

        var content = await request.Content!.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri("https://endpoint.com/Observation/ObservationId"), request.Path);
            Assert.AreEqual(HttpMethod.Patch, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", FullFhirMediaType), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "1"), request.Headers);
            Assert.AreEqual(0, request.QueryParameters.Count);
            Assert.AreEqual(64, content.Length);
        });
    }

    [Test]
    public void CreationFromACdrPatchByIdRequestWithConditionShouldReturnAHsdpRequestWithIfNoneExistsHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrPatchByIdRequest(
            resourceType: "Observation",
            id: "ObservationId",
            operations: new List<Operation>
            {
                new("a", "b", "c", "d")
            },
            forVersion: "someVersion"
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("If-Match", "someVersion"), request.Headers);
    }

    [Test]
    public void CreationFromACdrPatchByIdRequestWithValidationShouldReturnAHsdpRequestWithXValidateResourceHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrPatchByIdRequest(
            resourceType: "Observation",
            id: "ObservationId",
            operations: new List<Operation>
            {
                new("a", "b", "c", "d")
            },
            shouldValidate: true
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("X-validate-resource", "true"), request.Headers);
    }

    [Test]
    public void CreationFromACdrPatchByIdRequestWithPreferenceShouldReturnAHsdpRequestWithPreferHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrPatchByIdRequest(
            resourceType: "Observation",
            id: "ObservationId",
            operations: new List<Operation>
            {
                new("a", "b", "c", "d")
            },
            returnPreference: ReturnPreference.Representation
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("Prefer", "return=representation"), request.Headers);
    }

    #endregion
    
    #region PatchByQuery
    
    [Test]
    public async Task CreationFromACdrPatchByQueryRequestWithDefaultsShouldReturnAHsdpRequest()
    {
        var request = _hsdpRequestFactory.Create(new CdrPatchByQueryRequest(
            resourceType: "Observation",
            queryParameters: new List<QueryParameter>
            {
                new("key1", "value1"),
                new("key2", "value2")
            },
            operations: new List<Operation>
            {
                new("a", "b", "c", "d")
            }
        ), _token.Object);

        var content = await request.Content!.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.IsInstanceOf<HsdpRequest>(request);
            Assert.AreEqual(new Uri("https://endpoint.com/Observation"), request.Path);
            Assert.AreEqual(HttpMethod.Patch, request.Method);
            Assert.AreEqual(3, request.Headers.Count);
            Assert.Contains(KeyValuePair.Create("Authorization", "Bearer accessToken"), request.Headers);
            Assert.Contains(KeyValuePair.Create("Accept", FullFhirMediaType), request.Headers);
            Assert.Contains(KeyValuePair.Create("api-version", "1"), request.Headers);
            Assert.AreEqual(2, request.QueryParameters.Count);
            Assert.Contains(KeyValuePair.Create("key1", "value1"), request.QueryParameters);
            Assert.Contains(KeyValuePair.Create("key2", "value2"), request.QueryParameters);
            Assert.AreEqual(64, content.Length);
        });
    }

    [Test]
    public void CreationFromACdrPatchByQueryRequestWithConditionShouldReturnAHsdpRequestWithIfNoneExistsHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrPatchByQueryRequest(
            resourceType: "Observation",
            queryParameters: new List<QueryParameter>
            {
                new("key1", "value1"),
                new("key2", "value2")
            },
            operations: new List<Operation>
            {
                new("a", "b", "c", "d")
            },
            forVersion: "someVersion"
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("If-Match", "someVersion"), request.Headers);
    }

    [Test]
    public void CreationFromACdrPatchByQueryRequestWithPreferenceShouldReturnAHsdpRequestWithPreferHeader()
    {
        var request = _hsdpRequestFactory.Create(new CdrPatchByQueryRequest(
            resourceType: "Observation",
            queryParameters: new List<QueryParameter>
            {
                new("key1", "value1"),
                new("key2", "value2")
            },
            operations: new List<Operation>
            {
                new("a", "b", "c", "d")
            },
            returnPreference: ReturnPreference.Representation
        ), _token.Object);

        Assert.Contains(KeyValuePair.Create("Prefer", "return=representation"), request.Headers);
    }

    #endregion
    
}
