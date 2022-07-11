using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
namespace Quickbase_Interview_01
{
    public class InterviewTask
    {
        //requests a profile through a GET request to GitHub's api and returns a populated dictionary with the data
        static public async Task<Dictionary<string, string>> GetGitHubUser(string gitHubUsername)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Quickbase Interview App");
            var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", githubToken);
            string response = null;
            Dictionary<string, string> gitHubUser = new Dictionary<string, string>();
            try
            {
                response = await client.GetStringAsync("https://api.github.com/users/" + gitHubUsername);
                gitHubUser = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            }
			catch(Exception e)
			{
                System.Diagnostics.Debug.WriteLine("CAUGHT EXCEPTION:");
                System.Diagnostics.Debug.WriteLine(e);
                gitHubUser.Add("message", e.Message);
            }
            return gitHubUser;
        }
        //creates a contact in Freshdesk from a provided JSON string by submitting a POST request through their API
        static public async Task<System.Net.HttpStatusCode> SubmitFreshdeskContact(string subdomain, string contentJson)
        {
            var client = new HttpClient();
            var freshdeskToken = Environment.GetEnvironmentVariable("FRESHDESK_TOKEN");
            var byteArray = new UTF8Encoding().GetBytes(freshdeskToken + ":X");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", Convert.ToBase64String(byteArray));
            HttpResponseMessage freshdeskResponse = null;
            try
            {
                freshdeskResponse = await client.PostAsync("https://" + subdomain + ".freshdesk.com/api/v2/contacts", new StringContent(contentJson, Encoding.UTF8, "application/json"));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("CAUGHT EXCEPTION:");
                System.Diagnostics.Debug.WriteLine(e);
            }
            if (freshdeskResponse != null)
            {
                return freshdeskResponse.StatusCode;
            }
            return System.Net.HttpStatusCode.NotFound;
        }
        //Populates and returns a JSON string with the applicable fields from a GitHub user's profile
        static public string CreateFreshdeskContactJson(ref Dictionary<string, string> gitHubUser)
        {
            if(!gitHubUser.ContainsKey("login") || !gitHubUser.ContainsKey("id"))
			{
                return "Error! GitHub user information not as expected.";
			}
            Dictionary<string, string> freshdeskContact = new Dictionary<string, string>();
            freshdeskContact.Add("name", gitHubUser["login"]);
            freshdeskContact.Add("unique_external_id", gitHubUser["id"]);
            if(gitHubUser.ContainsKey("email"))
                freshdeskContact.Add("email", gitHubUser["email"]);
            if(gitHubUser.ContainsKey("twitter_username"))
                freshdeskContact.Add("twitter_id", gitHubUser["twitter_username"]);
            if(gitHubUser.ContainsKey("bio"))
                freshdeskContact.Add("description", gitHubUser["bio"]);
            return JsonConvert.SerializeObject(freshdeskContact);
        }
        static public async Task Main()
        {
            Console.WriteLine("Please enter GitHub login: ");
            string? userLogin = Console.ReadLine();
            if(String.IsNullOrEmpty(userLogin))
			{
                Console.WriteLine("Error! Invalid GitHub username input.");
                return;
			}
            Console.WriteLine("Please enter Freshdesk subdomain: ");
            string? freshdeskDomain = Console.ReadLine();
            if (String.IsNullOrEmpty(freshdeskDomain))
            {
                Console.WriteLine("Error! Invalid Freshdesk subdomain input.");
                return;
            }

            Dictionary<string, string> gitHubUser = await GetGitHubUser(userLogin);
            if (gitHubUser.ContainsKey("message"))
            {
                Console.WriteLine("Error while fetching GitHub user! " + gitHubUser["message"]);
                return;
			}

			string freshdeskContactJson = CreateFreshdeskContactJson(ref gitHubUser);
            if(freshdeskContactJson.Equals("Error! GitHub user information not as expected."))
			{
                Console.WriteLine(freshdeskContactJson);
                return;
			}

			var responseCode = await SubmitFreshdeskContact(freshdeskDomain, freshdeskContactJson);
            if(responseCode.Equals(System.Net.HttpStatusCode.Created))
			{
                Console.WriteLine("Contact succesfully added.");
            }
            else
			{
                Console.WriteLine("Error while creating Freshdesk contact! Status: " + responseCode.ToString());
			}
            Console.ReadKey();
        }
    }
}