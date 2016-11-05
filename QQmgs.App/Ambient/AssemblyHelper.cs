using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Twitter.App.Ambient
{
    public static class AssemblyHelper
    {
        public static string GetCurrentVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly()
                .GetName()
                .Version
                .ToString();
        }
    }
    
}