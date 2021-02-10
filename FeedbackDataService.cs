using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using FeedbackData;

namespace BlazorApp1.Data
{
    public class FeedbackDataService
    {
        public string SetFeedbackDataAsync(FeedbackRecord feedbackRecord)
        {
            string jsonString;
            string postURL = Globals.FixitBEAPI_Intranet_HTTP_URL + "/api/FeedbackData";

            jsonString = "{   \"clientid\":\"{clientid}\",\"userhandle\":\"{userhandle}\", \"status\": {status}, \"category\":\"{category}\",\"comments\": \"{comments}\", ";
            jsonString = jsonString + "\"countrycode\": \"{countrycode}\",\"city\":\"{city}\",\"district\":\"{district}\",\"targetaudience\":\"{targetaudience}\"}";
            jsonString = jsonString.Replace("{comments}", feedbackRecord.comments.Replace("'", @"\'"));
            jsonString = jsonString.Replace("{category}", feedbackRecord.category);
            jsonString = jsonString.Replace("{countrycode}", feedbackRecord.countrycode);
            jsonString = jsonString.Replace("{city}", feedbackRecord.city);
            jsonString = jsonString.Replace("{district}", feedbackRecord.district);
            jsonString = jsonString.Replace("{targetaudience}", feedbackRecord.targetaudience);
            jsonString = jsonString.Replace("{status}", "1");
            jsonString = jsonString.Replace("{userhandle}", feedbackRecord.userhandle);
            jsonString = jsonString.Replace("{clientid}", feedbackRecord.clientid);

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(postURL);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = client.PostAsync(postURL, content).Result;

            return result.ToString();
        }
        
        public List<cityEntry> getCities(string countryCode)
        {
            string postURL = Globals.FixitBEAPI_Intranet_HTTP_URL + "/api/city/" + countryCode.ToString();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(postURL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = client.GetStringAsync(postURL).Result;
            var json_data = JsonConvert.DeserializeObject(json);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;

            List<cityEntry> jsonObj = JsonConvert.DeserializeObject<List<cityEntry>>(json_data.ToString(), settings);

            return jsonObj;
        }
        

    }
  
}
