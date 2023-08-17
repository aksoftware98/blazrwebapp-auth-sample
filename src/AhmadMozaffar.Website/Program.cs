using AhmadMozaffar.Website;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddServerComponents()
    .AddWebAssemblyComponents();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    
    // Handling SameSite cookie according to https://learn.microsoft.com/aspnet/core/security/samesite?view=aspnetcore-3.1
    options.HandleSameSiteCookieCompatibility();
});
builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.GetSection("AzureAdB2C").Bind(options);
        options.TokenValidationParameters.ValidIssuers = new[] { builder.Configuration["AzureAdB2C:ValidIssuer"] };

    })
    .EnableTokenAcquisitionToCallDownstreamApi(new[] { "User.Read" })
    .AddInMemoryTokenCaches();
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();
// Add the Microsoft Identity Web cookie policy
app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddServerRenderMode()
    .AddWebAssemblyRenderMode();

app.MapGet("/api/weather-forecast", async (IHttpContextAccessor httpContext) =>
{
    var request = httpContext.HttpContext.Request;
    var user = httpContext.HttpContext.User;
    await Task.Delay(1000);
    // Fetch the user details 
    
    return Results.Ok();
});

app.MapGet("/api/identity", async (IHttpContextAccessor httpContext) =>
{
	var request = httpContext.HttpContext.Request;
	var user = httpContext.HttpContext.User;
	await Task.Delay(1000);
	// Fetch the user details 
	
	return Results.Ok(user.Claims);
})
    .RequireAuthorization();

app.MapGet("/api/comments", async ([FromBody] CommentRequest comment, IHttpContextAccessor httpContext) =>
{
	var request = httpContext.HttpContext.Request;
	var user = httpContext.HttpContext.User;
    var name = user.GetDisplayName();

	await Task.Delay(1000);
    // Fetch the user details 

    var content = comment.Content; // Save and validate or other operations needed

	return Results.Ok(user.Claims);
})
	.RequireAuthorization();
app.Run();


public class CommentRequest
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}