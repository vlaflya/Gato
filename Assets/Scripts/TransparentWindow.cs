using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class TransparentWindow : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll")]
    private static extern int SetActiveWindow(IntPtr hWnd);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [SerializeField]
    private TapObject _tapObject;
    [SerializeField]
    private GameObject _back;

    private struct MARGINS
    {
        public int cxLeftWidth, cxRightWidth, cyTopHeight, cyBottomHeight;
    }

    const int GWL_EXSTYLE = -20;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
    private bool _isOverlay;
    public event Action<bool> OvelayChanged;

    private IntPtr _hWnd;

    public void Initialize()
    {
        _tapObject.OnClick += ChangeScreenOverlay;
#if !UNITY_EDITOR
        SetWindow();
#endif
    }

    private void ChangeScreenOverlay()
    {
        SetOverlay(!_isOverlay);
        OvelayChanged?.Invoke(_isOverlay);
    }
#if !UNITY_EDITOR
    private void SetWindow()
    {
        _hWnd = GetActiveWindow();
        SetWindowLong(_hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);

        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(_hWnd, ref margins);
    }

    private void Update()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        var collider = Physics2D.OverlapPoint(mousePosition);
        bool isOverCollider = collider != null;
        SetClickthrough(!isOverCollider);
    }

    private void SetClickthrough(bool clickthrough)
    {
        if (clickthrough)
            SetWindowLong(GetActiveWindow(), GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        else
        {
            SetActiveWindow(_hWnd);
            SetWindowLong(GetActiveWindow(), GWL_EXSTYLE, WS_EX_LAYERED);
        }
    }
#endif
    public void SetOverlay(bool overlay)
    {
        _isOverlay = overlay;
#if !UNITY_EDITOR
        if (_isOverlay)
        {
            _back.SetActive(false);
            SetWindowPos(_hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
        }
        else
        {
            _back.SetActive(true);
            SetWindowPos(_hWnd, HWND_BOTTOM, 0, 0, 0, 0, 0);
        }
#endif
    }
}
