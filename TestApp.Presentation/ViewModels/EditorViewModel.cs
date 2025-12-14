using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using TestApp.Application.Models;
using TestApp.Application.Services;
using TestApp.Infrastructure.Enums;
using TestApp.Infrastructure.Services;
using TestApp.Presentation.Models;

using TestApp.Presentation.ViewModels.Dialogs;

namespace TestApp.Presentation.ViewModels;

public class EditorViewModel : ViewModelBase
{
    private readonly IMapper _mapper;

    private readonly ISignalService _signalService;

    private readonly ISignalsStorageService _signalsStorageService;

    private readonly DialogViewModel _dialog;

    private SignalModel? _currentSignal = new();

    public Validation<SignalModel> SignalValidation { get; }

    public SignalModel? CurrentSignal
    {
        get => _currentSignal;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentSignal, value);
            if (_currentSignal != null)
            {
                SignalValidation.SetSource(_currentSignal);
            }
        }
    }

    public ObservableCollection<string> Shapes { get; }

    public ObservableCollection<string> DataTypes { get; }

    public ReactiveCommand<Unit, Unit> AddCommand { get; }
    public ReactiveCommand<Unit, Unit> NewCommand { get; }
    public ReactiveCommand<Unit, Unit> PopulateCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public EditorViewModel(
        IMapper mapper,
        ISignalService signalService,
        IShapesService shapesService,
        IDataTypesService dataTypesService,
        ISignalsStorageService signalsStorageService,
        IValidator<SignalModel> signalValidator,
        DialogViewModel dialog)
    {
        _mapper = mapper;
        _signalService = signalService;
        _signalsStorageService = signalsStorageService;
        _dialog = dialog;

        SignalValidation = new(x => signalValidator.Validate(x).ToDictionary(), CurrentSignal);

        Shapes = new ObservableCollection<string>(shapesService.GetShapes());
        DataTypes = new ObservableCollection<string>(dataTypesService.GetDataTypes());

        var canExecute = this.WhenAnyValue(x => x.SignalValidation.IsValid);
        AddCommand = ReactiveCommand.Create(AddSignal, canExecute);
        NewCommand = ReactiveCommand.Create(NewSignal);
        PopulateCommand = ReactiveCommand.Create(PopulateSignals);
        OpenCommand = ReactiveCommand.Create(OpenFile);
        SaveCommand = ReactiveCommand.Create(SaveFile);
    }

    private void AddSignal()
    {
        var signal = _mapper.Map<Signal>(CurrentSignal);
        _signalService.AddSignal(signal);
    }

    private void NewSignal()
    {
        CurrentSignal = new SignalModel();
        this.RaisePropertyChanged(nameof(CurrentSignal));
    }

    private void PopulateSignals()
    {
        _signalService.GenerateRandomSignals(10);
    }

    private void OpenFile()
    {
        _dialog.Open<OpenDialogViewModel>(async x =>
        {
            if (x is not StorageType storageType) return;

            var signals = await _signalsStorageService.OpenAsync("signals", storageType);
            _signalService.ResetSignals(signals);
        });
    }

    private void SaveFile()
    {
        _dialog.Open<SaveDialogViewModel>(async x =>
        {
            if (x is not StorageType storageType) return;

            var signals = _signalService.GetSignals();
            await _signalsStorageService.SaveAsync(signals, "signals", storageType);
        });
    }
}