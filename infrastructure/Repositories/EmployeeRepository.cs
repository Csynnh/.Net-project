using Dapper;
using infrastructure.DataModels;
using Npgsql;

namespace infrastructure.Repositories;

// public interface IEmployeeRepository
// {
//     void CreateEmployee(string name, string position, int man_hours, DateTime hired_date, string email, string phone, string avatar);
//     List<Employee> GetListEmployees(string? hiredDate, int? sumManHours = null);
// }

public class EmployeeRepository
{
    private NpgsqlDataSource _dataSource;

    public EmployeeRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public void CreateEmployee(string name, string position, int man_hours, DateTime hired_date, string email, string phone, string avatar)
    {
        var sql = $@"
        INSERT INTO dev.employee (name, position, man_hours, hired_date,email, phone,avatar)
        VALUES (@name, @position, @man_hours, @hired_date, @email, @phone, @avatar)
        ";

        try
        {
            using (var conn = _dataSource.OpenConnection()) // Open connection to the database
            {
                conn.Execute(sql, new { name, position, man_hours, hired_date, email, phone, avatar });
            }
        }
        catch (Exception ex) // Catch other general exceptions
        {
            // Handle other errors such as general exceptions or unexpected errors
            throw new Exception(ex.Message);
        }
    }

    public List<EmployeeResponse> GetListEmployees(string? hiredDate, int? sumManHoursFrom = 0,  int? sumManHoursTo = 0){
        var sql = $@"
        SELECT * FROM dev.employee
        ";
        if (hiredDate != null )
        {
            sql += $" WHERE hired_date = '{hiredDate}'";  
        }
        if (sumManHoursTo != 0)
        {
            if (hiredDate == null)
            { 
                sql += " WHERE";
            }
            else
            {
                sql += " AND";
            }
            sql += $" man_hours BETWEEN '{sumManHoursFrom}' AND '{sumManHoursTo}'";
        }

        try
        {
            using (var conn = _dataSource.OpenConnection()) // Open connection to the database
            {
                 return conn.Query<EmployeeResponse>(sql).ToList();
            }
        }
        catch (Exception ex) // Catch other general exceptions
        {
            // Handle other errors such as general exceptions or unexpected errors
            throw new Exception(ex.Message);
        }
    }

    public void DeleteEmployee(Guid id){
        var sql = $@"
        DELETE FROM dev.employee
        WHERE id = @id
        ";

        try
        {
            using (var conn = _dataSource.OpenConnection()) // Open connection to the database
            {
                conn.Execute(sql, new { id });
            }
        }
        catch (Exception ex) // Catch other general exceptions
        {
            // Handle other errors such as general exceptions or unexpected errors
            throw new Exception(ex.Message);
        }
    }
}