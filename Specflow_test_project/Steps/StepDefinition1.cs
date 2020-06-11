using Dynamitey.Internal.Optimization;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Internal;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Specflow_test_project.Steps
{
    [Binding]
    public sealed class StepDefinition1
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext context;

        public StepDefinition1(ScenarioContext injectedContext)
        {
            context = injectedContext;
        }

        public string text_data;
        public HttpStatusCode http_code;

        [Given(@"I have sent request using IBAN and token provided")]
        public void GivenIHaveSentRequestUsingIBANAndTokenProvided(Table table)
        {
            dynamic data = table.CreateDynamicInstance();
            string iban = data.iban;
            string token = data.token;
            Console.WriteLine("IBAN received in method is " + iban);
            Console.WriteLine("Token received in method is " + token);

            // Request prepared and sent
            var client = new RestClient("https://api-test.afterpay.dev/");
            var request = new RestRequest("api/v3/validate/bank-account", Method.POST);            
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new {bankAccount = iban });
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-Auth-Key", token);

            // Log outcoming request
            var bodyParameter = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            Console.WriteLine(bodyParameter);

            var response = client.Execute(request);      

            var content = response.Content;
            HttpStatusCode code = response.StatusCode;            

            text_data = content;
            http_code = code;
        }

        [Then(@"the response should be reported")]
        public void ThenTheResponseShouldBeReported()
        
        {            
            Console.WriteLine(text_data);            
        }

        [Then(@"the correct test result should be returned")]
        public void ThenTheCorrectTestResultShouldBeReturned(Table table)                    
        {
            dynamic data = table.CreateDynamicInstance();
            string error_code = data.error_code;
            Console.WriteLine("Expected error code is " + error_code);
            Console.WriteLine("Received response code is " + http_code);

            dynamic data_j = JObject.Parse(text_data);

            if (error_code == "Ok")
            {                
                string message = data_j.isValid;
                Console.WriteLine("Received message is " + message);
                if (message == "True")
                {
                    Console.WriteLine("IBAN validation is successfull.");
                }
                else
                {
                    throw new System.InvalidOperationException("Test is failed. Validation is not correct.");
                }
            }
            else if (error_code == "Unauthorized")
            {                
                string message_j = data_j.message;
                Console.WriteLine("Received message is " + message_j);
                if (message_j == "Authorization has been denied for this request.")
                {
                    Console.WriteLine("IBAN validation returned Unauthorized access error. Test is successfull.");
                }
                else
                {
                    throw new System.InvalidOperationException("Test is failed. Validation is not correct.");
                }
            }
            else throw new System.InvalidOperationException("Test is failed. Incorrect data is received.");

            
        }

    }
}
