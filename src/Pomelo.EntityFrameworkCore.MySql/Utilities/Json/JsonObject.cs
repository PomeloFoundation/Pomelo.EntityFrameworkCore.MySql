// ReSharper disable once CheckNamespace
namespace System
{
  public class JsonObject : JsonObject<object>
  {
    public JsonObject(string json)
      : base(json)
    {
    }
  }
}