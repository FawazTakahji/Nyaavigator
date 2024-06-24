using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Nyaavigator.Models;

// https://stackoverflow.com/a/13303245/22970706
public class SmartCollection<T> : ObservableCollection<T>
{
    public SmartCollection() : base()
    {
    }

    public SmartCollection(IEnumerable<T> collection) : base(collection)
    {
    }

    public SmartCollection(List<T> list) : base(list)
    {
    }

    public virtual void AddRange(IEnumerable<T> range)
    {
        foreach (var item in range)
        {
            Items.Add(item);
        }

        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public virtual void Reset(IEnumerable<T> range)
    {
        Items.Clear();
        AddRange(range);
    }
}