using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Turbocharged.Beanstalk.Tests
{
    public static class Settings
    {
        static IConfigurationRoot Configuration { get; } =
            new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("settings.json")
            .Build();

        public static string BeanstalkHostName
        {
            get
            {
                var fromEnv = Environment.GetEnvironmentVariable("BEANSTALK_HOSTNAME");
                return fromEnv ?? Configuration["beanstalk.hostname"];
            }
        }

        public static int BeanstalkPort
        {
            get
            {
                var fromEnv = Environment.GetEnvironmentVariable("BEANSTALK_PORT");
                return int.Parse(fromEnv ?? Configuration["beanstalk.port"]);
            }
        }
    }
}
