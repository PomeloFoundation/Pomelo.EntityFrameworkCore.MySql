using static Newtonsoft.Json.JsonConvert;

// ReSharper disable once CheckNamespace
namespace System
{
  public class JsonObject<T> : IEquatable<JsonObject<T>>, IEquatable<JsonObject>, IEquatable<string>
    where T : class
  {
    private string _originalValue { get; set; }
    private T _originalObject { get; set; }
    private Type _internalType { get; set; }

    public JsonObject()
    {
      _internalType = typeof(T);
    }

    public JsonObject(T instance)
      : this()
    {
      Object = instance;
    }

    public JsonObject(string json)
      : this()
    {
      Json = json;
      Object = Object;
    }

    public T Object
    {
      get { return _originalObject; }
      set
      {
        _originalObject = value;

        _originalValue = _originalObject != null
          ? SerializeObject(Object)
          : string.Empty;
      }
    }

    public string Json
    {
      get { return _originalValue; }
      set
      {
        try
        {
          Object = string.IsNullOrWhiteSpace(value)
            ? default(T)
            : DeserializeObject<T>(value);

          _originalValue = value;
        }
        catch
        {
          Object = null;
          _originalValue = string.Empty;
        }
      }
    }

    public override string ToString()
    {
      return Json;
    }

    public override bool Equals(object obj)
    {
      if (obj == null && Json == Null)
        return true;

      if (obj.GetType().Name == "String")
      {
        var objString = obj as string;
        if (objString == NaN && Json == NaN)
          return false;

        return Equals(objString);
      }

      try
      {
        dynamic o = obj;
        return Equals((string) o.Json);
      }
      catch
      {
        return base.Equals(obj);
      }
    }

    public bool Equals(JsonObject<T> other)
    {
      if (other == null && Json == Null)
        return true;

      return Equals(other.Json);
    }

    public bool Equals(JsonObject other)
    {
      if (other == null && Json == Null)
        return true;

      return Equals(other.Json);
    }

    public bool Equals(string other)
    {
      if (other == NaN && Json == NaN)
        return false;

      if (string.IsNullOrWhiteSpace(other) || IsJsonConstant(other))
        return string.CompareOrdinal(other, Json) == 0;

      if (!IsSameType(Json, other))
        return false;

      var tempJsonObject = new JsonObject(other);
      return GetHashCode() == tempJsonObject.GetHashCode();
    }

    private Type GetInternalObjectType()
    {
      return _internalType;
    }

    public static implicit operator JsonObject<T>(byte[] serialized)
    {
        return new JsonObject<T>
        {
            Json = Text.Encoding.UTF8.GetString(serialized)
        };
    }

    public static implicit operator JsonObject<T>(string json)
    {
      return new JsonObject<T>(json);
    }

    public static implicit operator JsonObject<T>(T obj)
    {
      return new JsonObject<T>(obj);
    }

    public static implicit operator JsonObject<T>(JsonObject<object> obj)
    {
      return new JsonObject<T>(obj.Json);
    }

    private static bool IsObject(string json)
    {
      JsonObject jsonObject;
      return IsObject(json, out jsonObject);
    }

    private static bool IsObject(string json, out JsonObject jsonObject)
    {
      jsonObject = null;

      if (string.IsNullOrWhiteSpace(json) || IsJsonConstant(json))
        return false;

      if (string.CompareOrdinal(json, Null) == 0)
        return true;

      jsonObject = new JsonObject(json);

      return jsonObject.Object != null;
    }

    private static bool IsJsonConstant(string json)
    {
      if (string.CompareOrdinal(json, NaN) == 0 ||
          string.CompareOrdinal(json, Undefined) == 0 ||
          string.CompareOrdinal(json, True) == 0 ||
          string.CompareOrdinal(json, False) == 0 ||
          string.CompareOrdinal(json, NegativeInfinity) == 0 ||
          string.CompareOrdinal(json, PositiveInfinity) == 0)
        return true;

      return false;
    }

    private static bool IsSameType(string json1, string json2)
    {
      if (string.IsNullOrWhiteSpace(json1) && string.IsNullOrWhiteSpace(json2)
          || json1 == Null && json2 == Null)
        return true;

      if (IsJsonConstant(json1) && IsJsonConstant(json2))
        return json1 == json2;

      JsonObject left, right;
      if (!IsObject(json1, out left) || !IsObject(json2, out right))
        return false;

      return left.GetInternalObjectType().FullName == right.GetInternalObjectType().FullName;
    }

    public static bool operator ==(JsonObject<T> a, JsonObject<T> b)
    {
      return a.Equals(b);
    }

    public static bool operator !=(JsonObject<T> a, JsonObject<T> b)
    {
      return !a.Equals(b);
    }

    public override int GetHashCode()
    {
      return Json?.GetHashCode() ?? 0;
    }
  }
}