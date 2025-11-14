using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WinRT.Interop;

namespace EducationInstitutionsRB;

public sealed partial class SplashWindow : Window
{
    private bool _isTitleBarSetup = false;

    public SplashWindow()
    {
        try
        {
            this.InitializeComponent();

            // Подписываемся на событие Activated
            this.Activated += SplashWindow_Activated;

            // Сразу запускаем переход
            _ = StartTransitionImmediately();

            Debug.WriteLine("SplashWindow: Запущен");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SplashWindow: Ошибка: {ex.Message}");
            ShowMainWindow();
        }
    }

    private void SplashWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        // Настраиваем title bar только один раз
        if (!_isTitleBarSetup)
        {
            _isTitleBarSetup = true;
            SetupCustomTitleBar();
        }
    }

    private void SetupCustomTitleBar()
    {
        try
        {
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(HiddenTitleBar);

            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.SetBorderAndTitleBar(false, false);
                presenter.Maximize();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SplashWindow: Ошибка title bar: {ex.Message}");
        }
    }

    private async Task StartTransitionImmediately()
    {
        try
        {
            Debug.WriteLine("SplashWindow: Начало перехода");

            // Короткая задержка - 200ms
            await Task.Delay(2000);

            Debug.WriteLine("SplashWindow: Создание главного окна");

            ShowMainWindow();
            this.Close();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SplashWindow: Ошибка перехода: {ex.Message}");
            ShowMainWindow();
        }
    }

    private void ShowMainWindow()
    {
        try
        {
            var mainWindow = new MainWindow();
            mainWindow.Activate();

            // Устанавливаем главное окно в App
            App.SetMainWindow(mainWindow);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SplashWindow: Ошибка создания главного окна: {ex.Message}");
        }
    }
}