using System;

namespace Pomelo.EntityFrameworkCore.MySql
{
    public interface IMySqlJsonSerializer
    {
        public string Name { get; }
    }

    public class MicrosoftJsonSerializer : IMySqlJsonSerializer
    {
        private static readonly Lazy<MicrosoftJsonSerializer> _instance = new Lazy<MicrosoftJsonSerializer>();
        public static MicrosoftJsonSerializer Instance => _instance.Value;

        public MicrosoftJsonSerializer()
        {
        }

        public string Name => "System.Text.Json";
    }
}
