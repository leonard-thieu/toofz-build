using System;
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
        public string Version1
        {
            get => version1.ToString();
            set => version1 = Version.Parse(value);
        }
        private Version version1;

        /// <summary>
        /// The operator to use for comparison. Valid values are 'Equal', 'NotEqual', 'LessThan', 
        /// 'GreaterThan', 'LessThanOrEqual', 'GreaterThanOrEqual'.
        /// </summary>
        [Required]
        public string Operator
        {
            get => @operator.ToString();
            set => @operator = (Operator)Enum.Parse(typeof(Operator), value, ignoreCase: true);
        }
        private Operator @operator;

        /// <summary>
        /// The second version number to compare.
        /// </summary>
        [Required]
        public string Version2
        {
            get => version2.ToString();
            set => version2 = Version.Parse(value);
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
