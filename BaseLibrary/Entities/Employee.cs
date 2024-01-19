namespace BaseLibrary.Entities
{
    public class Employee : BaseEntitity
    {
        public string? CivilId { get; set; }
        public string? FileNumber { get; set; }
        public string? FullName { get; set; }
        public string? JobName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Photo { get; set; }
        public string? Orther { get; set; }

        public GeneralDepartment? GeneralDepartment { get; set; }
        public Guid GeneralDepartmentId { get; set; }
        public Department? Department { get; set; }
        public Guid DepartmentId { get; set; }
        public Branch? Branch { get; set; }
        public Guid BranchId { get; set; }
        public Town? Town { get; set; }
        public Guid TownId { get; set; }

    }
}
