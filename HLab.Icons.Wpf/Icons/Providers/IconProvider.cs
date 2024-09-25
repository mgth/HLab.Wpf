using HLab.Mvvm.Annotations;
using Mages.Core.Ast;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows;

namespace HLab.Icons.Wpf.Icons.Providers;

public abstract class IconProvider : IIconProvider
{
    public object? Get(uint foregroundColor = 0) => GetPooled(foregroundColor)?? GetIcon(foregroundColor);
    public async Task<object?> GetAsync(uint foregroundColor = 0) => GetPooled(foregroundColor)??await GetIconAsync(foregroundColor);

    protected abstract object? GetIcon(uint foregroundColor = 0);
    protected abstract Task<object?> GetIconAsync(uint foregroundColor = 0);
    public abstract Task<string> GetTemplateAsync(uint foregroundColor = 0);


    readonly ConcurrentQueue<object> _pool = new();
    object? GetPooled(uint foregroundColor)
    {
        while (_pool.TryDequeue(out var pooledIcon))
        {
            if (pooledIcon is FrameworkElement { Parent: null })
                return pooledIcon;
        }

        return null;
    }
}