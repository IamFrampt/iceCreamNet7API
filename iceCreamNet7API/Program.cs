using Newtonsoft.Json;

string url = "https://icecreamsapi.azurewebsites.net";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/GetData", async (HttpClient _httpClient) =>
{
    var response = await _httpClient.GetAsync(url + "/allicecreams");

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine("Error");
        return Results.NotFound("Url not found.");
    }

    var data = await response.Content.ReadAsStringAsync();
    var allIceCreams = JsonConvert.DeserializeObject<List<IceCreamData>>(data);
    return Results.Ok(allIceCreams);
});

app.MapGet("/GetByCompanyName", async (HttpClient _httpClient,string CompanyName) =>
{
    var response = await _httpClient.GetAsync(url + "/allicecreams");

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine("Error");
        return Results.NotFound("Url not found.");
    }

    var data = await response.Content.ReadAsStringAsync();
    var allIceCreams = JsonConvert.DeserializeObject<List<IceCreamData>>(data).FindAll(x=>x.Company.Contains(CompanyName,StringComparison.OrdinalIgnoreCase));
    return Results.Ok(allIceCreams);
});


app.MapGet("/GetByIDData", async (HttpClient _httpClient, int id) =>
{
    var response = await _httpClient.GetAsync(url + "/allicecreams/icecream/" + id);

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine("Error");
        return Results.NotFound("No Ice cream with that id.");
    }

    var data = await response.Content.ReadAsStringAsync();
    var iceCreamByID = JsonConvert.DeserializeObject<IceCreamData>(data);
    return Results.Ok(iceCreamByID);
});


app.MapPost("/AddData", async (HttpClient httpClient, AddIceCreamData iceCream) =>
{
    
    var formContent = new FormUrlEncodedContent(new[]
    {
    new KeyValuePair<string, string>("Name", iceCream.Name),
    new KeyValuePair<string, string>("Info", iceCream.Info),
    new KeyValuePair<string, string>("Type", iceCream.Type),
    new KeyValuePair<string, string>("Company", iceCream.Company),
    new KeyValuePair<string, string>("Calories", iceCream.nutritionalContent.Energy.Calorie),
    new KeyValuePair<string, string>("KiloJoules", iceCream.nutritionalContent.Energy.Kilojoules),
    new KeyValuePair<string, string>("Fat", iceCream.nutritionalContent.Fat),
    new KeyValuePair<string, string>("Salt", iceCream.nutritionalContent.Salt),
    new KeyValuePair<string, string>("Carbohydrates", iceCream.nutritionalContent.Carbohydrates),
    new KeyValuePair<string, string>("Protein", iceCream.nutritionalContent.Protein)
});

    var httpResponse = await httpClient.PostAsync(url + "/allicecreams/add", formContent);

    if (httpResponse.IsSuccessStatusCode)
    {
        return Results.Ok(httpResponse);
    }
    else
    {
        return Results.BadRequest();
    }
});


app.MapDelete("/DeleteData", async (HttpClient _httpClient, int id) =>
{
    var iceCreamToDelete = await _httpClient.GetAsync(url + "/allIceCreams/icecream/" + id);

    if (!iceCreamToDelete.IsSuccessStatusCode)
    {
        Console.WriteLine("Error");
        return Results.NotFound("No Ice cream with that id.");
    }

    var data = await iceCreamToDelete.Content.ReadAsStringAsync();
    var iceCreamByID = JsonConvert.DeserializeObject<IceCreamData>(data);
    var deletedIceCream = await _httpClient.DeleteAsync(url + "/allicecreams/delete/" + id);

    return Results.Ok(iceCreamByID.Name+ " was deleted.");
});

app.Run();

public class IceCreamData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Info { get; set; }
    public string Img { get; set; }
    public string Type { get; set; }
    public string Company { get; set; }
    public nutritionalContent nutritionalContent { get; set; }
}

public class AddIceCreamData
{
    public string Name { get; set; }
    public string Info { get; set; }
    public string Img { get; set; }
    public string Type { get; set; }
    public string Company { get; set; }
    public nutritionalContent nutritionalContent { get; set; }
}

public class nutritionalContent
{
    public Energy Energy { get; set; }
    public string Fat { get; set; }
    public string Salt { get; set; }
    public string Carbohydrates { get; set; }
    public string Protein { get; set; }
}

public class Energy
{
    public string Calorie { get; set; }
    public string Kilojoules { get; set; }
}