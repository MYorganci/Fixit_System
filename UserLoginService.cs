using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Login.Data;
using Newtonsoft.Json;

namespace BlazorApp1.Data
{
    public class UserLoginDataService
    {
        public bool SetUserLoginDataAsync(UserLogin userRecord)
        {
            string strResult;
            string postURL = Globals.FixitBEAPI_Remote_HTTP_URL + "/api/UserLoginCheck";
            string apiDataExt = "";
            string passwordB64 = "";
            string strJson = "{\"clientid\": \"{clientid}\",\"password\": \"{password}\"}";
            string[] usrRecord;
            bool flag = false;

            apiDataExt = userRecord.UserHandle.Trim();
			// Convert the local alphabet special characters so they do not cause issues in the HTTPS call.
			// The reverse conversion must be performed at the receiving end first, before trying to process the HTTPS Call.
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(replaceLocalAlphabetChars(userRecord.Password.Trim()));
            passwordB64 = System.Convert.ToBase64String(plainTextBytes);
            strJson = strJson.Replace("{password}", passwordB64);
            strJson = strJson.Replace("{clientid}", userRecord.ClientId);
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(postURL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(strJson, Encoding.UTF8, "application/json");
			// Post the Login request
            var result = client.PostAsync(postURL + "/" + apiDataExt, content).Result;
			//Get the result of the Login Request
            strResult = result.Content.ReadAsStringAsync().Result;

            if (result.ReasonPhrase == "OK")
            {// If successfull, the returned data pack has pieces of information about the user which will be used throughout the user session.
                var json_data = JsonConvert.DeserializeObject(strResult);

                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
                UserLogin jsonObj = JsonConvert.DeserializeObject<UserLogin>(json_data.ToString(), settings);
				// Parse teh data pack into their respective user record fields.
                if (jsonObj.userInfoPack != "")
                {
                    strResult = jsonObj.userInfoPack;
                    usrRecord = strResult.Split("~|~");
                    userRecord.Name = usrRecord[2];
                    userRecord.CountryCode = usrRecord[3];
                    userRecord.City = usrRecord[4].Replace("\"", "").Trim();
                    userRecord.District = usrRecord[5];
                    if (userRecord.CountryCode != "" || userRecord.City != "'")
                    {
                        flag = true;
                    }
                }
            }
            else
            {
                //Login Failed. already logged in the backend into LoginHistory log table
            }
            userRecord.Password = ""; // Clear the password as it has served its purpose.
            return flag;
        }
        public string replaceLocalAlphabetChars(string inputStr)
        {
           // Content removed for safety of the method.

            return inputStr;
        }
    }

}
