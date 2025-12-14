using AutoMapper;
using Moq;
using TestApp.Application.Models;
using TestApp.Application.Services;
using TestApp.Infrastructure.Services;
using TestApp.Presentation.Models;
using TestApp.Presentation.Validators;
using TestApp.Presentation.ViewModels;
using TestApp.Presentation.ViewModels.Dialogs;

namespace TestApp.Presentation.Tests.ViewModels;

public class EditorViewModelTests
{
    [Fact]
    public void IsValid_ShouldBeTrue_IfModelIsValid()
    {
        // Arrange
        var dataTypes = new List<string>
        {
            "Int32",
            "Double",
            "Boolean"
        };
        var mockDataTypesService = new Mock<IDataTypesService>();
        mockDataTypesService.Setup(x => x.GetDataTypes()).Returns(dataTypes);

        var mockShapesService = new Mock<IShapesService>();
        mockShapesService.Setup(x => x.GetShapes()).Returns(new string[] { "Test"});

        var validator = new SignalValidator(mockDataTypesService.Object);

        var editorViewModel = new EditorViewModel(
            It.IsAny<IMapper>(), 
            It.IsAny<ISignalService>(),
            mockShapesService.Object,
            mockDataTypesService.Object,
            It.IsAny<ISignalsStorageService>(),
            validator,
            It.IsAny<DialogViewModel>());

        // Act
        editorViewModel.CurrentSignal = new SignalModel
        {
            Name = "Test",
            Shape = "Test",
            Value = 1,
            DataType = "Int32"
        };

        // Assert
        Assert.True(editorViewModel.SignalValidation.IsValid);
    }

    [Fact]
    public void IsValid_ShouldBeFalse_IfModelIsNotValid()
    {
        // Arrange
        var dataTypes = new List<string>
        {
            "Int32",
            "Double",
            "Boolean"
        };
        var mockDataTypesService = new Mock<IDataTypesService>();
        mockDataTypesService.Setup(x => x.GetDataTypes()).Returns(dataTypes);

        var mockShapesService = new Mock<IShapesService>();
        mockShapesService.Setup(x => x.GetShapes()).Returns(new string[] { "Test" });

        var validator = new SignalValidator(mockDataTypesService.Object);

        var editorViewModel = new EditorViewModel(
            It.IsAny<IMapper>(),
            It.IsAny<ISignalService>(),
            mockShapesService.Object,
            mockDataTypesService.Object,
            It.IsAny<ISignalsStorageService>(),
            validator,
            It.IsAny<DialogViewModel>());

        // Act
        editorViewModel.CurrentSignal = new SignalModel
        {
            Name = string.Empty,
            Shape = "Test",
            Value = 2,
            DataType = "Boolean"
        };

        // Assert
        Assert.False(editorViewModel.SignalValidation.IsValid);
        Assert.True(editorViewModel.SignalValidation["Name"].HasErrors);
        Assert.True(editorViewModel.SignalValidation["Value"].HasErrors);
    }

    [Fact]
    public void AddSignal_ShouldAddNewSignalViaCommandExecution_IfModelIsValid()
    {
        // Arrange
        var dataTypes = new List<string>
        {
            "Int32",
            "Double",
            "Boolean"
        };
        var mockDataTypesService = new Mock<IDataTypesService>();
        mockDataTypesService.Setup(x => x.GetDataTypes()).Returns(dataTypes);

        var mockShapesService = new Mock<IShapesService>();
        mockShapesService.Setup(x => x.GetShapes()).Returns(new string[] { "Test" });

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(x => x.Map<Signal>(It.IsAny<SignalModel>())).Returns(new Signal());

        var validator = new SignalValidator(mockDataTypesService.Object);
        var signalService = new SignalService(mockShapesService.Object, mockDataTypesService.Object);
        var sourceCollection = signalService.GetSignals();

        var editorViewModel = new EditorViewModel(
            mockMapper.Object,
            signalService,
            mockShapesService.Object,
            mockDataTypesService.Object,
            It.IsAny<ISignalsStorageService>(),
            validator,
            It.IsAny<DialogViewModel>());

        editorViewModel.CurrentSignal = new SignalModel
        {
            Name = "Test",
            Shape = "Test",
            Value = 1,
            DataType = "Boolean"
        };

        // Act
        editorViewModel.AddCommand.Execute().Subscribe();

        // Assert
        Assert.True(editorViewModel.SignalValidation.IsValid);
        Assert.Equal(1, sourceCollection.Count);
    }
}