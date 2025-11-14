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
    private readonly Stopwatch _stopwatch = new Stopwatch();
    private bool _isInitialized = false;

    public SplashWindow()
    {
        try
        {
            this.InitializeComponent();
            _stopwatch.Start();

            // Подписываемся на событие Activated
            this.Activated += SplashWindow_Activated;

            Debug.WriteLine("SplashWindow: Конструктор завершен");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SplashWindow: Ошибка в конструкторе: {ex.Message}");
        }
    }

    private void SplashWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        // Защита от повторного выполнения
        if (_isInitialized) return;
        _isInitialized = true;

        Debug.WriteLine("SplashWindow: Activated событие вызвано");

        // Отписываемся от события
        this.Activated -= SplashWindow_Activated;

        // Разворачиваем на весь экран
        SetupFullScreen();

        // Запускаем таймер перехода
        _ = StartTransitionTimer();
    }

    private void SetupFullScreen()
    {
        try
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.Maximize();
                Debug.WriteLine("SplashWindow: Окно развернуто на полный экран");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SplashWindow: Ошибка разворачивания: {ex.Message}");
        }
    }

    private async Task StartTransitionTimer()
    {
        try
        {
            Debug.WriteLine("SplashWindow: Таймер запущен");

            // Ждем всего 800 миллисекунд
            await Task.Delay(2000);

            Debug.WriteLine($"SplashWindow: Таймер завершен. Прошло времени: {_stopwatch.ElapsedMilliseconds}ms");

            // Переходим на главное окно
            ShowMainWindow();

            // Закрываем сплеш-окно
            this.Close();

            Debug.WriteLine("SplashWindow: Окно закрыто");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SplashWindow: Ошибка в таймере: {ex.Message}");
            // В случае ошибки все равно пытаемся открыть главное окно
            ShowMainWindow();
        }
    }

    private void ShowMainWindow()
    {
        try
        {
            Debug.WriteLine("SplashWindow: Создание главного окна...");

            var mainWindow = new MainWindow();
            mainWindow.Activate();

            Debug.WriteLine("SplashWindow: Главное окно активировано");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SplashWindow: Ошибка создания главного окна: {ex.Message}");
        }
    }
}