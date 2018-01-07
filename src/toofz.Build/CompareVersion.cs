using System;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace toofz.Build
{
    /// <summary>
    /// Compares Microsoft-format version numbers.
    /// </summary>
    public sealed class CompareVersion : Task
    {
        /// <summary>
        /// The first version number to compare.
        /// </summary>
        [Required]
        public string Version1 { get; set; }

        /// <summary>
        /// The operator to use for comparison. Valid values are 'Equal', 'NotEqual', 'LessThan', 
        /// 'GreaterThan', 'LessThanOrEqual', 'GreaterThanOrEqual'.
        /// </summary>
        [Required]
        public string Operator { get; set; }

        /// <summary>
        /// The second version number to compare.
        /// </summary>
        [Required]
        public string Version2 { get; set; }

        /// <summary>
        /// Returns true if the comparison is true; otherwise, false.
        /// </summary>
        [Output]
        public bool Result { get; private set; }

        /// <summary>
        /// Compares Microsoft-format version numbers.
        /// </summary>
        /// <returns>
        /// Returns true if <see cref="Version1"/> and <see cref="Version2"/> could be parsed; 
        /// otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            if (!Version.TryParse(Version1, out var version1))
            {
                Log.LogError($"'{nameof(Version1)}' is not a valid version number.");

                return false;
            }

            if (!Enum.TryParse(Operator, false, out Operator @operator))
            {
                var operatorValues = Enum
                    .GetValues(typeof(Operator))
                    .Cast<Operator>()
                    .Select(o => $"'{o.ToString()}'");
                var operators = string.Join(", ", operatorValues);
                Log.LogError($"'{nameof(Operator)}' must be one of the following: {operators}.");

                return false;
            }

            if (!Version.TryParse(Version2, out var version2))
            {
                Log.LogError($"'{nameof(Version2)}' is not a valid version number.");

                return false;
            }

            switch (@operator)
            {
                case Build.Operator.Equal: Result = version1 == version2; break;
                case Build.Operator.NotEqual: Result = version1 != version2; break;
                case Build.Operator.LessThan: Result = version1 < version2; break;
                case Build.Operator.GreaterThan: Result = version1 > version2; break;
                case Build.Operator.LessThanOrEqual: Result = version1 <= version2; break;
                case Build.Operator.GreaterThanOrEqual: Result = version1 >= version2; break;
            }

            return true;
        }
    }

    internal enum Operator
    {
        Equal,
        NotEqual,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
    }
}
