using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using Fixit_Viber.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using ExifLib;
using Newtonsoft.Json;

namespace Fixit_Viber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileInputController : ControllerBase
    {
        // POST: api/MobileInput
        [HttpPost]
        public void Post([FromBody] MobileData value)
        {
            string strUserHandle = value.userhandle;
            // ... Code to validate the input data, initialize the variables for picture processing task.
            #endregion

            #region resize image file
            // ... Code to resize incoming picture files down to maximum allowed without losing useful data.
            #endregion

            #region validateMovieFile
			// ... Code to validate the movie files.
            #endregion
    
            #region parseFileInfo
            // ... Code to parse the metadata from the picture file.e.g. Longitutude, latitude, altitude, etc. 
            #endregion

            #region composeJson
            // ... Code to compose JSON
            #endregion

        #region postJson_FixItDBItem_n_Picture

            string postURL = Globals.FixitBEAPI_Intranet_HTTP_URL; 
            string apiExt = "api/fixitbe";
            string apiLogExt = "api/log";
            string strResult = postJson(sb.ToString(), postURL, apiExt, apiLogExt, strUserHandle, strFilename, actionStr, value.clientid);
        #endregion

        #region postJson_PicturInfo_to_PictureTable
           // ... Code to compose JSON variable (sb) from the input + extracted data.
		   //	Post the JSON data to the API to insert a new Incident record in the to database downstream.
                strResult = postJson(sb.ToString(), postURL, apiExt, "", strUserHandle, strFilename, "", value.clientid.ToString());
            
        #endregion  
        }

       
        public static string postJson(string JsonStr, string postURL, string apiExt, string apiLogExt, string strUserHandle, string strFilename, string actionStr, string clientid)
        {
            string strResult = "";
            string statusMessage = "";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(postURL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // List all Names.  
            var content = new StringContent(JsonStr, Encoding.UTF8, "application/json");
            
    
            var result = client.PostAsync(postURL + "/" + apiExt, content).Result;
            
            if (result.IsSuccessStatusCode) 
            {
                strResult = result.StatusCode.ToString();
            }
            #region Log_the_transaction         
           if(apiLogExt != "") { // Do this if logging is to be done only.
                if (result.ReasonPhrase == "OK") {
                    statusMessage = "Success";
                   } 
                else
                {
                    statusMessage = result.ReasonPhrase;
                }
                    string strLogJson = "{   \"userhandle\": \"{UserHandle}\"," +
                 "\"statuscode\": \"{StatusCode}\"," +
                 "\"statusmessage\": \"{StatusMessage}\"," +
                 "\"filename\": \"{FileName}\", " +
                 "\"clientid\": \"{ClientId}\", " +
                  "\"action\": \"{action}\"}";
                strLogJson = strLogJson.Replace("{UserHandle}", strUserHandle);
                strLogJson = strLogJson.Replace("{StatusCode}", result.StatusCode.ToString());
                strLogJson = strLogJson.Replace("{StatusMessage}", statusMessage);
                strLogJson = strLogJson.Replace("{FileName}", strFilename);
                strLogJson = strLogJson.Replace("{ClientId}", clientid);
                strLogJson = strLogJson.Replace("{action}", actionStr);
            
                #endregion
                var contentLog = new StringContent(strLogJson, Encoding.UTF8, "application/json");
                postURL = postURL + "/" + apiLogExt;
                result = client.PostAsync(postURL, contentLog).Result;
            }
            return strResult;
        }
       
    }
}
