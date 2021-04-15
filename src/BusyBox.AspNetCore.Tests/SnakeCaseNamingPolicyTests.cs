using System;
using BusyBox.AspNetCore.Json;
using Xunit;

namespace BusyBox.AspNetCore.Tests
{
    public class SnakeCaseNamingPolicyTests
    {
        [Theory]
        [InlineData("user_name", "UserName")]
        [InlineData("o_data", "OData")]
        public void ConvertNameToSnakeCase(string expected, string value)
        {
            var policy = new SnakeCaseNamingPolicy();
            string result = policy.ConvertName(value);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ShouldHaveErrorWhenValueEmpty()
        {
            var policy = new SnakeCaseNamingPolicy();
            Assert.Throws<ArgumentNullException>(() => policy.ConvertName(string.Empty));
        }
    }
}
