using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.OpsWorks.Model;

namespace Dragon.Files.S3
{
    public class S3Configuration : IS3Configuration
    {
        public string AccessKeyID { get; set; }
        public string AccessKeySecret { get; set; }
        public string Bucket { get; set; }
        public string Region { get; set; }

        public static S3Configuration FromAppConfig()
        {
            return new S3Configuration()
            {
                AccessKeyID = ConfigurationManager.AppSettings["Dragon.Files.S3.AccessKeyID"],
                AccessKeySecret = ConfigurationManager.AppSettings["Dragon.Files.S3.AccessKeySecret"],
                Bucket = ConfigurationManager.AppSettings["Dragon.Files.S3.Bucket"],
                Region = ConfigurationManager.AppSettings["Dragon.Files.S3.Region"]
            };
        }
    }
}
