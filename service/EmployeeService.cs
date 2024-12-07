using System;
using System.Collections.Generic;
using infrastructure.DataModels;
using infrastructure.Repositories;

namespace service;

public class EmployeeService
{
    private readonly EmployeeRepository _employeeRepository;

    public EmployeeService(EmployeeRepository employeeRepository)
    {
       _employeeRepository = employeeRepository;
    }
       

    public void CreateEmployee(string name, string position, int man_hours, DateTime hired_date, string email, string phone, string avatar)
    {
        try
        {
            _employeeRepository.CreateEmployee(name, position, man_hours, hired_date,email, phone,avatar);
        }
        catch (Exception ex) // Catch other general exceptions
        {
            // Handle other errors such as general exceptions or unexpected errors
            throw new Exception(ex.Message);
        }
    }

    public IEnumerable<EmployeeResponse> GetListEmployees(string? hiredDate, int? sumManHoursFrom = 0, int? sumManHoursTo = 0)
    {
        try{
           return _employeeRepository.GetListEmployees(hiredDate,sumManHoursFrom, sumManHoursTo);
        }
         catch (Exception ex) // Catch other general exceptions
        {
            // Handle other errors such as general exceptions or unexpected errors
            throw new Exception(ex.Message);
        }
    }

    public void DeleteEmployee(Guid id){
        try{
            _employeeRepository.DeleteEmployee(id);
        }
        catch (Exception ex) // Catch other general exceptions
        {
            // Handle other errors such as general exceptions or unexpected errors
            throw new Exception(ex.Message);
        }
    }
}