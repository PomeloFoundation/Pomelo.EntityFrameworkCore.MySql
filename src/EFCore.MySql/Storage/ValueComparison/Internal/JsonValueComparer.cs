using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pomelo.EntityFrameworkCore.MySql.Storage.ValueComparison.Internal
{
    public class JsonValueComparer<T> : ValueComparer<T>
    {
        public JsonValueComparer(ValueConverter valueConverter)
            : base(
                (left, right) => object.Equals(valueConverter.ConvertToProvider(left), valueConverter.ConvertToProvider(right)),
                value => value.GetHashCode(),
                value => (T)valueConverter.ConvertFromProvider(valueConverter.ConvertToProvider(value)))
        {
        }
    }
}
