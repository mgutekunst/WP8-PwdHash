using System;

namespace PwdHash.Portable.Utils
{
    public static class DomainExtractor
    {
         public static string Extract(string url)
		    {
                if(!url.StartsWith("http"))
                {
                    url = "http://" + url;
                }

			    try
			    {
				    var uri = new Uri(url);
				    return uri.Host;
			    }
			    catch (Exception ufe)
			    {
				    return String.Empty;
			    }
		    }
    }
}