using System.Windows;
using System.Windows.Threading;
using HLab.UI;

namespace HLab.Ui.Wpf;

public class GuiTimer : IGuiTimer
{
    readonly DispatcherTimer _timer;
    //readonly Dispatcher _dispatcher;

    public GuiTimer()
    {
        //_dispatcher = Application.Current.Dispatcher;
        _timer = new DispatcherTimer(DispatcherPriority.DataBind);
        _timer.Tick += _timer_Tick;
    }

    void _timer_Tick(object sender, EventArgs e)
    {
        Tick?.Invoke(sender, e);
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();
    public void DoTick()
    {
        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() => Tick?.Invoke(this, new EventArgs())));
        //Dispatcher.UIThread.Post(() => Tick?.Invoke(this, new EventArgs()));
        _timer.Start();
    }

    public TimeSpan Interval
    {
        get => _timer.Interval;
        set => _timer.Interval = value;
    }

    public bool IsEnabled
    {
        get => _timer.IsEnabled; 
        set => _timer.IsEnabled = value;
    }

    public event EventHandler Tick;
}