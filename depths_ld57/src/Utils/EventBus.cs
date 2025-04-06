using System;
using System.Collections.Generic;

namespace depths_ld57.Utils;

public static class EventBus
{
    private static readonly Dictionary<Type, List<object>> Registrations = new(); 
    
    public static void Register<T>(Action<T> action) where T : new()
    {
        if (!Registrations.ContainsKey(typeof(T)))
        {
            Registrations.Add(typeof(T), new List<object>());    
        }
        Registrations[typeof(T)].Add(action);
    }
    
    public static void Emit<T>(T emitted)
    {
        if (!Registrations.ContainsKey(typeof(T))) return;
        
        foreach (Action<T> action in Registrations[typeof(T)])
        {
            action(emitted);
        }
    }

    public static void Deregister<T>(Action<T> action)
    {
        if (!Registrations.ContainsKey(typeof(T))) return;
        Registrations[typeof(T)].Remove(action);
    }
}