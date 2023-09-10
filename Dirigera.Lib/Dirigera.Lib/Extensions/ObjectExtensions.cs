using System.Text.Json;

namespace Dirigera.Lib.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object obj) {
            var res = JsonSerializer.Serialize(obj, new JsonSerializerOptions()
            {
                WriteIndented = true
            });
            return res;
        }
    }
}
