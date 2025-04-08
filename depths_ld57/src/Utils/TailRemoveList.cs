using System.Collections.Generic;
using System.Linq;
using Godot;

namespace depths_ld57.Utils;

public class TailRemoveList<T>
{
    private readonly List<T> _list;

    public TailRemoveList(int length)
    {
        _list = new List<T>(length);
    }

    public TailRemoveList(List<T> input)
    {
        _list = input;
    }

    public void Add(T value)
    {
        _list.Add(value);
    }
    
    public void Remove(int index)
    {
        if (index == Count - 1)
        {
            _list.RemoveAt(index);
            return;
        }
        var lastElement = _list.Last();
        _list[index] = lastElement;
        _list.RemoveAt(_list.Count - 1);
    }
    
    public int Count => _list.Count;

    public T this[int i]
    {
        get => _list[i];
        set => _list[i] = value;
    }
}