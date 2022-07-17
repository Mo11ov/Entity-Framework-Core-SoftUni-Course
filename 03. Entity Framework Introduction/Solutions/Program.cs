namespace SoftUni
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SoftUni.Data;
    using SoftUni.Models;

    public class StartUp
    {
        static void Main(string[] args)
        {
            var softUniContext = new SoftUniContext();
            //Task 03
            //Console.WriteLine(GetEmployeesFullInformation(softUniContext));

            //Task04
            //Console.WriteLine(GetEmployeesWithSalaryOver50000(softUniContext));

            //Task 05. Employees from Research and Development
            //Console.WriteLine(GetEmployeesFromResearchAndDevelopment(softUniContext));

            //Task 06. Adding a New Address and Updating Employee 
            //Console.WriteLine(AddNewAddressToEmployee(softUniContext));

            //Task 07. Employees and Projects
            //Console.WriteLine(GetEmployeesInPeriod(softUniContext));

            //Task 08. Addresses by Town
            //Console.WriteLine(GetAddressesByTown(softUniContext));

            //Task 09. Employee 147
            //Console.WriteLine(GetEmployee147(softUniContext));

            //Task 10. Departments with More Than 5 Employees
            //Console.WriteLine(GetDepartmentsWithMoreThan5Employees(softUniContext));

            //Task 11. Find Latest 10 Projects
            //Console.WriteLine(GetLatestProjects(softUniContext));

            //Taks 12. Increase Salaries
            //Console.WriteLine(IncreaseSalaries(softUniContext));

            //Task 13. Find Employees by First Name Starting With Sa
            //Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(softUniContext));

            //Task 14.Delete Project by Id
            //Console.WriteLine(DeleteProjectById(softUniContext));

            //Task 15. Remove Town
            Console.WriteLine(RemoveTown(softUniContext));
        }


        //Task 03. Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context) 
        {
            StringBuilder sb = new StringBuilder();

            List<Employee> allEmployees = context.Employees.OrderBy(x => x.EmployeeId).ToList();

            foreach (var employee in allEmployees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }
            
            return sb.ToString().TrimEnd();
        }


        //Task 04. Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 05. Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DeparmentName = e.Department.Name,
                    e.Salary
                })
                .Where(e => e.DeparmentName == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.DeparmentName} - ${employee.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 06. Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Employee nakovEmployee = context.Employees.Where(e => e.LastName == "Nakov").FirstOrDefault();

            nakovEmployee.Address = new Address { AddressText = "Vitoshka 15", TownId = 4 };

            context.SaveChanges();

            var employees = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => new
                {
                    e.Address.AddressText
                })
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.AddressText}");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 07. Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.EmployeesProjects.Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    AllProjects = e.EmployeesProjects
                    .Select(ep => new
                    {
                        ProjectName = ep.Project.Name,
                        StartDate = ep.Project.StartDate,
                        EndDate = ep.Project.EndDate
                    })
                    .ToList()
                })
                .ToList();


            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

                foreach (var project in employee.AllProjects)
                {
                    string startDate = project.StartDate.ToString("M/d/yyyy h:mm:ss tt");
                    string endDate = project.EndDate.HasValue
                        ? project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt")
                        : "not finished";
                    sb.AppendLine($"--{project.ProjectName} - {startDate} - {endDate}");
                }
            }


            return sb.ToString().TrimEnd();
        }


        //Task 08. Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var adresses = context.Addresses
                .OrderByDescending(a => a.Employees.Count())
                .ThenBy(a => a.Town.Name)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .Select(a => new 
                {
                    a.AddressText,
                    townName = a.Town.Name,
                    employeeCount = a.Employees.Count()
                })
                .ToList();

            foreach (var adress in adresses)
            {
                sb.AppendLine($"{adress.AddressText}, {adress.townName} - {adress.employeeCount} employees");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 09. Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new 
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects
                    .Select(p => new 
                    {
                        projectName = p.Project.Name
                    })
                    .OrderBy(p => p.projectName)
                    .ToList()
                })
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

                foreach (var project in employee.Projects)
                {
                    sb.AppendLine($"{project.projectName}");
                }
            }

            return sb.ToString().TrimEnd();
        }


        //Task 10. Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departments = context.Departments
                .Where(d => d.Employees.Count() > 5)
                .OrderBy(d => d.Employees.Count())
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    allEmployees = d.Employees
                })
                .ToList();

            foreach (var department in departments)
            {
                sb.AppendLine($"{department.Name} - {department.ManagerFirstName} {department.ManagerLastName}");

                foreach (var employee in department.allEmployees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName))
                {
                    sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                }
            }
            
            return sb.ToString().TrimEnd();
        }


        //Task 11. Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var last10Projects = context.Projects.OrderByDescending(p => p.StartDate).Take(10).ToList();

            foreach (var project in last10Projects.OrderBy(p => p.Name))
            {
                sb.AppendLine($"{project.Name}");
                sb.AppendLine($"{project.Description}");
                sb.AppendLine($"{project.StartDate.ToString("M/d/yyyy h:mm:ss tt")}");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 12. Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            
            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering" || e.Department.Name == "Tool Design"
                || e.Department.Name == "Marketing" || e.Department.Name == "Information Services")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var employee in employees)
            {
                employee.Salary *= 1.12m;
            }

            context.SaveChanges();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 13. Find Employees by First Name Starting With Sa
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .ToList()
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName);

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 14.Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Project project = context.Projects.Find(2);

            var employeeProjects = context.EmployeesProjects
                .Where(ep => ep.Project == project)
                .ToList();

            foreach (var ep in employeeProjects)
            {
                context.EmployeesProjects.Remove(ep);
            }

            context.Projects.Remove(project);

            context.SaveChanges();

            var projects = context.Projects.Take(10).ToList();

            foreach (var proj in projects)
            {
                sb.AppendLine($"{proj.Name}");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 15. Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Town town = context.Towns
                .FirstOrDefault(t => t.Name == "Seattle");

            var adressIds = context.Addresses
                .Where(a => a.Town == town)
                .Select(a => a.AddressId)
                .ToList();

            int adressesCount = adressIds.Count();

            var employees = context.Employees
                .Where(x => x.AddressId.HasValue && adressIds.Contains(x.AddressId.Value))
                .ToList();

            foreach (var employee in employees)
            {
                employee.AddressId = null;
            }

            foreach (var adressID in adressIds)
            {
                Address addressToRemove = context.Addresses
                    .Where(a => a.AddressId == adressID).FirstOrDefault();
                context.Addresses.Remove(addressToRemove);
            }

            context.Towns.Remove(town);

            context.SaveChanges();

            sb.AppendLine($"{adressesCount} addresses in Seattle were deleted");

            return sb.ToString().TrimEnd();
        }
    }
}
