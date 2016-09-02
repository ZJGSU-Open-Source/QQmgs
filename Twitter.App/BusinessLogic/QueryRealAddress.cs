using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using Twitter.App.DataContracts;

namespace Twitter.App.BusinessLogic
{
    public static class HighAccIpLocationClient
    {
        private const string AccessToken = "znKjR9LSfPVGMWBNBfzzGwh4mmXQaPMR";

        public static HighAccIpLocation Query(string ipAddress, bool needDetailLocation = true)
        {
            var extensionsVersion = needDetailLocation ? 1 : 0;

            var uri = $" http://api.map.baidu.com/highacciploc/v1?ak=&ak={AccessToken}&qcip={ipAddress}&qterm=pc&extensions={extensionsVersion}";

            var request = (HttpWebRequest)WebRequest.Create(uri);
            var response = (HttpWebResponse)request.GetResponse();

            var stream = response.GetResponseStream();
            if (stream != null)
            {
                var result = new StreamReader(stream).ReadToEnd();
                var queryResult = JsonConvert.DeserializeObject<HighAccIpLocation>(result);
                return queryResult;
            }

            return null;
        }
    }
}