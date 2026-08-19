namespace it_department_site.ViewModels
{
    public class EmployeeCertificateViewModel
    {
        public string LinkUrl { get; set; } = string.Empty;
        public string PreviewUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public bool IsPdf { get; set; }
    }

    public class EmployeeInfoViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PatronymicName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public List<EmployeeCertificateViewModel> Certificates { get; set; } = new();
        public List<Discipline> Disciplines { get; set; } = new();
    }
}
