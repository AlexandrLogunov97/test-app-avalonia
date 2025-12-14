using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace TestApp.Presentation.Models;

public class ErrorsObject : ReactiveObject
{
    private bool _hasErrors = false;

    public bool HasErrors
    {
        get => _hasErrors;
        set => this.RaiseAndSetIfChanged(ref _hasErrors, value);
    }

    public ObservableCollection<string> Errors { get; } = [];

    public void Clear()
    {
        HasErrors = false;
        Errors.Clear();
    }

    public void AddErrors(IEnumerable<string> errors)
    {
        if (errors.Any())
        {
            HasErrors = true;
            Errors.AddRange(errors);
        }
    }
}

public class Validation<T> : ReactiveObject
    where T: ReactiveObject
{
    private bool _isValid;

    private Func<T, IDictionary<string, string[]>>? _validation;

    private Dictionary<string, ErrorsObject> _errors = [];

    public bool IsValid
    {
        get => _isValid;
        set => this.RaiseAndSetIfChanged(ref _isValid, value);
    }

    public ErrorsObject this[string property]
    {
        get => _errors[property];
    }

    public Validation(Func<T, IDictionary<string, string[]>>? validation = null, T? value = default(T))
    {
        _validation = validation;
        _errors = typeof(T)
            .GetProperties()
            .Select(x => x.Name)
            .ToDictionary(key => key, value => new ErrorsObject());
        if (value != null)
        {
            SetSource(value);
        }
    }

    public void SetSource(T? source)
    {
        if (source == null) return;

        Update(source);

        source.PropertyChanged -= OnPropertyChanged!;
        source.PropertyChanged += OnPropertyChanged!;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        var obj = (T)sender;
        Update(obj);
    }

    private void Update(T value)
    {
        var results = _validation?.Invoke(value);
        foreach (var errorobject in _errors.Values)
        {
            errorobject.Clear();
        }

        if (results != null)
        {
            foreach (var result in results)
            {
                _errors[result.Key].AddErrors(result.Value);
            }
        }

        IsValid = !_errors.Values.Any(x => x.HasErrors);
    }
}