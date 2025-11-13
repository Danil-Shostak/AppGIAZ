using EducationInstitutionsRB.Models;
using EducationInstitutionsRB.Services;
using EducationInstitutionsRB.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EducationInstitutionsRB.Views;

public sealed partial class OverviewPage : Page
{
    private readonly IDataService _dataService;
    private OverviewViewModel _viewModel;

    public OverviewPage()
    {
        try
        {
            Debug.WriteLine("OverviewPage конструктор начат");
            this.InitializeComponent();
            _dataService = App.GetService<IDataService>();
            _viewModel = new OverviewViewModel(_dataService);
            Debug.WriteLine("OverviewPage создана успешно");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка в конструкторе OverviewPage: {ex.Message}");
            throw;
        }
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        try
        {
            Debug.WriteLine("OverviewPage OnNavigatedTo начат");
            base.OnNavigatedTo(e);
            await LoadDataAsync();
            Debug.WriteLine("OverviewPage данные загружены");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка в OnNavigatedTo: {ex.Message}");
        }
    }

    private async Task LoadDataAsync()
    {
        try
        {
            LoadingProgress.IsActive = true;
            RecentInstitutionsList.Visibility = Visibility.Collapsed;
            NoDataText.Visibility = Visibility.Collapsed;

            // Загружаем данные
            await _viewModel.LoadDataAsync();

            // Обновляем UI
            TotalInstitutionsText.Text = _viewModel.TotalInstitutionsDisplay;
            TotalStudentsText.Text = _viewModel.TotalStudentsDisplay;
            TotalStaffText.Text = _viewModel.TotalStaffDisplay;
            SuccessRateText.Text = _viewModel.SuccessRateDisplay;

            // Обновляем список недавних учреждений
            if (_viewModel.RecentInstitutions.Any())
            {
                RecentInstitutionsList.ItemsSource = _viewModel.RecentInstitutions;
                RecentInstitutionsList.Visibility = Visibility.Visible;
                NoDataText.Visibility = Visibility.Collapsed;
            }
            else
            {
                RecentInstitutionsList.Visibility = Visibility.Collapsed;
                NoDataText.Visibility = Visibility.Visible;
            }

            LoadingProgress.IsActive = false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка загрузки данных: {ex.Message}");
            LoadingProgress.IsActive = false;
        }
    }

    private async void AddInstitutionButton_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.AddInstitutionCommand.ExecuteAsync(null);
        // После добавления перезагружаем данные
        await LoadDataAsync();
    }

    private async void GenerateReportButton_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.GenerateReportCommand.ExecuteAsync(null);
    }
}