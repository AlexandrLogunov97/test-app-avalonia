using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using TestApp.Application.Services;

namespace TestApp.Presentation.ViewModels;

public class ChartViewModel : ViewModelBase
{
    private readonly ISignalService _signalService;

    private readonly Dictionary<Guid, IPlottable> _signalPoints = [];
    private readonly Dictionary<Guid, IPlottable> _signalLabels = [];

    private Plot _currentPlot => PlotControl?.Plot!;

    public AvaPlot? PlotControl { get; } = new();

    public ChartViewModel(ISignalService signalService)
    {
        _signalService = signalService;
        var signals = _signalService.GetSignals();
        signals.CollectionChanged += OnSignalsChanged;

        CofigurePlot();

        InitializeChart(signals);
    }

    private void OnSignalsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
        {
            foreach (Application.Models.Signal signal in args.NewItems)
            {
                AddSignalToChart(signal);
            }
        }
        else if (args.Action == NotifyCollectionChangedAction.Remove && args.OldItems != null)
        {
            foreach (Application.Models.Signal signal in args.OldItems)
            {
                RemoveSignalFromChart(signal.Id);
            }
        }
        else if (args.Action == NotifyCollectionChangedAction.Reset)
        {
            ClearChart();
        }
    }

    private void AddSignalToChart(Application.Models.Signal signal)
    {
        var index = _signalPoints.Count;
        var scatter = _currentPlot.Add.Scatter(index, signal.Value);

        _signalPoints[signal.Id] = scatter;

        var text = _currentPlot.Add.Text(
            text: signal.Name,
            x: index,
            y: signal.Value
        );
        text.LabelFontSize = 10;
        text.LabelFontColor = Colors.Red;

        _signalLabels[signal.Id] = text;
    }

    private void RemoveSignalFromChart(Guid signalId)
    {
        if (_signalPoints.TryGetValue(signalId, out var point))
        {
            _currentPlot.Remove(point);
            _signalPoints.Remove(signalId);
        }

        if (_signalLabels.TryGetValue(signalId, out var label))
        {
            _currentPlot.Remove(label);
            _signalLabels.Remove(signalId);
        }
    }

    private Application.Models.Signal? GetSignalById(Guid signalId)
    {
        foreach (var signal in _signalService.GetSignals())
        {
            if (signal.Id == signalId)
                return signal;
        }

        return null;
    }

    private void InitializeChart(IEnumerable<Application.Models.Signal> signals)
    {
        foreach (var signal in signals)
        {
            AddSignalToChart(signal);
        }

        _currentPlot.Axes.AutoScale();
    }

    private void ClearChart()
    {
        foreach (var point in _signalPoints.Values)
        {
            _currentPlot.Remove(point);
        }
        foreach (var label in _signalLabels.Values)
        {
            _currentPlot.Remove(label);
        }

        _signalPoints.Clear();
        _signalLabels.Clear();
    }

    private void CofigurePlot()
    {
        _currentPlot.FigureBackground = new BackgroundStyle
        {
            Color = Colors.Transparent,
        };

        _currentPlot.Axes.Color(Colors.White);
    }
}