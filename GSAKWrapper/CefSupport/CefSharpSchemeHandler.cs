using CefSharp;
using GSAKWrapper.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace GSAKWrapper.CefSupport
{
    //https://github.com/cefsharp/CefSharp/blob/cefsharp/43/CefSharp.Example/CefSharpSchemeHandler.cs
    public class CefSharpSchemeHandler : IResourceHandler
    {
        private string mimeType;
        private MemoryStream stream;

        public CefSharpSchemeHandler(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
        }

        public System.IO.Stream GetResponse(IResponse response, out long responseLength, out string redirectUrl)
        {
            responseLength = stream.Length;
            redirectUrl = null;

            response.StatusCode = (int)HttpStatusCode.OK;
            response.StatusText = "OK";
            response.MimeType = mimeType;

            return stream;
        }

        public bool ProcessRequestAsync(IRequest request, ICallback callback)
        {
            var uri = new Uri(request.Url);
            var fileName = uri.ToString().Substring(6);


            Uri u = ResourceHelper.GetResourceUri(fileName);
            if (u != null)
            {
                Task.Run(() =>
                {
                    using (callback)
                    {
                        try
                        {
                            StreamResourceInfo info = Application.GetResourceStream(u);
                            byte[] buffer = new byte[info.Stream.Length];
                            info.Stream.Read(buffer, 0, buffer.Length);
                            stream = new MemoryStream(buffer);

                            var fileExtension = Path.GetExtension(fileName);
                            mimeType = ResourceHandler.GetMimeType(fileExtension);

                            callback.Continue();
                        }
                        catch
                        {
                            //todo: unknown image
                        }
                    }
                });

                return true;
            }
            else
            {
                callback.Dispose();
            }

            return false;
        }
    }
}
