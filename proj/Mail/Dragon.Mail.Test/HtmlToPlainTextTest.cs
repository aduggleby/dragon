using System;
using Dragon.Mail.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Mail.Test
{
    [TestClass]
    public class HtmlToPlainTextTest
    {
        [TestMethod]
        public void Basic()
        {
            const string html =
            @"<html lang=""en""
<head>
   <meta charset=""utf-8"">
   <title>Search - NuGet Must Haves</title>
   <link href=""/Content/themes/base/css?v=2KXSKRXfv8eCp_qIaLfMVXBpCPQIUI9obcF5sp1sbso1"" rel=""stylesheet"" type=""text/css"">
   <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
   <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
   <!--[if lt IE 9]>
   <script src=""xxx""></script>
   <![endif]-->
   <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
   <style type=""text/css""></style>
</head>
<body data-pinterest-extension-installed=""cr1.37"">
   <div class=""container"">
      <div class=""row"">
         <h3>Search results for html text</h3>
         <ul>
            <li>-list1</li>

            <li>-list2</li>
            <li>-list3</li>
         </ul>
      </div>
      <!--/row-->
      <hr>
      <footer>
         <p>footer</p>
      </footer>
   </div>
   <!--/.container-->
   <!-- jQuery (necessary for Bootstrap's JavaScript plugins) -->
   <script src=""xxx""></script>
</body>
</html>";

            const string plain = @"Search results for html text

-list1
-list2
-list3

footer

";


            Assert.AreEqual(plain, HtmlToPlainText.ConvertHtml(html));



        }
    }
}
