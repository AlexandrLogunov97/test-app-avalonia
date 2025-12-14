using Microsoft.Extensions.Options;
using Moq;
using System.Xml.Serialization;
using TestApp.Application.Models;
using TestApp.Infrastructure.Factories;
using TestApp.Infrastructure.Options;
using TestApp.Infrastructure.Repositories;

namespace TestApp.Infrastructure.Tests.Repositories;

public class XmlRepositoryTests
{
    [Fact]
    public async void OpenAsync_ShouldOpenFile_IfDataExists()
    {
        // Arrange
        var testData = new Signal[] { new Signal("Test 1", "Test 1", 1, "Boolean") };
        var serializer = new XmlSerializer(typeof(List<Signal>), new XmlRootAttribute("Signals"));
        var memoryStream = new MemoryStream();
        serializer.Serialize(memoryStream, new List<Signal>(testData));
        memoryStream.Position = 0;
        var reader = new StreamReader(memoryStream);


        var options = new XmlOptions();
        var mockOptions = new Mock<IOptions<XmlOptions>>();
        mockOptions.Setup(x => x.Value).Returns(options);

        var mockFactory = new Mock<IFileStreamFactory>();
        mockFactory.Setup(x => x.CreateReader(It.IsAny<string>())).Returns(reader);

        var repository = new XmlRepository(mockOptions.Object, mockFactory.Object);

        // Act
        var result = await repository.OpenAsync();

        // Assert
        Assert.Equal(1, result.Count());
        Assert.Equal(testData[0].Name, result.ElementAt(0).Name);
        Assert.Equal(testData[0].Shape, result.ElementAt(0).Shape);
        Assert.Equal(testData[0].Value, result.ElementAt(0).Value);
        Assert.Equal(testData[0].DataType, result.ElementAt(0).DataType);
    }

    [Fact]
    public async void SaveAsync_ShouldSaveDataToFile_IfOperationSuccess()
    {
        // Arrange
        var testData = new Signal[] { new Signal("Test 1", "Test 1", 1, "Boolean") };

        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream, leaveOpen: true);

        var options = new XmlOptions();

        var mockOptions = new Mock<IOptions<XmlOptions>>();
        mockOptions.Setup(x => x.Value).Returns(options);

        var mockFactory = new Mock<IFileStreamFactory>();
        mockFactory.Setup(x => x.CreateWriter(It.IsAny<string>())).Returns(writer);

        var repository = new XmlRepository(mockOptions.Object, mockFactory.Object);

        // Act
        await repository.SaveAsync(testData);

        // Assert

        var serializer = new XmlSerializer(typeof(List<Signal>), new XmlRootAttribute("Signals"));
        memoryStream.Position = 0;
        var signals = serializer.Deserialize(memoryStream) as List<Signal>;

        Assert.Equal(1, signals.Count());
        Assert.Equal(testData[0].Name, signals.ElementAt(0).Name);
        Assert.Equal(testData[0].Shape, signals.ElementAt(0).Shape);
        Assert.Equal(testData[0].Value, signals.ElementAt(0).Value);
        Assert.Equal(testData[0].DataType, signals.ElementAt(0).DataType);
    }
}