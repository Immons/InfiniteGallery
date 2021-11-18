using System.Collections.Generic;
using FluentAssertions;
using InfiniteGallery.Endpoints;
using InfiniteGallery.Models.Data;
using InfiniteGallery.Services.Data;
using NSubstitute;
using NUnit.Framework;

namespace InfiniteGallery.Tests.Unit
{
    [TestFixture]
    public class PhotoServiceTests
    {
        private PhotoService _photoService;
        private IPhotoEndpoint _endpointSubstitute;

        [SetUp]
        public void Setup()
        {
            _endpointSubstitute = Substitute.For<IPhotoEndpoint>();
            _photoService = new PhotoService(_endpointSubstitute);
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(50)]
        public void Photos_ShouldBePopulatedWithExactResultFromEndpoint_WhenSuccess(int photosCount)
        {
            //Arrange
            const int photosLimit = 5;
            var expectedList = new List<PhotoDTO>();
            for (var i = 0; i < photosCount; i++)
            {
                expectedList.Add(Substitute.For<PhotoDTO>());
            }

            _endpointSubstitute.GetImages(Arg.Any<int>(), photosLimit).Returns(expectedList);

            //Act
            _photoService.GetImages(photosLimit);

            //Assert
            _photoService
                .Photos
                .Count
                .Should()
                .Be(expectedList.Count);
        }
    }
}