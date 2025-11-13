using System;

namespace EducationInstitutionsRB.Models;

public class Institution
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Contacts { get; set; } = string.Empty;
    public int DistrictId { get; set; }
    public District? District { get; set; }
    public string Status { get; set; } = "Активно";
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public int StudentCount { get; set; }
    public int AdmittedCount { get; set; }
    public int ExpelledCount { get; set; }
    public int StaffCount { get; set; }

    // Вычисляемые свойства для отображения
    public string StudentCountDisplay => $"Учеников: {StudentCount}";
    public string StaffCountDisplay => $"Персонал: {StaffCount}";
    public string AdmittedCountDisplay => $"Принято: {AdmittedCount}";
    public string ExpelledCountDisplay => $"Отчислено: {ExpelledCount}";
    public string RegistrationDateDisplay => RegistrationDate.ToString("dd.MM.yyyy");

    public override string ToString() => Name;
}