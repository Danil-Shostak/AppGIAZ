using EducationInstitutionsRB.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace EducationInstitutionsRB;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();

        // ”станавливаем Mica материал дл€ современного вида
        this.SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
    }

    private void RootNavigation_Loaded(object sender, RoutedEventArgs e)
    {
        // ”станавливаем начальную страницу с анимацией
        ContentFrame.Navigate(typeof(OverviewPage), null, new EntranceNavigationTransitionInfo());
        RootNavigation.SelectedItem = RootNavigation.MenuItems[0];
    }

    private void RootNavigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer is NavigationViewItem item)
        {
            switch (item.Tag.ToString())
            {
                case "Overview":
                    ContentFrame.Navigate(typeof(OverviewPage), null, new DrillInNavigationTransitionInfo());
                    break;
                case "Institutions":
                    ContentFrame.Navigate(typeof(InstitutionsPage), null, new DrillInNavigationTransitionInfo());
                    break;
                case "Import":
                    ContentFrame.Navigate(typeof(ImportPage), null, new DrillInNavigationTransitionInfo());
                    break;
                case "Reports":
                    ContentFrame.Navigate(typeof(ReportsPage), null, new DrillInNavigationTransitionInfo());
                    break;
                case "Admin":
                    ContentFrame.Navigate(typeof(AdminPage), null, new DrillInNavigationTransitionInfo());
                    break;
            }
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