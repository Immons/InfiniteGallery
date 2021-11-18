using System.Threading.Tasks;
using FluentAssertions;
using InfiniteGallery.Configuration;
using InfiniteGallery.Endpoints;
using NUnit.Framework;
using Refit;

namespace InfiniteGallery.Tests.Integration
{
    [TestFixture]
    public class PhotoEndpointTests
    {
        private IPhotoEndpoint _photoEndpoint;

        [SetUp]
        public void Setup()
        {
            _photoEndpoint = RestService.For<IPhotoEndpoint>(AppConstants.BaseUrl);
        }

        [TestCase(10, 5)]
        [TestCase(50, 2)]
        public async Task Photos_ShouldBeReturned_WithExactCount(int photosCount, int photosPage)
        {
            //Arrange

            //Act
            var result = await _photoEndpoint.GetImages(photosPage, photosCount);

            //Assert
            result
                .Count
                .Should()
                .Be(photosCount);
        }

        [Test]
        public async Task Photos_ShouldNotBeReturned_WhenPageInvalid()
        {
            //Arrange

            //Act
            var result = await _photoEndpoint.GetImages(int.MaxValue, 10);

            //Assert
            result
                .Should()
                .BeEmpty();
        }
    }
}