using Amazon.IoT;
using Amazon.IoT.Model;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace IoT.AWS.DeviceSimulator
{
    public class ThingCredentials
    {
        private readonly AmazonIoTClient amazonIoTClient;
        private readonly string pem;
        private readonly string key;
        private readonly string certificateArn;

        public ThingCredentials(AmazonIoTClient amazonIoTClient, string thingName, string pem, string key, string certificateArn)
        {
            this.amazonIoTClient = amazonIoTClient;
            ThingName = thingName;
            this.pem = pem;
            this.key = key;
            this.certificateArn = certificateArn;
        }

        public async Task Deprovision()
        {
            Console.WriteLine("Do you want to deprovision the simulated device \"" + ThingName + "\"? Type [y|Y]es if you want to deprovision");
            if (Console.ReadLine().ToLowerInvariant().StartsWith("y"))
            {
                var certificateId = GetCertificateIdFromArn(certificateArn);
                await amazonIoTClient.DetachThingPrincipalAsync(ThingName, certificateArn);
                await amazonIoTClient.DetachPolicyAsync(new DetachPolicyRequest
                {
                    PolicyName = Constants.AwsIoTPolicyName,
                    Target = certificateArn
                });
                await amazonIoTClient.UpdateCertificateAsync(certificateId, CertificateStatus.INACTIVE);
                await amazonIoTClient.DeleteCertificateAsync(certificateId);
                await amazonIoTClient.DeleteThingAsync(ThingName);
            }
        }

        public X509Certificate2 Certificate(X509Certificate2 amazonRootCertificate) 
        {
            var devicePrivateKey = new RSACryptoServiceProvider();
            devicePrivateKey.ImportRSAPrivateKey(Convert.FromBase64String(key), out _);
            var certificate = new X509Certificate2(Encoding.UTF8.GetBytes(pem)).CopyWithPrivateKey(devicePrivateKey);

            return new X509Certificate2(
               new X509Certificate2Collection(
                   new X509Certificate2[] { amazonRootCertificate, certificate }
               ).Export(X509ContentType.Pfx)
           );
        }

        public Task<string> IoTCoreEndpoint => amazonIoTClient.DescribeEndpointAsync(new DescribeEndpointRequest { EndpointType = "iot:Data-ATS" }).ContinueWith(e => e.Result.EndpointAddress);

        public string ThingName { get; }

        private static string GetCertificateIdFromArn(string certificateArn)
        {
            return certificateArn.Substring(certificateArn.IndexOf(":cert/") + 6);
        }
    }
}
