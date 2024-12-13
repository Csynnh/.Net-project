namespace infrastructure.DataModels;

public class Employee
{
    public string name{ get; set; }  // Primary Key, Foreign Key, liên kết tới bảng variant_product
    public string position { get; set; } 
    public int man_hours {get;set;}
    public DateTime hired_date {get;set;}
    public string email {get;set;}
    public string phone {get;set;}
    public string avatar {get;set;}
}

public class EmployeeResponse:Employee
{
    public Guid id { get; set; }
   
}

public class EmployeeList
{
   
    public List<EmployeeResponse> employeeList{get; set; }
    public int totalCount { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng Hóa đơn

}