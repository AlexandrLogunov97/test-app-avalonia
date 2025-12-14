using AutoMapper;
using DynamicData;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using TestApp.Application.Models;
using TestApp.Application.Services;
using TestApp.Presentation.Models;
using TestApp.Presentation.ViewModels.Dialogs;

namespace TestApp.Presentation.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IMapper _mapper;

    private readonly ISignalService _signalService;

    private SignalModel? _selectedSignal;

    public DialogViewModel DialogContainerViewModel { get; }

    public EditorViewModel EditorViewModel { get; }

    public ChartViewModel ChartViewModel { get; }

    public ObservableCollection<SignalModel> Signals { get; } = [];

    public SignalModel? SelectedSignal
    {
        get => _selectedSignal;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedSignal, value);
            EditorViewModel.CurrentSignal = value?.Clone() ?? new SignalModel();
        }
    }

    public MainWindowViewModel(
        IMapper mapper,
        ISignalService signalService,
        DialogViewModel dialogContainerViewModel,
        EditorViewModel editorViewModel,
        ChartViewModel chartViewModel)
    {
        _mapper = mapper;
        _signalService = signalService;

        DialogContainerViewModel = dialogContainerViewModel;
        EditorViewModel = editorViewModel;
        ChartViewModel = chartViewModel;

        var signals = _signalService.GetSignals();
        signals.CollectionChanged += OnSignalsChanged;

        Signals = _mapper.Map<ObservableCollection<SignalModel>>(signals);
    }

    private void OnSignalsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
        {
            var newSignals = args.NewItems.Cast<Signal>().ToList();
            var newSignalsModels = _mapper.Map<IEnumerable<SignalModel>>(newSignals);
            Signals.AddRange(newSignalsModels);
        }
        else if (args.Action == NotifyCollectionChangedAction.Remove && args.OldItems != null)
        {
            var oldSignals = args.OldItems.Cast<Signal>().ToList();
            var newSignalsModels = _mapper.Map<IEnumerable<SignalModel>>(oldSignals);
            Signals.Remove(newSignalsModels);
        }
        else if (args.Action == NotifyCollectionChangedAction.Reset)
        {
            Signals.Clear();
            SelectedSignal = null;
        }
    }
}