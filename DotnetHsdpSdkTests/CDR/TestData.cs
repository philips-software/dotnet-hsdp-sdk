using Hl7.Fhir.Model;

namespace DotnetHsdpSdkTests.CDR;

public static class TestData
{
    public static readonly Observation Observation = new()
    {
        Status = ObservationStatus.Final,
        Category = new List<CodeableConcept>
        {
            new()
            {
                Coding = new List<Coding>
                {
                    new()
                    {
                        System = "http://hl7.org/fhir/observation-category",
                        Code = "vital-signs",
                        Display = "Vital Signs"
                    }
                }
            }
        },
        Code = new CodeableConcept
        {
            Coding = new List<Coding>
            {
                new()
                {
                    System = "http://hl7.org/fhir/observation-category",
                    Code = "vital-signs",
                    Display = "Vital Signs",
                }
            }
        },
        Subject = new ResourceReference
        {
            Reference = "Patient/6cc7127e-4c03-11ea-bfde-93a1b2effe45"
        },
        Effective = new FhirDateTime(2022, 9, 13, 12, 13, 14, TimeSpan.Zero),
        Issued = new DateTimeOffset(2022, 9, 13, 12, 13, 14, TimeSpan.Zero),
        Performer = new List<ResourceReference>
        {
            new()
            {
                Reference = "Patient/93411b9c-95dd-11ea-a0f0-13f9eff55841"
            }
        },
        Component = new List<Observation.ComponentComponent>
        {
            new()
            {
                Code = new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new()
                        {
                            System = "http://loinc.org",
                            Code = "8310-5",
                            Display = "Body temperature"
                        }
                    }
                },
                Value = new Quantity
                {
                    Value = new decimal(35.8),
                    Unit = "C",
                    System = "http://unitsofmeasure.org",
                    Code = "Cel"
                }
            }
        }
    };

    public static readonly Bundle TransactionBundle = new()
    {
        Type = Bundle.BundleType.Transaction,
        Entry = new List<Bundle.EntryComponent>
        {
            new()
            {
                FullUrl = Guid.NewGuid().ToString(),
                Request = new Bundle.RequestComponent
                {
                    Method = Bundle.HTTPVerb.POST,
                    Url = "Observation"
                },
                Resource = Observation
            }
        }
    };

    public static readonly OperationOutcome TokenExpiredOperationOutcome = new()
    {
        Issue = new List<OperationOutcome.IssueComponent>
        {
            new()
            {
                Severity = OperationOutcome.IssueSeverity.Error,
                Code = OperationOutcome.IssueType.Security,
                Details = new CodeableConcept
                {
                    Text = "Token expired"
                }
            }
        }
    };
}
