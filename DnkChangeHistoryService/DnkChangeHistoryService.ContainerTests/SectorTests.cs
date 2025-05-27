using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.Json;

namespace DnkCompanyMicroservice.ContainerTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SectorTests : PlaywrightTest
{
    private IAPIRequestContext request = null!;

    [SetUp]
    public async Task Setup()
    {
        ushort mappedPort = ContainerSetUpFixture.MappedPort;

        this.request = await this.Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = $"http://localhost:{mappedPort}",
            IgnoreHTTPSErrors = true
        });
    }    

    [Test]
    public async Task IsAlive()
    {
        IAPIResponse response = await this.request.GetAsync("/healthcheck");

        await this.Expect(response).ToBeOKAsync();
    }

    [Test]
    public async Task ShouldCreateBusinessSector()
    {
        Dictionary<string, string> data = new()
        {
            { "name", "Test Business Sector" },
            { "language", "de" }
        };

        IAPIResponse newBusinessSectorResponse = await this.request.PostAsync("/sectors", new() { DataObject = data });

        await this.Expect(newBusinessSectorResponse).ToBeOKAsync();
    }

    [Test]
    public async Task InvalidLanguage_ShouldFail()
    {
        Dictionary<string, string> data = new()
        {
            { "name", "Test Business Sector" },
            { "language", "fr" }
        };

        IAPIResponse newBusinessSectorResponse = await this.request.PostAsync("/sectors", new() { DataObject = data });

        await this.Expect(newBusinessSectorResponse).Not.ToBeOKAsync();
    }

    [Test]
    public async Task ShouldCreateBusinessBranch()
    {
        Dictionary<string, string> sectorData = new()
        {
            { "name", "Test Business Sector" },
            { "language", "de" }
        };

        IAPIResponse newBusinessSectorResponse = await this.request.PostAsync("/sectors", new() { DataObject = sectorData });

        await this.Expect(newBusinessSectorResponse).ToBeOKAsync();

        JsonElement? json = await newBusinessSectorResponse.JsonAsync();

        string? businessSectorId = json?.GetString();

        if(string.IsNullOrEmpty(businessSectorId))
        {
            Assert.Fail("Business Sector Id is null or empty");
        }

        Dictionary<string, string> branchData = new()
        {
            { "name", "Test Business Branch" },
            { "language", "de" },
            { "businesssectorid", businessSectorId! }
        };

        IAPIResponse newBusinessBranchResponse = await this.request.PostAsync("/sectors/branches", new() { DataObject = branchData });

        await this.Expect(newBusinessBranchResponse).ToBeOKAsync();
    }
}
