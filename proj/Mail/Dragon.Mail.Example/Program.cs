using Dragon.Mail.Impl;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Basic();
        }

        static void Basic()
        {
            // Create the main generator service
            var generatorService = new MailGeneratorService();

            // Use template helper to load templates from folder 
            // and add to generator service
            var templateFolder = new FileFolderTemplateRepository();
            templateFolder.EnumerateTemplates(generatorService.Register);

            /*
             * Send basic templated email with the service
             */
            generatorService.Send(
                /* the recipient, email is required, fullname is optional */
                new { email = "test@example.org" },
                /* the name of the folder to take the template files from */
                "template1",
                /* data passed to the template for e-mail generation      */
                new { title = "Example", link = "http://www.google.com" });

            /*
             * Send template in different language
             */
            generatorService.Send(
                new { email = "test@example.org" },
                "template1",
                new { title = "Example", link = "http://www.google.com" },
                /* pass a specific culture to use the template in that language */
                /* if no template for that culture exists, it will fall back to */
                /* the default culture specific in application configuration    */
                CultureInfo.GetCultureInfo("de"));

            /*
             * Send template with more complex data
             */
            dynamic data = new ExpandoObject();
            data.title = "Another example";

            dynamic link1 = new ExpandoObject();
            link1.name = "Google";
            link1.link = "http://www.google.com";

            dynamic link2 = new ExpandoObject();
            link2.name = "Bing";
            link2.link = "http://www.bing.com";

            dynamic link3 = new ExpandoObject();
            link3.name = "DuckDuckGo";
            link3.link = "http://www.duckduckgo.com";

            data.links = new[] { link1, link2, link3 };

            generatorService.Send(new { email = "test@example.org" }, "template2", data);


            /*
             * Send asynchronous email (generatorService must be configuration with async=true)
             */
            generatorService.Send(
                new
                {
                    email = "test@example.org",
                    userid = "unique-identifier-for-user-1",
                    bufferHours = 4,
                    ignoreBuffer = false,
                    flushBuffer = false
                },
                "template1",
                new { title = "Example", link = "http://www.google.com" },
                CultureInfo.GetCultureInfo("de"));


          /*
           * using REST resolver
           */
            generatorService.Send(
                new Uri("http://example.org/api/v1/users/123"),
                "template1",
                new {
                    title = "Example",
                    link = "http://www.google.com",
                    order = new Uri("http://example.org/api/v1/orders/17823")
                },
                CultureInfo.GetCultureInfo("de"));


        }
    }
}
