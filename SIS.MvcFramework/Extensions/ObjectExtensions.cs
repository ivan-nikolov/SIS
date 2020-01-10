namespace SIS.MvcFramework.Extensions
{
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class ObjectExtensions
    {
        public static string ToXml(this object obj)
        {
            var sb = new StringBuilder();
            using (var stringWriter = new StringWriter(sb))
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(stringWriter, obj);

                return sb.ToString();
            }
        }

        public static string ToJson(this object obj)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
            });
        }
    }
}
