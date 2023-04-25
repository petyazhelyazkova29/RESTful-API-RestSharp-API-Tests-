using RestSharp;
using System.Net;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace RestSharpAPITests
{
    public class RestSharpAPI_Tests
    {
        private RestClient client;
        private const string myBaseUrl = "https://petyscontactbook.petyazh29.repl.co";

        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(myBaseUrl);
        }

        [Test]
        public void Test_ListAllContacts_CheckFirstAndLastName()
        {
            var request = new RestRequest("/api/contacts", Method.Get);
            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var contact = JsonSerializer.Deserialize<List<Contacts>>(response.Content!);

            Assert.That(contact[0].firstName, Is.EqualTo("Steve"));
            Assert.That(contact[0].lastName, Is.EqualTo("Jobs"));
        }

        [Test]
        public void Test_FindContactsByKeyword_CheckFirstAndLastName()
        {
            var request = new RestRequest("/api/contacts/search/albert", Method.Get);
            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var contact = JsonSerializer.Deserialize<List<Contacts>>(response.Content!);

           Assert.That(contact[0].firstName, Is.EqualTo("Albert"));
           Assert.That(contact[0].lastName, Is.EqualTo("Einstein"));
        }

        [Test]
        public void Test_FindContactsByMissingKeyword()
        {
            var request = new RestRequest("/api/contacts/search/missing" + DateTime.Now.Ticks, Method.Get);
            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.EqualTo("[]"));
        }

        [Test]
        public void Test_CreateNewContact_InvalidData()
        {
            var request = new RestRequest("/api/contacts ", Method.Post);

            var contactBody = new
            {
                comments =  "Old friend"
            };
            request.AddBody(contactBody);   
            var response = client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"First name cannot be empty!\"}"));
        }

        [Test]
        public void Test_CreateNewContact_ValidData()
        {
            var request = new RestRequest("/api/contacts ", Method.Post);

            var contactBody = new
            {
                firstName = "Petya",
                lastName = "Zhelyazkova",
                email= "petya29@gmail.com",
                phone= "+1 800 200 301",
                comments="Old friend"
            };
            request.AddBody(contactBody);
            var response = client.Execute(request);
            var contactProperty = JsonSerializer.Deserialize<contactProperty>(response.Content!);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(contactProperty.msg, Is.EqualTo("Contact added."));
            Assert.That(contactProperty.contact.id, Is.GreaterThan(0));
            Assert.That(contactProperty.contact.firstName, Is.EqualTo(contactBody.firstName));
            Assert.That(contactProperty.contact.lastName, Is.EqualTo(contactBody.lastName));
            Assert.That(contactProperty.contact.email, Is.EqualTo(contactBody.email));
            Assert.That(contactProperty.contact.phone, Is.EqualTo(contactBody.phone));
            Assert.That(contactProperty.contact.dateCreated, Is.Not.Empty);
            Assert.That(contactProperty.contact.comments, Is.Not.Empty);
        }
    }
}