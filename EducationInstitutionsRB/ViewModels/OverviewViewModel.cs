using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationInstitutionsRB.Models;
using EducationInstitutionsRB.Services;
using EducationInstitutionsRB.Views;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EducationInstitutionsRB.ViewModels;

public partial class OverviewViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private int _totalInstitutions;

    [ObservableProperty]
    private int _totalStudents;

    [ObservableProperty]
    private int _totalStaff;

    [ObservableProperty]
    private double _successRate = 98.2;

    [ObservableProperty]
    private List<Institution> _recentInstitutions = new();

    [ObservableProperty]
    private bool _isLoading;

    // Вычисляемые свойства для форматирования
    public string SuccessRateDisplay => $"{SuccessRate:F1}%";
    public string TotalInstitutionsDisplay => TotalInstitutions.ToString("N0");
    public string TotalStudentsDisplay => TotalStudents.ToString("N0");
    public string TotalStaffDisplay => TotalStaff.ToString("N0");

    public OverviewViewModel(IDataService dataService)
    {
        Debug.WriteLine("OverviewViewModel создан");
        _dataService = dataService;
        _ = LoadDataAsync();
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Debug.WriteLine("Начало загрузки данных для Overview");

            var institutions = await _dataService.GetInstitutionsAsync();
            Debug.WriteLine($"Загружено учреждений: {institutions.Count}");

            // Обновляем статистику
            TotalInstitutions = institutions.Count;
            TotalStudents = institutions.Sum(i => i.StudentCount);
            TotalStaff = institutions.Sum(i => i.StaffCount);

            Debug.WriteLine($"Статистика: Учреждений={TotalInstitutions}, Учащихся={TotalStudents}, Персонала={TotalStaff}");

            // Последние добавленные учреждения (последние 3)
            RecentInstitutions = institutions
                .OrderByDescending(i => i.RegistrationDate)
                .Take(3)
                .ToList();

            Debug.WriteLine($"Недавних учреждений: {RecentInstitutions.Count}");

            // Уведомляем об изменении вычисляемых свойств
            OnPropertyChanged(nameof(TotalInstitutionsDisplay));
            OnPropertyChanged(nameof(TotalStudentsDisplay));
            OnPropertyChanged(nameof(TotalStaffDisplay));
            OnPropertyChanged(nameof(SuccessRateDisplay));
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine($"Ошибка загрузки данных в OverviewViewModel: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddInstitutionAsync()
    {
        Debug.WriteLine("Кнопка 'Добавить учреждение' нажата");

        if (App.MainWindow?.Content?.XamlRoot == null)
        {
            Debug.WriteLine("XamlRoot не доступен");
            return;
        }

        try
        {
            var newInstitution = new Institution
            {
                RegistrationDate = System.DateTime.Now,
                Status = "Активно"
            };

            var dialog = new InstitutionDialog(newInstitution, "Добавить учреждение")
            {
                XamlRoot = App.MainWindow.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                Debug.WriteLine("Учреждение добавлено, перезагружаем данные");
                await LoadDataAsync();
            }
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine($"Ошибка при добавлении учреждения: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task GenerateReportAsync()
    {
        Debug.WriteLine("Кнопка 'Создать отчет' нажата");

        if (App.MainWindow?.Content?.XamlRoot == null) return;

        try
        {
            var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
            {
                Title = "Генерация отчетов",
                Content = "Раздел отчетов будет реализован в следующем обновлении",
                CloseButtonText = "OK",
                XamlRoot = App.MainWindow.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine($"Ошибка при показе диалога отчета: {ex.Message}");
        }
    }
}