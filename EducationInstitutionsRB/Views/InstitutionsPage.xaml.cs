using EducationInstitutionsRB.Models;
using EducationInstitutionsRB.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducationInstitutionsRB.Views;

public sealed partial class InstitutionsPage : Page
{
    private readonly IDataService _dataService;
    private readonly DialogService _dialogService;
    private List<Institution> _allInstitutions = new();
    private List<Region> _regions = new();
    private List<District> _districts = new();
    private bool _isDialogOpen = false;

    public InstitutionsPage()
    {
        this.InitializeComponent();
        _dataService = App.GetService<IDataService>();
        _dialogService = App.GetService<DialogService>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _allInstitutions = await _dataService.GetInstitutionsAsync();
            _regions = await _dataService.GetRegionsAsync();

            InstitutionsList.ItemsSource = _allInstitutions;
            RegionCombo.ItemsSource = _regions;

            System.Diagnostics.Debug.WriteLine($"Загружено учреждений: {_allInstitutions.Count}");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync($"Ошибка загрузки данных: {ex.Message}", this.Content.XamlRoot);
        }
    }

    private async void AddInstitutionButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isDialogOpen) return;
        _isDialogOpen = true;

        try
        {
            var newInstitution = new Institution
            {
                RegistrationDate = DateTime.Now,
                Status = "Активно"
            };

            var dialog = new InstitutionDialog(newInstitution, "Добавить учреждение");
            dialog.XamlRoot = this.Content.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    await _dataService.AddInstitutionAsync(newInstitution);
                    await LoadDataAsync();
                    await _dialogService.ShowSuccessAsync("Учреждение успешно добавлено!", this.Content.XamlRoot);
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync($"Ошибка при добавлении: {ex.Message}", this.Content.XamlRoot);
                }
            }
        }
        finally
        {
            _isDialogOpen = false;
        }
    }

    private async void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isDialogOpen) return;

        if (sender is Button button && button.Tag is Institution selectedInstitution)
        {
            await EditInstitutionAsync(selectedInstitution);
        }
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isDialogOpen) return;

        if (sender is Button button && button.Tag is Institution selectedInstitution)
        {
            await DeleteInstitutionAsync(selectedInstitution);
        }
    }

    private async Task EditInstitutionAsync(Institution selectedInstitution)
    {
        _isDialogOpen = true;

        try
        {
            var institutionToEdit = new Institution
            {
                Id = selectedInstitution.Id,
                Name = selectedInstitution.Name,
                Type = selectedInstitution.Type,
                Address = selectedInstitution.Address,
                Contacts = selectedInstitution.Contacts,
                DistrictId = selectedInstitution.DistrictId,
                District = selectedInstitution.District,
                Status = selectedInstitution.Status,
                RegistrationDate = selectedInstitution.RegistrationDate,
                StudentCount = selectedInstitution.StudentCount,
                AdmittedCount = selectedInstitution.AdmittedCount,
                ExpelledCount = selectedInstitution.ExpelledCount,
                StaffCount = selectedInstitution.StaffCount
            };

            var dialog = new InstitutionDialog(institutionToEdit, "Редактировать учреждение");
            dialog.XamlRoot = this.Content.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    await _dataService.UpdateInstitutionAsync(institutionToEdit);
                    await LoadDataAsync();
                    await _dialogService.ShowSuccessAsync("Учреждение успешно обновлено!", this.Content.XamlRoot);
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync($"Ошибка при обновлении: {ex.Message}", this.Content.XamlRoot);
                }
            }
        }
        finally
        {
            _isDialogOpen = false;
        }
    }

    private async Task DeleteInstitutionAsync(Institution selectedInstitution)
    {
        var result = await _dialogService.ShowConfirmationAsync(
            "Подтверждение удаления",
            $"Вы уверены, что хотите удалить учреждение \"{selectedInstitution.Name}\"?",
            this.Content.XamlRoot
        );

        if (result == ContentDialogResult.Primary)
        {
            try
            {
                await _dataService.DeleteInstitutionAsync(selectedInstitution.Id);
                await LoadDataAsync();
                await _dialogService.ShowSuccessAsync("Учреждение успешно удалено!", this.Content.XamlRoot);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync($"Ошибка при удалении: {ex.Message}", this.Content.XamlRoot);
            }
        }
    }

    private async void RegionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RegionCombo.SelectedItem is Region selectedRegion)
        {
            try
            {
                _districts = await _dataService.GetDistrictsByRegionAsync(selectedRegion.Id);
                DistrictCombo.ItemsSource = _districts;
                DistrictCombo.IsEnabled = true;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync($"Ошибка загрузки районов: {ex.Message}", this.Content.XamlRoot);
            }
        }
        else
        {
            DistrictCombo.ItemsSource = null;
            DistrictCombo.IsEnabled = false;
        }
        await FilterInstitutionsAsync();
    }

    private async void DistrictCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await FilterInstitutionsAsync();
    }

    private async void TypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await FilterInstitutionsAsync();
    }

    private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        await FilterInstitutionsAsync();
    }

    private async Task FilterInstitutionsAsync()
    {
        try
        {
            var searchText = SearchTextBox.Text;
            var regionId = (RegionCombo.SelectedItem as Region)?.Id;
            var districtId = (DistrictCombo.SelectedItem as District)?.Id;
            var type = TypeCombo.SelectedItem as string;
            var status = "Активно";

            var filtered = await _dataService.SearchInstitutionsAsync(
                searchText, regionId, districtId, type, status);

            InstitutionsList.ItemsSource = filtered;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка фильтрации: {ex.Message}");
        }
    }
}