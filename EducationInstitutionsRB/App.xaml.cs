using EducationInstitutionsRB.Services;
using EducationInstitutionsRB.Views;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;

namespace EducationInstitutionsRB;

public partial class App : Application
{
    private Window? _window;
    public static Window? MainWindow { get; private set; }
    private static IDataService? _dataService;
    private static DialogService? _dialogService;

    public App()
    {
        this.InitializeComponent();

        // Обработчик неперехваченных исключений
        this.UnhandledException += App_UnhandledException;

        Debug.WriteLine("=== ПРИЛОЖЕНИЕ ЗАПУЩЕНО ===");

        try
        {
            // Создаем сервисы
            _dataService = new DataService();
            _dialogService = new DialogService();
            Debug.WriteLine("Сервисы инициализированы успешно");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка инициализации сервисов: {ex.Message}");
            Debug.WriteLine($"StackTrace: {ex.StackTrace}");
        }
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Debug.WriteLine($"=== НЕПЕРЕХВАЧЕННОЕ ИСКЛЮЧЕНИЕ ===");
        Debug.WriteLine($"Сообщение: {e.Message}");
        Debug.WriteLine($"Исключение: {e.Exception}");
        Debug.WriteLine($"StackTrace: {e.Exception.StackTrace}");
        Debug.WriteLine($"=== КОНЕЦ ИСКЛЮЧЕНИЯ ===");

        e.Handled = true; // Предотвращаем краш приложения
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            Debug.WriteLine("OnLaunched начат");
            _window = new MainWindow();
            MainWindow = _window;
            _window.Activate();
            Debug.WriteLine("Главное окно создано и активировано");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка в OnLaunched: {ex.Message}");
            Debug.WriteLine($"StackTrace: {ex.StackTrace}");
        }
    }

    public static T GetService<T>() where T : class
    {
        if (typeof(T) == typeof(IDataService) && _dataService is T dataService)
        {
            return dataService;
        }

        if (typeof(T) == typeof(DialogService) && _dialogService is T dialogService)
        {
            return dialogService;
        }

        throw new InvalidOperationException($"Service {typeof(T)} not registered");
    }
}