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
        /// The version number to compare.
        /// </summary>
        [Required]
        public string Version { get; set; }
        /// <summary>
        /// The version number to compare <see cref="Version"/> to.
        /// </summary>
        [Required]
        public string CompareTo { get; set; }

        private bool isEqual;
        private bool isLessThan;

        /// <summary>
        /// Returns true if <see cref="Version"/> is equal to <see cref="CompareTo"/>; 
        /// otherwise, false.
        /// </summary>
        [Output]
        public bool IsEqual => isEqual;
        /// <summary>
        /// Returns true if <see cref="Version"/> is not equal to <see cref="CompareTo"/>; 
        /// otherwise, false.
        /// </summary>
        [Output]
        public bool IsNotEqual => !isEqual;
        /// <summary>
        /// Returns true if <see cref="Version"/> is less than <see cref="CompareTo"/>; 
        /// otherwise, false.
        /// </summary>
        [Output]
        public bool IsLessThan => isLessThan;
        /// <summary>
        /// Returns true if <see cref="Version"/> is greater than <see cref="CompareTo"/>; 
        /// otherwise, false.
        /// </summary>
        [Output]
        public bool IsGreaterThan => !isLessThan;
        /// <summary>
        /// Returns true if <see cref="Version"/> is less than or equal to <see cref="CompareTo"/>; 
        /// otherwise, false.
        /// </summary>
        [Output]
        public bool IsLessThanOrEqual => IsLessThan || IsEqual;
        /// <summary>
        /// Returns true if <see cref="Version"/> is greater than or equal to <see cref="CompareTo"/>; 
        /// otherwise, false.
        /// </summary>
        [Output]
        public bool IsGreaterThanOrEqualTo => IsGreaterThan || IsEqual;

        /// <summary>
        /// Compares Microsoft-format version numbers.
        /// </summary>
        /// <returns>
        /// Returns true if <see cref="Version"/> and <see cref="CompareTo"/> could be parsed; 
        /// otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            if (!TryParse(Version, out var version))
            {
                Log.LogError($"{nameof(Version)} is not a Microsoft-format version number.");

                return false;
            }

            if (!TryParse(CompareTo, out var compareTo))
            {
                Log.LogError($"{nameof(CompareTo)} is not a Microsoft-format version number.");

                return false;
            }

            isEqual = version == compareTo;
            isLessThan = version < compareTo;

            return true;
        }
    }
}
