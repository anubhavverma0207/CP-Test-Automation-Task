
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TodoList.Api.Models;
using Xunit;

namespace TodoList.Api.ApiTests
{

    public class ApiTestsClass
    {
        private readonly HttpClient client;

        public ApiTestsClass()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:3002/");


        }



        [Fact]
        public async Task TestGetToDoListAPI()
        {
            // Send a GET request to the "ToDoList" endpoint of the API
            HttpResponseMessage response = await client.GetAsync("api/todoItems");

            // Ensure that the response status code is successful (200 OK)
            Assert.True(response.IsSuccessStatusCode);

            string content = await response.Content.ReadAsStringAsync();
            List<TodoItemDto> ResponseToDo = JsonConvert.DeserializeObject<List<TodoItemDto>>(content);

            Assert.NotNull(ResponseToDo[0].Id);
            Assert.True(ResponseToDo[0].Description == "test");
            Assert.True(!ResponseToDo[0].IsCompleted);

        }



        [Fact]
        public async Task TestPostToDoListAPI()
        {

            // Build the request body as a string
            var requestBody = "{\"description\": \"test\"}";

            // Create a StringContent object to represent the request body
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            // Send the post request
            var response = await client.PostAsync("api/todoItems", content);

            // Get the response body as a string
            string responseBody = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.True(responseBody.Equals("A todo item with description already exists"));
        }



        [Fact]
        public async Task TestGetToDoListApiWithID()
        {
            // Send a GET request to the "ToDoList" endpoint of the API
            var uuid = Guid.NewGuid();
            var url = "api/todoItems/" + uuid;
            HttpResponseMessage response = await client.GetAsync(url);

            // Ensure that the response status code is NotFound (404 )
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            string content = await response.Content.ReadAsStringAsync();

            var ExpectedOutput = "Todo item with id " + uuid + " not found";
            //Assert the response content matches to the swagger output
            Assert.True(content.Equals(ExpectedOutput));

        }

    }
}
