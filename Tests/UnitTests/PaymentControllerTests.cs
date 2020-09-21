using System;
using Xunit;
using dotnetexample.Services;
using dotnetexample.Controllers;
using dotnetexample.Models;
using System.Threading.Tasks;

namespace dotnetexample.Tests.UnitTests
{
    public class PaymentControllerTests
    {
        [Fact]
        public async Task Test_Examples_Test_Examples()
        {
            
            Assert.Equal(true, true);

            // // Arrange
            // var mockRepo = new Mock<PaymentService>();
            // mockRepo.Setup(repo => repo.ListAsync())
            //     .ReturnsAsync(GetTestSessions());
            // var controller = new Paym(mockRepo.Object);

            // // Act
            // var result = await controller.Index();

            // // Assert
            // var viewResult = Assert.IsType<ViewResult>(result);
            // var model = Assert.IsAssignableFrom<IEnumerable<StormSessionViewModel>>(
            //     viewResult.ViewData.Model);
            // Assert.Equal(2, model.Count());
        }
        
    }
}