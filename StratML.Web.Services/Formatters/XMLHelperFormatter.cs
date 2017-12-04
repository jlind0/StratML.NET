using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using StratML.Core;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Xml.Linq;

namespace StratML.Web.Services.Formatters
{
    public class XMLHelperOutputFormatter : TextOutputFormatter
    {
        public XMLHelperOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/xml"));
            SupportedEncodings.Add(Encoding.UTF8);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            return response.WriteAsync(XmlHelper.Serialize(context.Object, context.Object.GetType()));

        }
    }
    public class XMLHelperInputFormatter : TextInputFormatter
    {
        public XMLHelperInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/xml"));
            SupportedEncodings.Add(Encoding.UTF8);
        }
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(context.HttpContext.Request.Body))
            {
                var data = await reader.ReadToEndAsync();
                var deserialized = XmlHelper.Deserialize(data, context.ModelType);
                return await InputFormatterResult.SuccessAsync(deserialized);
            }
        }
    }
}