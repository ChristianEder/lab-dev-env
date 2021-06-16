using Amazon.IoT;
using Amazon.IoT.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IoT.AWS.DeviceSimulator
{
    public static class DeviceProvisioning
    {
        public static async Task<ThingCredentials> Provision(string thingName)
        {
            // We use the AWS IoT SDK to provision the simulated thing and its certificate used for authentication directly from the simulator.
            // When dealing with "real" devices, the provisioning step would typically happen during physical production of the device.
            // The AmazonIoTClient requires you to be locally authenticated in the AWS CLI ("aws configure")
            var amazonIoTClient = new AmazonIoTClient();

            await EnsureThingExistsInAwsIoTCore(amazonIoTClient, thingName);
            return await GetOrCreateLocallyCachedCertificate(thingName, amazonIoTClient);
        }

        private static async Task<ThingCredentials> GetOrCreateLocallyCachedCertificate(string thingName, AmazonIoTClient amazonIoTClient)
        {
            var principals = await amazonIoTClient.ListThingPrincipalsAsync(thingName);

            if (principals.Principals.Count == 1)
            {
                // If the thing already has a certificate asssigned and we have that certificate cached locally, we'll use that.
                var certificateArn = principals.Principals.Single();
                var certificateId = GetCertificateIdFromArn(certificateArn);
                var pemFile = Path.Combine("certs", certificateId + ".pem");
                var keyFile = Path.Combine("certs", certificateId + ".key");
                if (File.Exists(pemFile) && File.Exists(keyFile))
                {
                    Console.WriteLine($"Certificate \"{certificateId}\" for thing \"{thingName}\" exists locally, will reuse this");
                    return new ThingCredentials(amazonIoTClient, thingName, File.ReadAllText(pemFile), File.ReadAllText(keyFile), certificateArn);
                }
            }

            // If the thing either has more than 1 certificate assigned, or we dont have it cached locally, we delete all certificates in AWS IoT Core...
            foreach (var principal in principals.Principals)
            {
                var certificateId = GetCertificateIdFromArn(principal);
                Console.WriteLine($"Deleting previously used certificate \"{certificateId}\" for thing \"{thingName}\"...");
                await amazonIoTClient.UpdateCertificateAsync(certificateId, CertificateStatus.INACTIVE);
                await amazonIoTClient.DeleteCertificateAsync(certificateId);
                Console.WriteLine("   ... deleted");
            }

            // ... then create a new one...
            System.Console.WriteLine($"Creating new certificate for thing \"{thingName}\"...");
            var createKeysAndCertificateResponse = await amazonIoTClient.CreateKeysAndCertificateAsync(true);
            await amazonIoTClient.AttachPolicyAsync(new AttachPolicyRequest
            {
                PolicyName = Constants.AwsIoTPolicyName,
                Target = createKeysAndCertificateResponse.CertificateArn
            });
            await amazonIoTClient.AttachThingPrincipalAsync(thingName, createKeysAndCertificateResponse.CertificateArn);

            // ... and cache that locally
            var createdCertificateId = GetCertificateIdFromArn(createKeysAndCertificateResponse.CertificateArn);
            var createdPemFile = Path.Combine("certs", createdCertificateId + ".pem");
            var createdKeyFile = Path.Combine("certs", createdCertificateId + ".key");
            if (!Directory.Exists("certs"))
            {
                Directory.CreateDirectory("certs");
            }
            var key = createKeysAndCertificateResponse.KeyPair.PrivateKey.Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "");
            File.WriteAllText(createdPemFile, createKeysAndCertificateResponse.CertificatePem);
            File.WriteAllText(createdKeyFile, key);

            Console.WriteLine($"   ... create and cached locally ({createdCertificateId})");
            return new ThingCredentials(
                amazonIoTClient,
                thingName,
                createKeysAndCertificateResponse.CertificatePem,
                key,
                createKeysAndCertificateResponse.CertificateArn);
        }

        private static async Task EnsureThingExistsInAwsIoTCore(AmazonIoTClient client, string thingName)
        {
            try
            {
                await client.DescribeThingAsync(thingName);
                Console.WriteLine($"Thing with name {thingName} already exists, using this");
            }
            catch (ResourceNotFoundException)
            {
                Console.WriteLine($"Thing with name {thingName} does not exist yet, creating...");
                await client.CreateThingAsync(new CreateThingRequest { ThingName = thingName });
                Console.WriteLine($"   ... created");
            }
        }

        private static string GetCertificateIdFromArn(string certificateArn)
        {
            return certificateArn.Substring(certificateArn.IndexOf(":cert/") + 6);
        }
    }
}
