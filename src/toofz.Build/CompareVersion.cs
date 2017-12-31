using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using static System.Version;

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
        public string Version1
        {
            get { return version1.ToString(); }
            set { version1 = Parse(value); }
        }
        private Version version1;

        /// <summary>
        /// The operator to use for comparison. Valid values are '==', '!=', '&lt;', '&gt;', '&lt;=', '&gt;='.
        /// </summary>
        [Required]
        public string Operator
        {
            get { return @operator; }
            set
            {
                switch (value)
                {
                    case "==":
                    case "!=":
                    case "<":
                    case ">":
                    case "<=":
                    case ">=":
                        @operator = value;
                        break;
                    default:
                        throw new ArgumentException($"'{value}' is not a valid operator. Valid values are '==', '!=', '<', '>', '<=', '>='.");
                }
            }
        }
        private string @operator;

        /// <summary>
        /// The second version number to compare.
        /// </summary>
        [Required]
        public string Version2
        {
            get { return version2.ToString(); }
            set { version2 = Parse(value); }
        }
        private Version version2;

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
            switch (Operator)
            {
                case "==": Result = version1 == version2; break;
                case "!=": Result = version1 != version2; break;
                case "<": Result = version1 < version2; break;
                case ">": Result = version1 > version2; break;
                case "<=": Result = version1 <= version2; break;
                case ">=": Result = version1 >= version2; break;
            }

            return true;
        }
    }
}
