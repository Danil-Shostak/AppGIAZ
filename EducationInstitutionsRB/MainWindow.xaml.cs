using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WinRT.Interop;

namespace EducationInstitutionsRB;

public sealed partial class MainWindow : Window
{
    private AppWindow _appWindow;
    private bool _isInitialized = false;

    public MainWindow()
    {
        try
        {
            this.InitializeComponent();

            // Подписываемся на событие Activated
            this.Activated += MainWindow_Activated;

            // Быстрая базовая настройка
            SetupBasicWindow();

            Debug.WriteLine("MainWindow: Конструктор завершен");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MainWindow: Ошибка в конструкторе: {ex.Message}");
        }
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (_isInitialized) return;
        _isInitialized = true;

        // Отписываемся от события
        this.Activated -= MainWindow_Activated;

        Debug.WriteLine("MainWindow: Activated - завершаем настройку");

        // Завершаем настройку после показа окна
        CompleteWindowSetup();
    }

    private void SetupBasicWindow()
    {
        try
        {
            // Минимальная настройка для быстрого показа
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(DragRegion);

            // Получаем AppWindow
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = AppWindow.GetFromWindowId(windowId);

            // Сразу разворачиваем и скрываем стандартный title bar
            if (_appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.SetBorderAndTitleBar(false, false);
                presenter.Maximize();
            }

            Debug.WriteLine("MainWindow: Базовая настройка завершена");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MainWindow: Ошибка базовой настройки: {ex.Message}");
        }
    }

    private void CompleteWindowSetup()
    {
        try
        {
            // Устанавливаем фон асинхронно (может быть медленным)
            _ = Task.Run(async () =>
            {
                await Task.Delay(100); // Небольшая задержка чтобы не мешать показу окна

                _ = this.DispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        this.SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
                        Debug.WriteLine("MainWindow: Mica backdrop установлен");
                    }
                    catch
                    {
                        try
                        {
                            this.SystemBackdrop = new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop();
                            Debug.WriteLine("MainWindow: Acrylic backdrop установлен");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"MainWindow: Ошибка установки backdrop: {ex.Message}");
                        }
                    }
                });
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MainWindow: Ошибка завершения настройки: {ex.Message}");
        }
    }

    // Обработчики кнопок управления окном
    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.Minimize();
        }
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_appWindow.Presenter is OverlappedPresenter presenter)
        {
            if (presenter.State == OverlappedPresenterState.Maximized)
            {
                presenter.Restore();
                MaximizeButton.Content = "&#xE922;";
            }
            else
            {
                presenter.Maximize();
                MaximizeButton.Content = "&#xE923;";
            }
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void RootNavigation_Loaded(object sender, RoutedEventArgs e)
    {
        ContentFrame.Navigate(typeof(Views.OverviewPage));
        RootNavigation.SelectedItem = RootNavigation.MenuItems[0];
    }

    private void RootNavigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer is NavigationViewItem item)
        {
            var pageType = item.Tag?.ToString() switch
            {
                "Overview" => typeof(Views.OverviewPage),
                "Institutions" => typeof(Views.InstitutionsPage),
                "Import" => typeof(Views.ImportPage),
                "Reports" => typeof(Views.ReportsPage),
                "Admin" => typeof(Views.AdminPage),
                _ => typeof(Views.OverviewPage)
            };

            ContentFrame.Navigate(pageType);
        }
    }

    private void RootNavigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (ContentFrame.CanGoBack)
        {
            ContentFrame.GoBack();
        }
    }
}