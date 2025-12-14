using System.Collections.ObjectModel;
using TestApp.Application.Models;

namespace TestApp.Application.Services;

public interface ISignalService
{
    ObservableCollection<Signal> GetSignals();

    void ResetSignals(IEnumerable<Signal> signals);

    void AddSignal(Signal signal);

    void GenerateRandomSignals(int count);
}