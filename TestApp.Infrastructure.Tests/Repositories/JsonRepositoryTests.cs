using Microsoft.Extensions.Options;
using Moq;
using System.Text;
using System.Text.Json;
using TestApp.Application.Models;
using TestApp.Infrastructure.Factories;
using TestApp.Infrastructure.Options;
using TestApp.Infrastructure.Repositories;

namespace TestApp.Infrastructure.Tests.Repositories;

public class JsonRepositoryTests
{
    private JsonSerializerOptions _options;

    public JsonRepositoryTests()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async void OpenAsync_ShouldOpenFile_IfDataExists()
    {
        // Arrange
        var testData = new Signal[] { new Signal("Test 1", "Test 1", 1, "Boolean") };
        var json = JsonSerializer.Serialize(testData, _options);

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var reader = new StreamReader(memoryStream);

        var options = new JsonOptions();

        var mockOptions = new Mock<IOptions<JsonOptions>>();
        mockOptions.Setup(x => x.Value).Returns(options);

        var mockFactory = new Mock<IFileStreamFactory>();
        mockFactory.Setup(x => x.CreateReader(It.IsAny<string>())).Returns(reader);

        var repository = new JsonRepository(mockOptions.Object, mockFactory.Object);

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
        var writer = new StreamWriter(memoryStream);

        var options = new JsonOptions();

        var mockOptions = new Mock<IOptions<JsonOptions>>();
        mockOptions.Setup(x => x.Value).Returns(options);

        var mockFactory = new Mock<IFileStreamFactory>();
        mockFactory.Setup(x => x.CreateWriter(It.IsAny<string>())).Returns(writer);

        var repository = new JsonRepository(mockOptions.Object, mockFactory.Object);

        // Act
        await repository.SaveAsync(testData);

        // Assert
        var json = Encoding.UTF8.GetString(memoryStream.ToArray());
        var signals = JsonSerializer.Deserialize<List<Signal>>(json, _options);
        Assert.Equal(1, signals.Count());
        Assert.Equal(testData[0].Name, signals.ElementAt(0).Name);
        Assert.Equal(testData[0].Shape, signals.ElementAt(0).Shape);
        Assert.Equal(testData[0].Value, signals.ElementAt(0).Value);
        Assert.Equal(testData[0].DataType, signals.ElementAt(0).DataType);
    }
}