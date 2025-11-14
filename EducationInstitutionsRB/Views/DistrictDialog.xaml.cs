using EducationInstitutionsRB.Models;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;

namespace EducationInstitutionsRB.Views;

public sealed partial class DistrictDialog : ContentDialog
{
    public District District { get; set; }
    public string Title { get; set; }
    public List<Region> Regions { get; set; }

    public DistrictDialog(District district, string title, List<Region> regions)
    {
        this.InitializeComponent();
        District = district;
        Title = title;
        Regions = regions;
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Валидация
        if (string.IsNullOrWhiteSpace(District.Name))
        {
            args.Cancel = true;
            return;
        }

        if (District.RegionId == 0)
        {
            args.Cancel = true;
            return;
        }
    }
}