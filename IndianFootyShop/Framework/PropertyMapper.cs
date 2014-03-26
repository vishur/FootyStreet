using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// This class maps two Properties or Fields of unlike types.
    /// </summary>
    public static class PropertyMapper
    {
        private static readonly string[] Yeses = new[] { "Y", "YES", "T", "TRUE", "1" };
        private static readonly string[] Nos = new[] { "N", "NO", "F", "FALSE", "0" };

        /// <summary>
        /// Converts a string to a boolean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool? StringToNullableBool(string source)
        {

            if (source == null)
            {
                return null;
            }

            bool? result = null;

            source = source.ToUpper(CultureInfo.InvariantCulture);

            if (Yeses.Any(y => y == source))
            {
                result = true;
            }
            else if (Nos.Any(n => n == source))
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Converts a string to a boolean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool StringToBool(string source)
        {
            if (source == null)
            {
                return false;
            }

            source = source.ToUpper(CultureInfo.InvariantCulture);

            return Yeses.Any(y => y == source);
        }

        /// <summary>
        /// Converts a bool to a string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string NullableBoolToString(bool? source)
        {
            return source.HasValue ? BoolToString(source.Value) : null;
        }

        /// <summary>
        /// Converts a bool to a string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string BoolToString(bool source)
        {
            return source ? "Y" : "N";
        }
    }
}
