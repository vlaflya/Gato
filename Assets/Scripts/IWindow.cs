using System;

public interface IWindow
{
    public event Action<IWindow> RequestOpen;
    public void Open();
    public bool Close();
}
