using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System.Text.Json;

namespace DnkCompanyMicroservice.IntegrationTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CompanyTypeTests : PlaywrightTest
{
    private IAPIRequestContext request = null!;


    [SetUp]
    public async Task Setup()
    {
        request = await this.Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = "https://localhost:7207",
            IgnoreHTTPSErrors = true
        });
    }

    [Test]
    public async Task IsAlive()
    {
        IAPIResponse response = await this.request.GetAsync("/types?language=de");

        await this.Expect(response).ToBeOKAsync();
    }

    [TestCase("Playwright Test Company Type")]
    public async Task ShouldCreateCompanyType(string newName)
    {
        string newCompanyTypeName = newName + Guid.NewGuid().ToString()[..5];

        Dictionary<string, string> data = new()
        {
            { "name", newCompanyTypeName },
            { "language", "de" }
        };

        IAPIResponse newCompanyTypeResponse = await this.request.PostAsync("/types", new() { DataObject = data });

        await this.Expect(newCompanyTypeResponse).ToBeOKAsync();

        IAPIResponse companyTypesResponse = await this.request.GetAsync("/types?language=de");

        await this.Expect(companyTypesResponse).ToBeOKAsync();

        JsonElement? companyTypesJsonResponse = await companyTypesResponse.JsonAsync();        

        JsonElement? newCompanyType = null;

        foreach (JsonElement issueObj in companyTypesJsonResponse?.EnumerateArray())
        {
            if (issueObj.TryGetProperty("translation", out var translation) && translation.GetString() == newCompanyTypeName)
            {
                newCompanyType = issueObj;
            }
        }

        Assert.That(newCompanyType, Is.Not.Null);
    }

    [TestCase("Playwright Test Company Type", "Updated Playwright Test Company Type")]
    public async Task ShouldUpdateCompanyType(string newName, string updatedName)
    {
        string newCompanyTypeName = newName + Guid.NewGuid().ToString()[..5];

        Dictionary<string, string> data = new()
        {
            { "name", newCompanyTypeName },
            { "language", "de" }
        };

        //Create a new company type
        IAPIResponse newCompanyTypeResponse = await this.request.PostAsync("/types", new() { DataObject = data });

        await this.Expect(newCompanyTypeResponse).ToBeOKAsync();

        JsonElement? newCompanyTypeJsonResponse = await newCompanyTypeResponse.JsonAsync();

        string newCompanyTypeGuid = newCompanyTypeJsonResponse.Value.GetString()!;

        //Update the company type

        string updatedCompanyTypeName = updatedName + Guid.NewGuid().ToString()[..5];

        Dictionary<string, string> updateData = new()
        {
            { "id", newCompanyTypeGuid },
            { "name", updatedCompanyTypeName },
            { "language", "de" }
        };

        IAPIResponse updateCompanyTypeResponse = await this.request.PutAsync("/types", new() { DataObject = updateData });

        await this.Expect(updateCompanyTypeResponse).ToBeOKAsync();
    }

    [TestCase("Playwright Test Company Type")]
    public async Task Language_Isfr_CreateShouldFailValidation(string newName)
    {
        string newCompanyTypeName = newName + Guid.NewGuid().ToString()[..5];

        Dictionary<string, string> data = new()
        {
            { "name", newCompanyTypeName },
            { "language", "fr" }
        };

        //Create a new company type
        IAPIResponse newCompanyTypeResponse = await this.request.PostAsync("/types", new() { DataObject = data });

        await this.Expect(newCompanyTypeResponse).Not.ToBeOKAsync();
        Assert.That(newCompanyTypeResponse.StatusText, Is.EqualTo("Unprocessable Entity"));
    }

    [Test]
    public async Task Name_IsEmpty_CreateShouldFailValidation()
    {
        Dictionary<string, string> data = new()
        {
            { "name", "" },
            { "language", "de" }
        };

        //Create a new company type
        IAPIResponse newCompanyTypeResponse = await this.request.PostAsync("/types", new() { DataObject = data });

        await this.Expect(newCompanyTypeResponse).Not.ToBeOKAsync();
        Assert.That(newCompanyTypeResponse.StatusText, Is.EqualTo("Unprocessable Entity"));
    }
}