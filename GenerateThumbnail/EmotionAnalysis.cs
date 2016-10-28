using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Twitter.App.EmotionAnalysis
{
    public static class EmotionAnalysis
    {
        private static readonly string EmotionKey = "54ac45aeec71428dbe6b89ebbbb9a855";
        private static readonly string APIHost = "https://api.projectoxford.ai/emotion/v1.0/recognize";

        public static List<Emotion> Go(string photoUrl)
        {
            string postData = $"{{\"url\": \"{photoUrl}\"}}";
            var encoding = new ASCIIEncoding();
            var bodyByte = encoding.GetBytes(postData);

            var request = (HttpWebRequest)WebRequest.Create(APIHost);
            request.Headers.Add("Ocp-Apim-Subscription-Key", EmotionKey);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json";

            request.ContentLength = bodyByte.Length;
            var newStream = request.GetRequestStream();

            newStream.Write(bodyByte, 0, bodyByte.Length);

            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                var queryResult = JsonConvert.DeserializeObject<List<Emotion>>(result);
                return queryResult;
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }

        public class Emotion
        {
            [JsonProperty("faceRectangle")]
            public FaceRetangle FaceRetangle { get; set; }

            [JsonProperty("scores")]
            public Scores Scores { get; set; }
        }

        public class FaceRetangle
        {
            [JsonProperty("left")]
            public int Left { get; set; }

            [JsonProperty("top")]
            public int Top { get; set; }

            [JsonProperty("width")]
            public int Width { get; set; }

            [JsonProperty("height")]
            public int Height { get; set; }
        }

        public class Scores
        {
            [JsonProperty("anger")]
            public double Anger { set; get; }

            [JsonProperty("contempt")]
            public double Contempt { set; get; }

            [JsonProperty("disgust")]
            public double Disgust { set; get; }

            [JsonProperty("fear")]
            public double Fear { set; get; }

            [JsonProperty("happiness")]
            public double Happiness { set; get; }

            [JsonProperty("neutral")]
            public double Neutral { set; get; }

            [JsonProperty("sadness")]
            public double Sadness { set; get; }

            [JsonProperty("surprise")]
            public double Surprise { set; get; }
        }
    }
}
