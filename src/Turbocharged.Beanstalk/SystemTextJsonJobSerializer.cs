using System.Text.Json;

namespace Turbocharged.Beanstalk
{
    class SystemTextJsonJobSerializer : IJobSerializer
    {
        private readonly JsonSerializerOptions _options;

        public SystemTextJsonJobSerializer()
        {
        }

        public SystemTextJsonJobSerializer(JsonSerializerOptions options)
        {
            _options = options;
        }

        public byte[] Serialize<T>(T job)
        {
            return JsonSerializer.SerializeToUtf8Bytes(job, _options);
        }

        public T Deserialize<T>(byte[] buffer)
        {
            return JsonSerializer.Deserialize<T>(buffer, _options);
        }
    }
}
