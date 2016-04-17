using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twitter.App.Controllers.APIControllers;
using Twitter.App.Controllers.V2Controllers;
using Twitter.App.Models.ViewModels;
using Twitter.Data.UnitOfWork;
using Xunit;

namespace Twitter.Tests
{
    public class V2TweetsControllerUnitTest
    {
        [Fact]
        public void GetAllTweets_ReturnOK()
        {
            // Arrange
            var allTweets = new[] {
                new TweetViewModel { Id=111, Text = "fake1"},
                new TweetViewModel { Id=112, Text = "fake2"},
            };
            
            var controller = new TweetsController();

            // Act
            var result = controller.GetAll();

            // Assert
            Assert.Equals(allTweets, result);
        }

    }
}
