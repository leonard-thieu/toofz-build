using Xunit;

namespace toofz.Build.Tests
{
    public class CompareVersionTests
    {
        [DisplayTheory]
        [InlineData("2.4.1.5", "Equal", "2.4.1.5", true)]
        [InlineData("2.4.1.5", "Equal", "5.1.4.0", false)]
        [InlineData("2.4.1.5", "NotEqual", "2.4.1.5", false)]
        [InlineData("2.4.1.5", "NotEqual", "5.1.4.0", true)]
        [InlineData("2.4.1.5", "LessThan", "3.0.6.7", true)]
        [InlineData("3.0.6.7", "LessThan", "2.4.1.5", false)]
        [InlineData("2.4.1.5", "GreaterThan", "3.0.6.7", false)]
        [InlineData("3.0.6.7", "GreaterThan", "2.4.1.5", true)]
        [InlineData("2.4.1.5", "LessThanOrEqual", "3.0.6.7", true)]
        [InlineData("3.0.6.7", "LessThanOrEqual", "2.4.1.5", false)]
        [InlineData("3.0.6.7", "LessThanOrEqual", "3.0.6.7", true)]
        [InlineData("2.4.1.5", "GreaterThanOrEqual", "3.0.6.7", false)]
        [InlineData("3.0.6.7", "GreaterThanOrEqual", "2.4.1.5", true)]
        [InlineData("3.0.6.7", "GreaterThanOrEqual", "3.0.6.7", true)]
        public void ComparesVersionNumbers(string version1, string @operator, string version2, bool result)
        {
            // Arrange
            var t = new CompareVersion();
            t.BuildEngine = new MockEngine();

            t.Version1 = version1;
            t.Operator = @operator;
            t.Version2 = version2;

            // Act
            var success = t.Execute();

            // Assert
            Assert.True(success);
            Assert.Equal(t.Result, result);
        }

        [DisplayFact(nameof(CompareVersion.Version1))]
        public void Version1IsInvalid_Fails()
        {
            // Arrange
            var t = new CompareVersion();
            var engine = new MockEngine();
            t.BuildEngine = engine;

            t.Version1 = "NotARealVersion";
            t.Operator = "NotEqual";
            t.Version2 = "2.7.1.0";

            // Act
            var success = t.Execute();

            // Assert
            Assert.False(success);
            Assert.Equal(1, engine.ErrorEvents.Count);
        }

        [DisplayFact]
        public void OperatorIsInvalid_Fails()
        {
            // Arrange
            var t = new CompareVersion();
            var engine = new MockEngine();
            t.BuildEngine = engine;

            t.Version1 = "2.7.1.0";
            t.Operator = "==";
            t.Version2 = "2.7.1.0";

            // Act
            var success = t.Execute();

            // Assert
            Assert.False(success);
            Assert.Equal(1, engine.ErrorEvents.Count);
        }

        [DisplayFact(nameof(CompareVersion.Version2))]
        public void Version2IsInvalid_Fails()
        {
            // Arrange
            var t = new CompareVersion();
            var engine = new MockEngine();
            t.BuildEngine = engine;

            t.Version1 = "2.7.1.0";
            t.Operator = "NotEqual";
            t.Version2 = "NotARealVersion";

            // Act
            var success = t.Execute();

            // Assert
            Assert.False(success);
            Assert.Equal(1, engine.ErrorEvents.Count);
        }
    }
}
