using Quickbase_Interview_01;
using Newtonsoft.Json;
namespace Unit_Tests
{
	[TestClass]
	public class GetGitHubUserTests
	{
		[TestMethod]
		public async Task defunktTest()
		{
			string login = "defunkt";
			Dictionary<string, string> result = await InterviewTask.GetGitHubUser(login);
			string expectedName = "Chris Wanstrath";
			string expectedBlog = "http://chriswanstrath.com/";
			string expectedId = "2";
			string expectedEmail = null;
			Assert.AreEqual(expectedName, result["name"]);
			Assert.AreEqual(expectedBlog, result["blog"]);
			Assert.AreEqual(expectedId, result["id"]);
			Assert.AreEqual(expectedEmail, result["email"]);
		}
		[TestMethod]
		public async Task IvanShishevTest()
		{
			string login = "Ivan-Shishev";
			Dictionary<string, string> result = await InterviewTask.GetGitHubUser(login);
			string expectedName = "Ivan Shishev";
			string expectedBio = "Test Bio";
			string expectedId = "100229870";
			string expectedEmail = null;
			Assert.AreEqual(expectedName, result["name"]);
			Assert.AreEqual(expectedBio, result["bio"]);
			Assert.AreEqual(expectedId, result["id"]);
			Assert.AreEqual(expectedEmail, result["email"]);
		}
		[TestMethod]
		public async Task IvanShishev2Test()
		{
			string login = "Ivan-Shishev2";
			Dictionary<string, string> result = await InterviewTask.GetGitHubUser(login);
			string expectedMesage = "Response status code does not indicate success: 404 (Not Found).";
			Assert.AreEqual(expectedMesage, result["message"]);
		}
	}

	[TestClass]
	public class CreateFreshdeskContactJsonTests
	{
		[TestMethod]
		public void FullDictionaryTest()
		{
			Dictionary<string, string> expectedDictionary = new Dictionary<string, string>();
			expectedDictionary.Add("name", "Ivan-Shishev");
			expectedDictionary.Add("unique_external_id", "100229870");
			expectedDictionary.Add("email", null);
			expectedDictionary.Add("twitter_id", null);
			expectedDictionary.Add("description", "Test Bio");
			string expectedJson = JsonConvert.SerializeObject(expectedDictionary);
			Dictionary<string, string> inputDictionary = new Dictionary<string, string>();
			inputDictionary.Add("login", "Ivan-Shishev");
			inputDictionary.Add("email", null);
			inputDictionary.Add("twitter_username", null);
			inputDictionary.Add("id", "100229870");
			inputDictionary.Add("bio", "Test Bio");
			string output = InterviewTask.CreateFreshdeskContactJson(ref inputDictionary);
			Assert.AreEqual(expectedJson, output);
		}
		[TestMethod]
		public void PartialDictionaryTest()
		{
			Dictionary<string, string> expectedDictionary = new Dictionary<string, string>();
			expectedDictionary.Add("name", "Ivan-Shishev");
			expectedDictionary.Add("unique_external_id", "100229870");
			string expectedJson = JsonConvert.SerializeObject(expectedDictionary);
			Dictionary<string, string> inputDictionary = new Dictionary<string, string>();
			inputDictionary.Add("login", "Ivan-Shishev");
			inputDictionary.Add("id", "100229870");
			string output = InterviewTask.CreateFreshdeskContactJson(ref inputDictionary);
			Assert.AreEqual(expectedJson, output);
		}
		[TestMethod]
		public void MissingIdTest()
		{
			string expectedJson = "Error! GitHub user information not as expected.";
			Dictionary<string, string> inputDictionary = new Dictionary<string, string>();
			inputDictionary.Add("login", "Ivan-Shishev");
			inputDictionary.Add("email", null);
			inputDictionary.Add("twitter_username", null);
			inputDictionary.Add("bio", "Test Bio");
			string output = InterviewTask.CreateFreshdeskContactJson(ref inputDictionary);
			Assert.AreEqual(expectedJson, output);
		}
		[TestMethod]
		public void MissingLoginTest()
		{
			string expectedJson = "Error! GitHub user information not as expected.";
			Dictionary<string, string> inputDictionary = new Dictionary<string, string>();
			inputDictionary.Add("id", "100229870");
			inputDictionary.Add("email", null);
			inputDictionary.Add("twitter_username", null);
			inputDictionary.Add("bio", "Test Bio");
			string output = InterviewTask.CreateFreshdeskContactJson(ref inputDictionary);
			Assert.AreEqual(expectedJson, output);
		}
	}

	[TestClass]
	public class SubmitFreshdeskContactTests
	{
		[TestMethod]
		public async Task SuccessfulCreationTest()
		{
			var expectedResponseCode = System.Net.HttpStatusCode.Created;
			string inputSubdomain = "jrvps-help";
			Dictionary<string, string> inputDictionary = new Dictionary<string, string>();
			inputDictionary.Add("name", "Ivan-Shishev");
			inputDictionary.Add("unique_external_id", new Random().Next().ToString());
			inputDictionary.Add("email", null);
			inputDictionary.Add("twitter_id", null);
			inputDictionary.Add("description", "Test Bio");
			string inputJson = JsonConvert.SerializeObject(inputDictionary);
			var returnedResponseCode = await InterviewTask.SubmitFreshdeskContact(inputSubdomain, inputJson);
			Assert.AreEqual(expectedResponseCode, returnedResponseCode);
		}

		[TestMethod]
		public async Task DuplicateCreationTest()
		{
			var expectedResponseCode = System.Net.HttpStatusCode.Conflict;
			string inputSubdomain = "jrvps-help";
			Dictionary<string, string> inputDictionary = new Dictionary<string, string>();
			inputDictionary.Add("name", "Ivan-Shishev");
			inputDictionary.Add("unique_external_id", "100229870");
			inputDictionary.Add("email", null);
			inputDictionary.Add("twitter_id", null);
			inputDictionary.Add("description", "Test Bio");
			string inputJson = JsonConvert.SerializeObject(inputDictionary);
			var returnedResponseCode = await InterviewTask.SubmitFreshdeskContact(inputSubdomain, inputJson);
			Assert.AreEqual(expectedResponseCode, returnedResponseCode);
		}
	}
}