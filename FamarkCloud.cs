using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public class FamarkCloud
{
    private HttpClient _client;

    public string ErrorMessage { get; private set; }

    public FamarkCloud()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://www.famark.com/Host/api.svc/api/");
    }

    public async Task<string> PostData(string path, string data, string sessionId)
    {
        const string contentType = "application/json";

        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

        if (!string.IsNullOrEmpty(sessionId))
        {
            _client.DefaultRequestHeaders.Add("SessionId", sessionId);
        }

        StringContent requestData = new StringContent(data, Encoding.UTF8, contentType);
        HttpResponseMessage response = await _client.PostAsync(path, requestData);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        IEnumerable<string> headerValues;
        ErrorMessage = response.Headers.TryGetValues("errormessage", out headerValues) ? headerValues.FirstOrDefault() : response.ReasonPhrase;
        Console.Error.WriteLine(ErrorMessage);
        return null;
    }
}