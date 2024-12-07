using api.Filters;
using api.Request;
using api.TransferModels;
using infrastructure.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service;

namespace library.Controllers;

public class EmployeeController : ControllerBase
{
    private readonly ILogger<EmployeeController> _logger;
    private readonly EmployeeService _employeeService;

    public EmployeeController(ILogger<EmployeeController> logger, EmployeeService employeeService)
    {
        _logger = logger;
        _employeeService = employeeService;
    }

    // Create Employee
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("/api/employee")]
    public ResponseDto Post([FromBody] Employee dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;


        try
        {
            _employeeService.CreateEmployee(dto.name, dto.position, dto.man_hours, dto.hired_date, dto.email, dto.phone, dto.avatar);
            return new ResponseDto()
            {
                MessageToClient = "Successfully add new Employee",
                ResponseData = "Created new Employee successfully!"
            };
        }
        catch (Exception ex) // Catch other general exceptions
        {
            return new ResponseDto()
            {
                MessageToClient = "Error",
                ResponseData = ex.Message
            };
        }
    }

    //Get List Employees
    //With 3 optional parameters: hiredDateFrom, hiredDateTo and sumManHours
    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("/api/employees")]
    public ResponseDto GetListEmployees(
        [FromQuery] string? hiredDate = null,
        [FromQuery] int? sumManHoursFrom = 0,
        [FromQuery] int? sumManHoursTo = 0
        )
    {
        HttpContext.Response.StatusCode = 200;

        try{
            var employees = _employeeService.GetListEmployees(hiredDate, sumManHoursFrom, sumManHoursTo);
            System.Console.WriteLine(employees.Count());
            return new ResponseDto()
                {
                    MessageToClient = "Successfully fetched",
                    ResponseData = employees
                };
        }
         catch (Exception ex) // Catch other general exceptions
        {
            return new ResponseDto()
            {

                MessageToClient = "Error",
                ResponseData = ex.Message
            };
        }
    }

    //Delete a Employee
    [Authorize(Roles = "Admin")]
    [HttpDelete]
    [Route("/api/employee/{id}")]
    public ResponseDto DeleteEmployee([FromRoute] Guid id)
    {
        HttpContext.Response.StatusCode = 200;

        try
        {
            _employeeService.DeleteEmployee(id);
            return new ResponseDto()
            {
                MessageToClient = "Successfully deleted",
                ResponseData = "Deleted Employee successfully!"
            };
        }
        catch (Exception ex) // Catch other general exceptions
        {
            return new ResponseDto()
            {
                MessageToClient = "Error",
                ResponseData = ex.Message
            };
        }
    }
}