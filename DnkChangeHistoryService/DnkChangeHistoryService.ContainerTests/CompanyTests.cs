using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.Json;

namespace DnkCompanyMicroservice.ContainerTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CompanyTests : PlaywrightTest
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
    public async Task ShouldGetAllCompanies()
    {
        IFormData multipart = this.request.CreateFormData();

        multipart.Append("businessBranchId", "50AAC8D5-D296-4E9B-A508-098442A3A479");
        multipart.Append("companyHeadcountId", "6FC8B4B1-705B-42CA-9542-A544904D0D67");
        multipart.Append("companyTypeId", "90708939-C192-4B18-AF73-F55370A10553");
        multipart.Append("countryId", "C8ACC094-FB61-459B-9BA5-0839F345A465");
        multipart.Append("name", "Ztest");
        multipart.Append("websiteUrl", "www.test.de");
        multipart.Append("stockExchangeListed", false);

        IAPIResponse response = await this.request.PostAsync("/company", new() { Multipart = multipart });

        await this.Expect(response).ToBeOKAsync();

        IFormData multipart2 = this.request.CreateFormData();

        multipart2.Append("businessBranchId", "50AAC8D5-D296-4E9B-A508-098442A3A479");
        multipart2.Append("companyHeadcountId", "6FC8B4B1-705B-42CA-9542-A544904D0D67");
        multipart2.Append("companyTypeId", "90708939-C192-4B18-AF73-F55370A10553");
        multipart2.Append("countryId", "C8ACC094-FB61-459B-9BA5-0839F345A465");
        multipart2.Append("name", "ABCtest");
        multipart2.Append("websiteUrl", "www.test.de");
        multipart2.Append("stockExchangeListed", false);

        IAPIResponse response2 = await this.request.PostAsync("/company", new() { Multipart = multipart2 });

        await this.Expect(response2).ToBeOKAsync();

        IAPIResponse responseGet = await this.request.GetAsync("/companies?language=de");

        await this.Expect(responseGet).ToBeOKAsync();

        JsonElement? json = await responseGet.JsonAsync();

        JsonElement? companies = json?.GetProperty("companies");

        JsonElement? totalCount = json?.GetProperty("totalCount");

        JsonElement? firstCompany = companies?.EnumerateArray().First();

        Assert.Multiple(() =>
        {
            Assert.That(companies?.EnumerateArray().Count() == 2);
            Assert.That(totalCount?.GetInt32() == 2);
            Assert.That(firstCompany?.GetProperty("id").GetString(), Is.Not.WhiteSpace);
            Assert.That(firstCompany?.GetProperty("name").GetString(), Is.EqualTo("ABCtest"));
            Assert.That(firstCompany?.GetProperty("companyType").GetString(), Is.Not.WhiteSpace);
            Assert.That(firstCompany?.GetProperty("businessBranch").GetString(), Is.Not.WhiteSpace);
            Assert.That(firstCompany?.GetProperty("headCount").GetString(), Is.Not.WhiteSpace);
            Assert.That(firstCompany?.GetProperty("isListedOnStockExchange").GetBoolean(), Is.False);
        });

        IAPIResponse responseGetFiltered = await this.request.GetAsync("/companies?language=de&searchTerm=abc");

        await this.Expect(responseGetFiltered).ToBeOKAsync();

        JsonElement? jsonFiltered = await responseGetFiltered.JsonAsync();

        Assert.Multiple(() =>
        {
            Assert.That(jsonFiltered, Is.Not.Null);
            Assert.That(jsonFiltered?.GetProperty("companies").EnumerateArray().Count(), Is.EqualTo(1));
        });

        IAPIResponse responseGetPaged = await this.request.GetAsync("/companies?language=de&page=10&pageSize=100");

        await this.Expect(responseGetPaged).ToBeOKAsync();

        JsonElement? jsonPaged = await responseGetPaged.JsonAsync();

        Assert.Multiple(() =>
        {
            Assert.That(jsonPaged, Is.Not.Null);
            Assert.That(jsonPaged?.GetProperty("totalCount").GetInt32(), Is.EqualTo(2));
            Assert.That(jsonPaged?.GetProperty("companies").EnumerateArray().Count(), Is.EqualTo(0));
        });
    }    
}

