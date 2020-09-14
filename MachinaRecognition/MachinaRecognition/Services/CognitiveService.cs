using MachinaRecognition.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MachinaRecognition.Services
{
   public static class CognitiveService
    {
        #region Clé d'api et url
        private static readonly string API_KEY = "30784024fc2b4adfacd741e735bfb49c";
        private static readonly string ENDPOINT_URL = "https://roniapivision.cognitiveservices.azure.com/face/v1.0/";
        #endregion

        public static async Task<FaceDetectResult> FaceDetect(Stream imageStream)
        {
            if (imageStream == null) 
                return null; 

            var URL = ENDPOINT_URL + "detect" + "?returnFaceAttributes=age,gender";

            using (var webClient = new WebClient())
            {
                try
                {
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
                    webClient.Headers.Add("Ocp-Apim-Subscription-Key", API_KEY);

                    var data = ReadStream(imageStream); // permet de convertir le stream en byte

                    var Result = await Task.Run(() => webClient.UploadData(URL, data));

                    if (Result == null) 
                        return null;

                    string json = Encoding.UTF8.GetString(Result, 0, Result.Length);
                   

                 var faceResult=  JsonConvert.DeserializeObject<FaceDetectResult[]>(json);
                    if (faceResult.Length >= 1)
                    {
                        return faceResult[0];
                    }

                }
                catch (Exception Ex)
                {
                    Console.WriteLine("Exception du webclient"+ Ex.Message);
                }
                return null;
            }

        }

        private static byte[] ReadStream(Stream input)
        {
            BinaryReader b = new BinaryReader(input);
            byte[] data = b.ReadBytes((int)input.Length);
            return data;
        }
    }
}
