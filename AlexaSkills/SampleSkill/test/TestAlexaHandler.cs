// -*- coding: utf-8 -*-

// Copyright 2018 Amazon.com, Inc. or its affiliates. All Rights Reserved.

// Licensed under the Amazon Software License (the "License"). You may not use this file except in
// compliance with the License. A copy of the License is located at

//    http://aws.amazon.com/asl/

// or in the "license" file accompanying this file. This file is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using Xunit;
using AlexaSmartHomeLambda;

namespace AlexaSmartHomeLambda.Tests
{
   
    public class AlexaHandlerUnitTest
    {
        private static readonly string sampleUri = "https://raw.githubusercontent.com/alexa/alexa-smarthome/master/sample_messages/";
            
        public static Stream CreateStream(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        
        private string GetSample(string url)
        {           
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        [Fact]
        public void Test_Authorization()
        {
            using (Stream r = CreateStream(GetSample(sampleUri + "Authorization/Authorization.AcceptGrant.request.json")))
            {           
                // Arrange
                AlexaHandler ah = new AlexaHandler();
                
                // Act
                Stream responseStream = ah.Handler(r, null);
                string responseString = string.Empty;
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    responseString = reader.ReadToEnd();
                }
                JObject response = JObject.Parse(responseString);

                System.Console.WriteLine(response);

                // Asert
                Assert.Equal("Alexa.Authorization", response["event"]["header"]["namespace"].ToString());
                Assert.Equal("AcceptGrant.Response", response["event"]["header"]["name"].ToString());                
            }
        }

        [Fact]
        public void Test_Discovery()
        {
            using (Stream r = CreateStream(GetSample(sampleUri + "Discovery/Discovery.request.json")))
            {            
                AlexaHandler ah = new AlexaHandler();
                Stream responseStream = ah.Handler(r, null);

                string responseString = string.Empty;
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    responseString = reader.ReadToEnd();
                }
        
                JObject response = JObject.Parse(responseString);

                System.Console.WriteLine(response);

                Assert.Equal("Alexa.Discovery", response["event"]["header"]["namespace"].ToString());
                Assert.Equal("Discover.Response", response["event"]["header"]["name"].ToString()); 
            }
        }

        [Fact]
        public void Test_PowerController_Off()
        {
            using (Stream r = CreateStream(GetSample(sampleUri + "PowerController/PowerController.TurnOff.request.json")))
            {            
                AlexaHandler ah = new AlexaHandler();
                Stream responseStream = ah.Handler(r, null);

                string responseString = string.Empty;
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    responseString = reader.ReadToEnd();
                }
        
                JObject response = JObject.Parse(responseString);

                System.Console.WriteLine(response);

                Assert.Equal("Alexa", response["event"]["header"]["namespace"].ToString());
                Assert.Equal("Response", response["event"]["header"]["name"].ToString()); 
                Assert.Equal("Alexa.PowerController", response["context"]["properties"][0]["namespace"].ToString());
                Assert.Equal("powerState", response["context"]["properties"][0]["name"].ToString());
                Assert.Equal("OFF", response["context"]["properties"][0]["value"]["value"].ToString());
            }
        }

        [Fact]
        public void Test_SendDeviceState_Off()
        {
            AlexaHandler ah = new AlexaHandler();
            bool result = ah.SendDeviceState("test", "powerState", "OFF");
            System.Console.WriteLine(result);
        }
        
        [Fact]
        public void Test_SendDeviceState_On()
        {
            AlexaHandler ah = new AlexaHandler();
            bool result = ah.SendDeviceState("test", "powerState", "ON");
            System.Console.WriteLine(result);
        }
    }
}