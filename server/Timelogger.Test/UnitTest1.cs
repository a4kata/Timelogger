using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Timelogger.Api.Module;
using Timelogger.Entities;
using Xunit;

namespace Timelogger.Test
{
    public class Tests : IClassFixture<ApiContext>
    {
        private static HttpClient client;
        //private List<Project> projects = new List<Project>();
        private ApiContext ac;

        public Tests()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44358")
            };
            ac = GetTestDbContext();
            //var appSettings = ConfigurationManager.AppSettings["ProjectsDataFilePath"];
            var json = File.ReadAllText($"../../../../Timelogger/Data/Projects.json");
            var projects = JsonSerializer.Deserialize<List<Project>>(json);
            projects.ForEach(p => ac.Projects.Add(p));
            ac.SaveChanges();
        }

        [Test]
        public async Task TestGetProjectById_Equal()
        {
            int ID = 1;
            var responseText = await GetHttpResponseAsText($"/api/Projects/GetProjectById?projectId={ID}", Methods.Get);
            Project exp = JsonSerializer.Deserialize<Project>(responseText);
            AreProjectsEqual(exp, ac.GetProjectByID(ID));
        }

        [Test]
        public async Task TestGetProjectById_DoesNotExist()
        {
            int ID = 4;
            var responseText = await GetHttpResponseAsText($"/api/Projects/GetProjectById?projectId={ID}", Methods.Get);
            Assert.AreEqual(responseText, String.Empty);
        }

        [Test]
        public async Task TestGetProjectById_NotEqual()
        {
            int ID = 1;
            var responseText = await GetHttpResponseAsText($"/api/Projects/GetProjectById?projectId={ID}", Methods.Get);
            Project exp = JsonSerializer.Deserialize<Project>(responseText);
            Project actual = ac.GetProjectByID(2);
            Assert.AreNotEqual(exp.ClientName, actual.ClientName);
            Assert.AreNotEqual(exp.Deadline, actual.Deadline);
            Assert.AreNotEqual(exp.HourRate, actual.HourRate);
            Assert.AreNotEqual(exp.Name, actual.Name);
        }

        [Test]
        public async Task TestGetProjects_OrderByDeadline()
        {
            var responseText = await GetHttpResponseAsText($"/api/Projects/GetProjects_OrderByDeadline", Methods.Get);
            var exp = JsonSerializer.Deserialize<List<Project>>(responseText);
            List<Project> actual = ac.GetProjectOrderedByDeadline();
            for (int i = 0; i < actual.Count; i++)
            {
                AreProjectsEqual(exp[i], actual[i]);
            }
        }

        [Test]
        public async Task TestGetAllProject()
        {
            var responseText = await GetHttpResponseAsText($"/api/Projects/GetAllProjects", Methods.Get);
            var exp = JsonSerializer.Deserialize<List<Project>>(responseText);
            for (int i = 0; i < exp.Count; i++)
            {
                AreProjectsEqual(exp[i], ac.Projects.Where(p => p.Id == exp[i].Id).FirstOrDefault());
            }
        }

        [Test]
        public async Task TestRegisterTime()
        {
            var rtr = new RegisterTimeRequest { ProjectID = 1, Minutes = 234, Notes = "dfgdfhcfhncfhjcgfhj" };
            var content = new StringContent(JsonSerializer.Serialize(rtr), Encoding.UTF8, "application/json");
            var expResp = await GetHttpResponseAsText($"/api/Projects/RegisterTime", Methods.Post, content);
            Assert.AreEqual(expResp, ac.RegisterTime(rtr.ProjectID, rtr.Minutes, rtr.Notes));
        }

        [Test]
        public async Task TestRegisterTime_Bellow30()
        {
            var rtr = new RegisterTimeRequest { ProjectID = 1, Minutes = 23, Notes = "dfgdfhcfhncfhjcgfhj" };
            var content = new StringContent(JsonSerializer.Serialize(rtr), Encoding.UTF8, "application/json");
            var expResp = await GetHttpResponseAsText($"/api/Projects/RegisterTime", Methods.Post, content);
            Assert.IsTrue(expResp.Contains("The field Minutes must be between 30 and 2147483647."));
        }

        [Test]
        public async Task TestRegisterTime_ProjectDoesNotExisting()
        {
            var rtr = new RegisterTimeRequest { ProjectID = 13, Minutes = 34, Notes = "dfgdfhcfhncfhjcgfhj" };
            var content = new StringContent(JsonSerializer.Serialize(rtr), Encoding.UTF8, "application/json");
            var expResp = await GetHttpResponseAsText($"/api/Projects/RegisterTime", Methods.Post, content);
            Assert.AreEqual(expResp, ac.RegisterTime(rtr.ProjectID, rtr.Minutes, rtr.Notes));
        }

        [Test]
        public async Task TestCompleteProject()
        {
            int projectId = 1;
            var expResp = await GetHttpResponseAsText($"/api/Projects/CompleteProject?projectId={projectId}", Methods.Post);
            Assert.AreEqual(expResp, ac.CompleteProject(projectId));
        }

        [Test]
        public async Task TestRegisterTime_ProjectCompleted()
        {
            var rtr = new RegisterTimeRequest { ProjectID = 1, Minutes = 234, Notes = "dfgdfhcfhncfhjcgfhj" };
            var content = new StringContent(JsonSerializer.Serialize(rtr), Encoding.UTF8, "application/json");
            await GetHttpResponseAsText($"/api/Projects/CompleteProject?projectId={rtr.ProjectID}", Methods.Post);
            var expResp = await GetHttpResponseAsText($"/api/Projects/RegisterTime", Methods.Post, content);
            ac.CompleteProject(rtr.ProjectID);
            Assert.AreEqual(expResp, ac.RegisterTime(rtr.ProjectID, rtr.Minutes, rtr.Notes));
        }

        public static ApiContext GetTestDbContext()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
            .UseInMemoryDatabase(databaseName: "e-conomic interview")
            .Options;
            return new ApiContext(options);
        }

        private static void AreProjectsEqual(Project exp, Project actual)
        {
            Assert.AreEqual(exp.ClientName, actual.ClientName);
            Assert.AreEqual(exp.Cost, actual.Cost);
            Assert.AreEqual(exp.Deadline, actual.Deadline);
            Assert.AreEqual(exp.HourRate, actual.HourRate);
            //Assert.AreEqual(exp.IsCompleted, actual.IsCompleted);
            Assert.AreEqual(exp.Logs, actual.Logs);
            Assert.AreEqual(exp.Minutes, actual.Minutes);
            Assert.AreEqual(exp.Name, actual.Name);
            Assert.AreEqual(exp.TotalTime, actual.TotalTime);
        }

        private static async Task<string> GetHttpResponseAsText(string requestUri, Methods method, StringContent content = null)
        {
            HttpResponseMessage response = null;
            switch (method)
            {
                case Methods.Post:
                    response = await client.PostAsync(requestUri, content);
                    break;
                case Methods.Get:
                    response = await client.GetAsync(requestUri);
                    break;
                default:
                    break;
            }
            //Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            string responseText = await response.Content.ReadAsStringAsync();
            return responseText;
        }
    }

    public enum Methods
    {
        Post,
        Get
    }

}