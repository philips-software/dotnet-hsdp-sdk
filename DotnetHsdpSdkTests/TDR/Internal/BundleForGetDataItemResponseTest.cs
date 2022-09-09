using System.Text.Json;
using DotnetHsdpSdk.TDR.Internal;
using NUnit.Framework;

namespace DotnetHsdpSdkTests.TDR.Internal;

[TestFixture]
public class BundleForGetDataItemResponseTest
{
    [Test]
    public void Deserialize()
    {
        var json = @"{""type"":""searchset"",""total"":1,""entry"":[{""fullUrl"":""https://tdr-service-client-test.eu-west.philips-healthsuite.com/store/tdr/DataItem?organization=McsTenantOrg1&user=%7C85cff1d2-cf5d-430c-adcd-e838c7c1df20&_id=8c23f80b204c1bc691964074858f1a9e89697ffae500571106e16f80c8b6c4a2"",""resource"":{""deleteTimestamp"":""2022-12-08T05:46:19.602Z"",""id"":""8c23f80b204c1bc691964074858f1a9e89697ffae500571106e16f80c8b6c4a2"",""meta"":{""versionId"":""1"",""lastUpdated"":""2022-09-08T05:46:19.602Z""},""organization"":""McsTenantOrg1"",""dataType"":{""system"":""McsTenantN1"",""code"":""FHIR-1""},""timestamp"":""2022-09-08T05:46:18.823Z"",""user"":{""system"":"""",""value"":""85cff1d2-cf5d-430c-adcd-e838c7c1df20""},""device"":{""system"":"""",""value"":""5e4712b4-a2a9-11eb-ad08-5b373692516d""},""data"":{""resourceType"":""Bundle"",""type"":""transaction"",""entry"":[{""fullUrl"":""urn:uuid:b7f527d3-c430-492c-84af-935e8309ea95"",""resource"":{""resourceType"":""Device"",""status"":""active"",""identifier"":[{""system"":""http://hl7.org/fhir/sid/eui-48/bluetooth"",""value"":""2C-AB-33-21-BC-D8""}],""manufacturer"":""A&D Medical"",""model"":""UC-352BLE""},""request"":{""method"":""POST"",""url"":""Device"",""ifNoneExist"":""identifier=http://hl7.org/fhir/sid/eui-48/bluetooth|2C-AB-33-21-BC-D8""}},{""fullUrl"":""urn:uuid:3a486e7e-ecde-4659-b542-5a9451f6210f"",""resource"":{""resourceType"":""Observation"",""status"":""final"",""category"":[{""coding"":[{""system"":""http://terminology.hl7.org/CodeSystems/observation_category"",""code"":""vital-signs"",""display"":""Vital Signs""}],""text"":""Vital Signs""}],""code"":{""coding"":[{""system"":""http://loinc.org"",""code"":""3141-9"",""display"":""Body weight measured""}]},""valueQuantity"":{""value"":95.2,""unit"":""Kg"",""system"":""http://unitsofmeasure.org"",""code"":""kg""},""effectiveDateTime"":""2022-09-08T07:46:17+02:00"",""subject"":{""reference"":""Patient/5e4712b4-a2a9-11eb-ad08-5b373692516d""},""device"":{""reference"":""urn:uuid:b7f527d3-c430-492c-84af-935e8309ea95""}},""request"":{""method"":""POST"",""url"":""Observation""}}]},""creationTimestamp"":""2022-09-08T05:46:19.602Z"",""resourceType"":""DataItem""}}],""_startAt"":0,""link"":[{""relation"":""next"",""url"":""https://tdr-service-client-test.eu-west.philips-healthsuite.com/store/tdr/DataItem?organization=McsTenantOrg1&_count=1&_startAt=1""}],""resourceType"":""Bundle""}";
        var result = JsonSerializer.Deserialize<BundleForGetDataItemResponse>(json);
        Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.AreEqual(0, result._startAt);
                Assert.AreEqual(1, result.total);
                Assert.AreEqual("searchset", result.type);
                Assert.AreEqual("Bundle", result.resourceType);
                Assert.AreEqual(1, result.link.Count);
                Assert.AreEqual("next", result.link[0].relation);
                Assert.AreEqual("https://tdr-service-client-test.eu-west.philips-healthsuite.com/store/tdr/DataItem?organization=McsTenantOrg1&_count=1&_startAt=1", result.link[0].url);
                Assert.AreEqual(1, result.entry.Count);
                Assert.AreEqual("McsTenantOrg1", result.entry[0].resource.organization);
            }
        );
    }
}