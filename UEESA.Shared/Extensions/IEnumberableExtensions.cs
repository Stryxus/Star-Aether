using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace UEESA
{
    public static class IEnumberableExtensions
    {
        public static bool Contains(this IEnumerable<FileInfo> enumerable, string lookingFor)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }
            if (lookingFor == null)
            {
                throw new ArgumentNullException("lookingFor");
            }
            foreach (FileInfo item in enumerable)
            {
                if (item.Name == lookingFor)
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, IEnumerable<T> appending)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }
            if (appending == null)
            {
                throw new ArgumentNullException("appending");
            }
            foreach (T item in appending)
            {
                appending.Append(item);
            }
            return enumerable.Distinct();
        }
    }
}
