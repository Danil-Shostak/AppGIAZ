using EducationInstitutionsRB.Services;
using EducationInstitutionsRB.Views;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;

namespace EducationInstitutionsRB;

public partial class App : Application
{
    private static Window? _mainWindow;
    public static Window? MainWindow => _mainWindow;
    private static IDataService? _dataService;
    private static DialogService? _dialogService;

    public App()
    {
        this.InitializeComponent();
        this.UnhandledException += App_UnhandledException;

        Debug.WriteLine("=== ПРИЛОЖЕНИЕ ЗАПУЩЕНО ===");

        try
        {
            _dataService = new DataService();
            _dialogService = new DialogService();
            Debug.WriteLine("Сервисы инициализированы успешно");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка инициализации сервисов: {ex.Message}");
        }
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            Debug.WriteLine("OnLaunched начат");

            // Создаем и показываем окно сплеш-скрина
            var splashWindow = new SplashWindow();
            splashWindow.Activate();

            Debug.WriteLine("Сплеш-окно создано и активировано");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка в OnLaunched: {ex.Message}");

            // Если сплеш-окно не работает, создаем главное окно напрямую
            CreateMainWindowDirectly();
        }
    }

    private void CreateMainWindowDirectly()
    {
        try
        {
            _mainWindow = new MainWindow();
            _mainWindow.Activate();
            Debug.WriteLine("Главное окно создано напрямую");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка создания главного окна: {ex.Message}");
        }
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Debug.WriteLine($"=== НЕПЕРЕХВАЧЕННОЕ ИСКЛЮЧЕНИЕ ===");
        Debug.WriteLine($"Сообщение: {e.Message}");
        e.Handled = true;
    }

    public static T GetService<T>() where T : class
    {
        if (typeof(T) == typeof(IDataService) && _dataService is T dataService)
            return dataService;
        if (typeof(T) == typeof(DialogService) && _dialogService is T dialogService)
            return dialogService;
        throw new InvalidOperationException($"Service {typeof(T)} not registered");
    }
}