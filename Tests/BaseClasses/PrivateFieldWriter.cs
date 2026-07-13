using System.Reflection;

namespace ScenesLoaderSystem.Tests
{
    public static class PrivateFieldWriter
    {
        public static void WriteWithName(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType()
                .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            field.SetValue(target, value);
        }
    }
}
