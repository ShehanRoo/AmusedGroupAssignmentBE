using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System.Text.Json;
using AmusedGroupAssignment;

namespace PlaywrightTests;

[TestFixture]
public class TestRestAPI : PlaywrightTest
{

    private IAPIRequestContext Request = null;
    RootObject jsonTestData = null;

    [Test]
    public async Task GetSingleItem()
    {
        string endpoint = jsonTestData.Endpoint;
        string parameters = jsonTestData.Scenarios.SingleItem.Parameters;
        var request = await GETAPIRequest(endpoint, parameters);
        Assert.True(request.Ok);
        var rsp = await request.JsonAsync();
        Assert.That(rsp.Value.GetProperty("id").ToString(), Is.EqualTo("7"));

    }

    [Test]
    public async Task GetMultipleItems()
    {
        string endpoint = jsonTestData.Endpoint;
        string parameters = jsonTestData.Scenarios.MultipleItems.Parameters;
        var request = await GETAPIRequest(endpoint, parameters);
        Assert.True(request.Ok);
        var rsp = await request.JsonAsync();
        Assert.That(rsp.Value[0].GetProperty("id").ToString(), Is.EqualTo("3"));
        Assert.That(rsp.Value[1].GetProperty("id").ToString(), Is.EqualTo("5"));
        Assert.That(rsp.Value[2].GetProperty("id").ToString(), Is.EqualTo("10"));
    }

    [Test]
    public async Task AddItem()
    {
        Dictionary<string, object> bodyData = jsonTestData.Scenarios.AddItem.Body;

        string endpoint = jsonTestData.Endpoint;
        var response = await POSTAPIRequest(endpoint, bodyData);
        Assert.True(response.Ok);
        var rsp = await response.JsonAsync();
        Assert.That(rsp.Value.GetProperty("name").ToString(), Is.EqualTo("Test Product"));
    }

    [Test]
    public async Task UpdateItem()
    {
        Dictionary<string, object> newItemBody = jsonTestData.Scenarios.AddItem.Body;
        string endpoint = jsonTestData.Endpoint;
        var newItem = await POSTAPIRequest(endpoint, newItemBody);
        Assert.True(newItem.Ok);
        var itemResponse = await newItem.JsonAsync();

        Dictionary<string, object> bodyData = jsonTestData.Scenarios.UpdateItem.Body;

        var itemId = itemResponse.Value.GetProperty("id").ToString();
        var response = await PUTAPIRequest(endpoint, "/" + itemId, bodyData);
        Assert.True(response.Ok);
        var rsp = await response.JsonAsync();
        rsp.Value.TryGetProperty("data", out JsonElement dataObject);
        Assert.That(dataObject.GetProperty("Color").ToString(), Is.EqualTo("Cyan"));
    }

    [Test]
    public async Task DeleteItem()
    {
        Dictionary<string, object> newItemBody = jsonTestData.Scenarios.AddItem.Body;

        string endpoint = jsonTestData.Endpoint;
        var newItem = await POSTAPIRequest(endpoint, newItemBody);
        Assert.True(newItem.Ok);
        var itemResponse = await newItem.JsonAsync();
        var itemId = itemResponse.Value.GetProperty("id").ToString();
        var response = await DELETEAPIRequest(endpoint, "/" + itemId);
        Assert.True(response.Ok);
        var rsp = await response.JsonAsync();
        Assert.That(rsp.Value.GetProperty("message").ToString(), Is.EqualTo("Object with id = " + itemId + " has been deleted."));

    }

    [SetUp]
    public async Task SetUpAPITesting()
    {
        await CreateAPIRequestContext();
        DotNetEnv.Env.TraversePath().Load();
        string jsonData = File.ReadAllText("../../../testdata.json");
        this.jsonTestData = JsonConvert.DeserializeObject<RootObject>(jsonData);
    }

    [TearDown]
    public async Task TearDownAPITesting()
    {
        await Request.DisposeAsync();
    }

    private async Task CreateAPIRequestContext()
    {
        Request = await Playwright.APIRequest.NewContextAsync();
    }

    public async Task<IAPIResponse> GETAPIRequest(string endpoint, string parameters)
    {
        var base_url = Environment.GetEnvironmentVariable("BASE_URL");
        var response = await Request.GetAsync(base_url + endpoint + parameters);
        return response;
    }

    public async Task<IAPIResponse> POSTAPIRequest(string endpoint, object bodyData)
    {
        var base_url = Environment.GetEnvironmentVariable("BASE_URL");
        string body = JsonConvert.SerializeObject(bodyData);
        var response = await Request.PostAsync(base_url + endpoint, new() { DataObject = body });
        return response;
    }

    public async Task<IAPIResponse> PUTAPIRequest(string endpoint, string itemId, object bodyData)
    {
        var base_url = Environment.GetEnvironmentVariable("BASE_URL");
        string body = JsonConvert.SerializeObject(bodyData);
        var response = await Request.PutAsync(base_url + endpoint + itemId, new() { DataObject = body });
        return response;
    }

    public async Task<IAPIResponse> DELETEAPIRequest(string endpoint, string parameters)
    {
        var base_url = Environment.GetEnvironmentVariable("BASE_URL");
        var response = await Request.DeleteAsync(base_url + endpoint + parameters);
        return response;
    }
}