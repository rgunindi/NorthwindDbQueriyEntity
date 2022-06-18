using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace Fibonacci_one.Repository.Models
{
    [Keyless]
    public partial class EmployeeTerritory
    {
        public int EmployeeId { get; set; }
        public string TerritoryId { get; set; }
        public virtual Employee Employe { get; set; }
        public virtual Territory Territory { get; set; }
        [ForeignKey("EmployeeId")]
        //[InverseProperty(nameof(Employee.EmployeeTerritories))]
        [NotMapped]
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
