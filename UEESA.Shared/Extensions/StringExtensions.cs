using System;
using System.Collections.Generic;
using System.Linq;

namespace UEESA
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string haystack)
        {
            if (haystack == null)
            {
                throw new ArgumentNullException("haystack");
            }
            return haystack.Length == 0;
        }

        public static bool IsWhitespace(this string haystack)
        {
            if (haystack == null)
            {
                throw new ArgumentNullException("haystack");
            }
            if (haystack.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < haystack.Length; i++)
            {
                if (haystack[i] != " ".ToCharArray()[0])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ContainsAny(this string haystack, params string[] needles)
        {
            if (haystack == null)
            {
                throw new ArgumentNullException("haystack");
            }
            foreach (string value in needles)
            {
                if (haystack.Contains(value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAll(this string haystack, params string[] needles)
        {
            if (haystack == null)
            {
                throw new ArgumentNullException("haystack");
            }
            foreach (string value in needles)
            {
                if (!haystack.Contains(value))
                {
                    return false;
                }
            }
            return true;
        }

        public static int NthIndexOf(this string str, string lookup, int nth = 1)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            List<int> locationsOfString = GetLocationsOfString(str, lookup);
            if (locationsOfString.Count < nth)
            {
                throw new ArgumentOutOfRangeException("str");
            }
            return locationsOfString[nth];
        }

        public static int NthLastIndexOf(this string str, string lookup, int nth = 1)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            List<int> locationsOfString = GetLocationsOfString(str, lookup);
            if (locationsOfString.Count < nth)
            {
                throw new ArgumentOutOfRangeException("str");
            }
            return locationsOfString[locationsOfString.Count - nth];
        }

        public static List<int> GetLocationsOfString(string str, string lookup)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            List<int> list = new List<int>();
            while (str.Any() && str.IndexOf(lookup) != -1)
            {
                list.Add(str.LastIndexOf(lookup));
                str = str.Substring(0, str.LastIndexOf(lookup));
            }
            return list;
        }

        public static bool EndsWithAny(this string str, IEnumerable<string> possibilities)
        {
            foreach (string possibility in possibilities)
            {
                if (str.EndsWith(possibility))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
