using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {

            SoftUniContext context = new SoftUniContext();
            Console.WriteLine(GetDepartmentsWithMoreThan5Employees(context));
        }


        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                }).Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName);

            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName} - {e.Salary:f2}"));

            return result;
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e=> new
                {
                    e.FirstName,
                    e.LastName,
                    e.Department.Name,
                    e.Salary
                })
                .OrderBy(e =>e.Salary).ThenByDescending(e =>e.FirstName).ToList();

            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName} {e.LastName} from {e.Name} - ${e.Salary:f2}"));
            return result;
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address =  new Address();

            address.AddressText = "Vitoshka 15";

            address.TownId = 4;

            var employee = context.Employees
                .FirstOrDefault(e => e.LastName == "Nakov");

            employee.Address = address;

            context.SaveChanges();

            var employees = context.Employees.Select(e=> new
            {
                e.AddressId,
                e.Address.AddressText
            })
            .OrderByDescending(e => e.AddressId)
            .Take(10)
            .ToList();

            string result = string.Join(Environment.NewLine, employees.Select(e => $"{e.AddressText}"));

            return result;
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Take(10).
                Include(e => e.Manager).
                Include(e => e.EmployeesProjects)
                .ToArray();

            for(int i = 0; i < 10; i++)
            {
                var employee = employees[i];

                sb.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.Manager.FirstName} {employee.Manager.LastName}");

                if(employee.EmployeesProjects.Count > 0)
                {
                    var projects = context.EmployeesProjects.Select(ep => new
                    {
                        ep.EmployeeId,
                        ep.Project.Name,
                        ep.Project.StartDate,
                        ep.Project.EndDate

                    })
                        .Where(ep => ep.EmployeeId == employee.EmployeeId)
                        .ToArray();

                    foreach( var p in projects)
                    {
                        if(p.StartDate.Year >= 2001 && p.StartDate.Year <= 2003)
                        {
                            var endDate = p.EndDate != null ? p.EndDate?.ToString("M/d/yyyy h:mm:ss tt") : "not finished";

                            sb.AppendLine($"--{p.Name} - {p.StartDate.ToString("M/d/yyyy h:mm:ss tt")} - {endDate}");
                        }
                    }
                }

            }

            return sb.ToString().Trim();

                            
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .Include(x => x.Employees)
                .Include(x => x.Town)
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town.Name)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .ToList();

            var result = new StringBuilder();

            foreach (var a in addresses)
            {
                result.AppendLine($"{a.AddressText}, {a.Town.Name} - {a.Employees.Count} employees");
            }

            return result.ToString().Trim();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employees = context.Employees
                .Include(ep => ep.EmployeesProjects);


            var employee = employees.FirstOrDefault(e => e.EmployeeId == 147);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            var projects = context.EmployeesProjects
                .Select(ep => new
                {
                    ep.EmployeeId,
                    ProjectName = ep.Project.Name
                })
                .Where(ep => ep.EmployeeId == employee.EmployeeId)
                .OrderBy(ep => ep.ProjectName)
                .ToList();

            foreach ( var project in projects)
            {
                sb.AppendLine(project.ProjectName);
            }
                
            return sb.ToString().Trim();
        }


        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments.Include(x=>x.Employees)
                .Where(d=> d.Employees.Count > 5).OrderBy(d=>d.Employees.Count).ThenBy(d=>d.Name)
                .ToList();


            var result = new StringBuilder();

            foreach (var d in departments)
            {
                result.AppendLine($"{d.Name} - {d.Manager.FirstName} {d.Manager.LastName}");

                var employees = d.Employees
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToList();

                foreach (var e in employees)
                {
                    result.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return result.ToString().Trim();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .ToList()
                .OrderBy(p => p.Name);

            var result = new StringBuilder();

            foreach (var p in projects)
            {
                result.AppendLine(p.Name);
                result.AppendLine(p.Description);
                result.AppendLine(p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture));
            }

            return result.ToString().Trim();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {

            var departments = new List<string>() { "Engineering", "Tool Design", "Marketing", "Information Services" };

            var employees = context.Employees
                .Where(e => departments.Any(d => d == e.Department.Name));

            foreach (var employee in employees)
            {
                employee.Salary *= 1.12M;
            }

            context.SaveChanges();

            var result = new StringBuilder();

            foreach (var e in employees.ToList())
            {
                result.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }

            return result.ToString().Trim();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees.Where(e => e.FirstName.ToLower().StartsWith("sa")).OrderBy(e => e.FirstName).ThenBy(e => e.LastName);

            var result = new StringBuilder();

            foreach (var employee in employees)
            {
                result.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:f2})");
            }

            return result.ToString().Trim();
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects
                .Find(2);

            context.EmployeesProjects.RemoveRange(context.EmployeesProjects.Where(ep => ep.Project == project));
            context.Projects.Remove(project);

            context.SaveChanges();

            var result = new StringBuilder();

            var projects = context.Projects
                .Take(10)
                .ToList();

            foreach (var p in projects)
            {
                result.AppendLine(p.Name);
            }

            return result.ToString().Trim();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var town = context.Towns
                .Include(t => t.Addresses)
                .FirstOrDefault(t => t.Name == "Seattle");

            context.Addresses.RemoveRange(town.Addresses);
            context.Towns.Remove(town);

            var employees = context.Employees
                .Where(e => e.Address.Town.Name == "Seattle");

            foreach (var e in employees)
            {
                e.AddressId = null;
            }

            context.SaveChanges();

            return $"{town.Addresses.Count} addresses in Seattle were deleted";

        }
    }
}