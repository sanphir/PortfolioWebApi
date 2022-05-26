using System.Text.RegularExpressions;

namespace Portfolio.WebApi.Helpers
{
    public static class StringHelper
    {
        private const string GUID_PATTERN = @"(?im)^[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$";

        /// <summary>
        /// Является ли строк GUID
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsGuid(this string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            return Regex.Match(value, GUID_PATTERN).Success;
        }

        public static bool IsEmptyGuid(this string value)
        {
            if (value.IsGuid())
            {
                return Guid.Parse(value) == Guid.Empty;
            }
            return false;
        }
    }
}
