using System;

namespace IoT.AWS.DeviceSimulator
{
    public static class Constants
    {
        // TODO: You need to replace this with the name of the AWS IoT policy to use
        public const string AwsIoTPolicyName = "device-policy-dev";

        // TODO: You need to replace this with the topic enabled per thing in its AWS IoT Core policy
        public static string TopicForThing(string thingName) => $"dev/{thingName}/message";

        public static void FailUntilConstantsHaveBeenCustomized()
        {
            // This is in here to ensure you customize the code before running.
            // See https://d1.awsstatic.com/whitepapers/Designing_MQTT_Topics_for_AWS_IoT_Core.pdf and https://docs.aws.amazon.com/iot/latest/developerguide/security-best-practices.html as a guide to design topics and policies.
            // After customizing, you can delete this method and its call.
            throw new Exception("When running the simulator the first time after cloning this repo, you will have to set Constants.AwsIoTPolicyName and Constants.TopicForThing to values specific to your project.");
        }
    }
}
