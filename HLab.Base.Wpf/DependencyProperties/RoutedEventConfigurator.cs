using System;
using System.Windows;

namespace HLab.Base.Wpf.DependencyProperties;

public class RoutedEventConfigurator<TClass, TValue>(string name)
    where TClass : DependencyObject
{
    RoutingStrategy _routingStrategy = RoutingStrategy.Tunnel;

    public RoutedEvent Register() => EventManager.RegisterRoutedEvent(
        name,
        _routingStrategy,
        typeof(TValue),
        typeof(TClass)
    );

    RoutedEventConfigurator<TClass, TValue> Do(Action action)
    {
        action();
        return this;
    }

    public RoutedEventConfigurator<TClass, TValue> Tunnel => Do(() => _routingStrategy = RoutingStrategy.Tunnel);
    public RoutedEventConfigurator<TClass, TValue> Bubble => Do(() => _routingStrategy = RoutingStrategy.Bubble);
    public RoutedEventConfigurator<TClass, TValue> Direct => Do(() => _routingStrategy = RoutingStrategy.Direct);
}