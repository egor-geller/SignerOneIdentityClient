// using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SignerOneIdentityClient.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using NLog;

namespace SignerOneIdentityClient.Pages;
public class IndexModel : PageModel
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly IConfiguration _configuration;

    [BindProperty]
    public TokenResult? Token { get; set; }

    public IndexModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnGetCallbackAsync([FromQuery] string code)
    {
        string baseUrl = _configuration["IdentityProvider:BaseUrl"]!;
        string clientId = _configuration["IdentityProvider:ClientId"]!;
        string clientSecret = _configuration["IdentityProvider:ClientSecret"]!;
        string callbackUri = _configuration["IdentityProvider:CallabckUri"]!;

        HttpClient httpClient = new ();

        List<KeyValuePair<string, string>> values = new()
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("redirect_uri", callbackUri),
            new KeyValuePair<string, string>("code", code),
        };
        FormUrlEncodedContent content = new (values);

        string authenticationString = $"{clientId}:{clientSecret}";
        string base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

        HttpRequestMessage requestMessage = new (HttpMethod.Post, $"{baseUrl}/connect/token");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        requestMessage.Content = content;

        //make the request
        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            Token = await response.Content.ReadFromJsonAsync<TokenResult>();
        }

        return Page();
    }
}
