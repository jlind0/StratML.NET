using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using StratML.Core;
using Microsoft.AspNetCore.Http;

namespace StratML.Web.Formatters
{
    public class StratMLOutputFormatter : TextOutputFormatter
    {
        public StratMLOutputFormatter()
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
}
