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

    public async Task<EmployeeList> GetListEmployees(int pageNumber, int pageSize, string? hiredDate, int? sumManHoursFrom = 0,  int? sumManHoursTo = 0){
        var sql = $@" SELECT *
         FROM dev.employee
        ";
        var sqlCount = $@" SELECT COUNT(*)
         FROM dev.employee
        ";
        var conditions = @"";
        if (hiredDate != null )
        {
            conditions += $" WHERE hired_date = '{hiredDate}'";  
        }
        if (sumManHoursTo != 0)
        {
            if (hiredDate == null)
            { 
                conditions += " WHERE";
            }
            else
            {
                conditions += " AND";
            }
            conditions += $" man_hours BETWEEN '{sumManHoursFrom}' AND '{sumManHoursTo}'";

        }
        sql += conditions;
        sqlCount += conditions;
        sql+=$"OFFSET {(pageNumber-1)*pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY;";

        // var sqlForCount= $@"SELECT COUNT(*) FROM dev.employee";
        
        try
        {
            using var conn = _dataSource.OpenConnection();
            var employeeList = (await conn.QueryAsync<EmployeeResponse>(sql)).ToList();
            var totalCount = await conn.QuerySingleAsync<int>(sqlCount);
            return new EmployeeList
            {
                employeeList = employeeList,
                totalCount = totalCount
            };
        
          
        }
        catch (Exception ex) // Catch other general exceptions
        {
            // Handle other errors such as general exceptions or unexpected errors
            throw new Exception(ex.Message);
        }
    }

    public EmployeeResponse GetEmployeeById(Guid id){
        var sql = $@"
        SELECT * FROM dev.employee
        WHERE id = @id
        ";

        try
        {
            using (var conn = _dataSource.OpenConnection()) // Open connection to the database
            {
                return conn.QueryFirstOrDefault<EmployeeResponse>(sql, new { id });
            }
        }
        catch (Exception ex) // Catch other general exceptions
        {
            // Handle other errors such as general exceptions or unexpected errors
            throw new Exception(ex.Message);
        }
    }

    public void UpdateEmployee(Guid id, string? name, string? position, int? man_hours, DateTime? hired_date, string? email, string? phone){
        var setClause = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(name))
        {
            setClause.Add("name = @name");
            parameters.Add("name", name);
        }

        if (!string.IsNullOrEmpty(position))
        {
            setClause.Add("position = @position");
            parameters.Add("position", position);
        }

        if (man_hours.HasValue)
        {
            setClause.Add("man_hours = @man_hours");
            parameters.Add("man_hours", man_hours.Value);
        }

        if (hired_date.HasValue)
        {
            setClause.Add("hired_date = @hired_date");
            parameters.Add("hired_date", hired_date.Value);
        }

        if (!string.IsNullOrEmpty(email))
        {
            setClause.Add("email = @email");
            parameters.Add("email", email);
        }

        if (!string.IsNullOrEmpty(phone))
        {
            setClause.Add("phone = @phone");
            parameters.Add("phone", phone);
        }

        if (setClause.Count == 0)
        {
            throw new ArgumentException("At least one parameter must be provided to update.");
        }

        var sql = $@"
            UPDATE dev.employee
            SET {string.Join(", ", setClause)}
            WHERE id = @id;
        ";

        // Adding the 'id' parameter
        parameters.Add("id", id);

        try{
            using (var conn = _dataSource.OpenConnection()) // Open connection to the database
            {
                conn.Execute(sql, new { id, name, position, man_hours, hired_date,email, phone});
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