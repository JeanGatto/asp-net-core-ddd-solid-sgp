using FluentAssertions;
using SGP.Infrastructure.Services;
using SGP.Shared.Interfaces;
using SGP.SharedTests.Constants;
using System;
using Xunit;
using Xunit.Categories;

namespace SGP.UnitTests.Infrastructure.Services
{
    [UnitTest(TestCategories.Infrastructure)]
    public class LocalDateTimeServiceTests
    {
        [Fact]
        public void Sould_ReturnDateNow()
        {
            // Arrange
            var dateTimeService = CreateDateTime();

            // Act
            var actual = dateTimeService.Now;

            // Assert
            actual.Should().BeSameDateAs(DateTime.Now);
        }

        private static IDateTime CreateDateTime()
            => new LocalDateTimeService();
    }
}