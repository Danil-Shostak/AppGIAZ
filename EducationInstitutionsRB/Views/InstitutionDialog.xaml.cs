using EducationInstitutionsRB.Models;
using EducationInstitutionsRB.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducationInstitutionsRB.Views;

public sealed partial class InstitutionDialog : ContentDialog
{
    public Institution Institution { get; set; }
    public string Title { get; set; }

    public List<string> InstitutionTypes { get; } = new()
    {
        "Школа", "Гимназия", "Лицей", "Колледж", "Университет"
    };

    public List<string> StatusTypes { get; } = new()
    {
        "Активно", "Закрыто", "На реконструкции"
    };

    private readonly IDataService _dataService;
    private bool _isLoading = false;

    public InstitutionDialog(Institution institution, string title)
    {
        this.InitializeComponent();
        Institution = institution;
        Title = title;
        _dataService = App.GetService<IDataService>();

        // Загружаем данные
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        if (_isLoading) return;
        _isLoading = true;

        try
        {
            System.Diagnostics.Debug.WriteLine("Начало загрузки данных для диалога...");

            // Загружаем регионы
            var regions = await _dataService.GetRegionsAsync();
            RegionCombo.ItemsSource = regions;
            System.Diagnostics.Debug.WriteLine($"Загружено регионов: {regions.Count}");

            // Если у учреждения уже есть район, загружаем соответствующие районы
            if (Institution.DistrictId > 0)
            {
                System.Diagnostics.Debug.WriteLine($"У учреждения есть DistrictId: {Institution.DistrictId}");

                var district = await _dataService.GetDistrictAsync(Institution.DistrictId);
                if (district != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Найден район: {district.Name}, RegionId: {district.RegionId}");

                    // Устанавливаем выбранный регион
                    RegionCombo.SelectedValue = district.RegionId;

                    // Загружаем районы для этого региона
                    await LoadDistrictsForRegion(district.RegionId);

                    // Устанавливаем выбранный район
                    DistrictCombo.SelectedValue = Institution.DistrictId;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Район не найден в базе данных");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("У учреждения нет DistrictId");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки данных: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async void RegionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isLoading) return;

        if (RegionCombo.SelectedValue is int regionId)
        {
            System.Diagnostics.Debug.WriteLine($"Выбран регион с ID: {regionId}");
            await LoadDistrictsForRegion(regionId);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Регион не выбран");
            DistrictsClear();
        }
    }

    private async Task LoadDistrictsForRegion(int regionId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Загрузка районов для региона {regionId}...");

            var districts = await _dataService.GetDistrictsByRegionAsync(regionId);
            DistrictCombo.ItemsSource = districts;

            System.Diagnostics.Debug.WriteLine($"Загружено районов: {districts.Count}");

            // Включаем или выключаем комбобокс районов в зависимости от наличия данных
            DistrictCombo.IsEnabled = districts.Count > 0;

            if (districts.Count > 0)
            {
                DistrictCombo.PlaceholderText = "Выберите район";
                System.Diagnostics.Debug.WriteLine("Комбобокс районов активирован");
            }
            else
            {
                DistrictCombo.PlaceholderText = "Нет доступных районов";
                System.Diagnostics.Debug.WriteLine("Нет районов для выбранного региона");
            }

            // Сбрасываем выбор района при смене региона
            Institution.DistrictId = 0;
            DistrictCombo.SelectedValue = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки районов: {ex.Message}");
            DistrictsClear();
        }
    }

    private void DistrictsClear()
    {
        DistrictCombo.ItemsSource = null;
        DistrictCombo.IsEnabled = false;
        DistrictCombo.PlaceholderText = "Сначала выберите область";
        Institution.DistrictId = 0;
    }

    private void DistrictCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DistrictCombo.SelectedValue is int districtId)
        {
            Institution.DistrictId = districtId;
            System.Diagnostics.Debug.WriteLine($"Выбран район с ID: {districtId}");
        }
        else
        {
            Institution.DistrictId = 0;
            System.Diagnostics.Debug.WriteLine("Район не выбран");
        }
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Валидация - проверяем обязательные поля
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Institution.Name))
            errors.Add("Название учреждения");

        if (string.IsNullOrWhiteSpace(Institution.Type))
            errors.Add("Тип учреждения");

        if (string.IsNullOrWhiteSpace(Institution.Address))
            errors.Add("Адрес");

        if (Institution.DistrictId == 0)
            errors.Add("Район");

        if (errors.Any())
        {
            args.Cancel = true;

            var errorMessage = "Пожалуйста, заполните следующие обязательные поля:\n• " +
                             string.Join("\n• ", errors);

            // Показываем ошибку валидации
            _ = ShowValidationErrorAsync(errorMessage);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Все поля заполнены корректно, можно сохранять");
        }
    }

    private async Task ShowValidationErrorAsync(string message)
    {
        var errorDialog = new ContentDialog
        {
            Title = "Не все поля заполнены",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };

        await errorDialog.ShowAsync();
    }
}