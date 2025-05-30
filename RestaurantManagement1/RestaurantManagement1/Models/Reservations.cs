//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RestaurantManagement1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Reservations
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Reservations()
        {
            this.Customers = new HashSet<Customers>();
            this.DepartedCustomers = new HashSet<DepartedCustomers>();
        }
    
        public int ReservationID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public int NumberOfPeople { get; set; }
        public System.DateTime ReservationTime { get; set; }
        public string ReservationStatus { get; set; }
        public int TableID { get; set; }
        public int EmployeeID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Customers> Customers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DepartedCustomers> DepartedCustomers { get; set; }
        public virtual Employees Employees { get; set; }
        public virtual Tables Tables { get; set; }
    }
}
