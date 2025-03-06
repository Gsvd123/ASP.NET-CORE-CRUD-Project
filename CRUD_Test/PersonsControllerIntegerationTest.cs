using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD_Test
{
    public class PersonsControllerIntegerationTest : IClassFixture<CustomWebApplicatonFactory>
    {
        private readonly HttpClient _httpClient;    

        public PersonsControllerIntegerationTest(CustomWebApplicatonFactory Factory)
        {
            _httpClient = Factory.CreateClient();
        }

        #region Index
        [Fact]
        public async Task Index_ToReturnIndex()
        {
            //Arrange

            //Act

            HttpResponseMessage httpResponse = await _httpClient.GetAsync("?Persons/Index");


            //Assert

            httpResponse.Should().BeSuccessful();

            string responsebody=await  httpResponse.Content.ReadAsStringAsync();

            HtmlDocument htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(responsebody);

            var document = htmlDocument.DocumentNode;

            document.QuerySelectorAll("table.Persons").Should().NotBeNull();    
        }
        #endregion
    }
}
