namespace UEESA
{
    public static class TypeExtensions
    {
        public static bool HasMethod(this Type type, string methodnName)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (methodnName == null)
            {
                throw new ArgumentNullException("methodnName");
            }
            try
            {
                return type.GetMethod(methodnName) != null;
            }
            catch (ArgumentNullException)
            {
                throw;
            }
        }
    }
}
