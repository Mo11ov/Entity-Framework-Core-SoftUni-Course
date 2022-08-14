namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        //Done
        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            List<Project> validProjects = new List<Project>();

            var dtoProjects = XmlConverter.Deserializer<ProjectsImportModel>(xmlString, "Projects");

            foreach (var dtoProject in dtoProjects)
            {
                List<Task> tasks = new List<Task>();

                DateTime validOpenDate;
                bool isOpenDateValid = DateTime.TryParseExact
                    (dtoProject.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validOpenDate);

                if (!IsValid(dtoProject) || !isOpenDateValid)
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                Project project = new Project
                {
                    Name = dtoProject.Name,
                    OpenDate = validOpenDate,
                    DueDate = DateTime.TryParseExact(dtoProject.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validDueDate) ? (DateTime?) validDueDate : null
                };
                

                foreach (var dtoTask in dtoProject.Tasks)
                {
                    bool isTaskOpenDateValid = DateTime.TryParseExact
                        (dtoTask.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime taskOpenDate);

                    bool isTaskDueDateValid = DateTime.TryParseExact
                        (dtoTask.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime taskDueDate);

                    if (!IsValid(dtoTask) || 
                        taskOpenDate < project.OpenDate || 
                        taskDueDate > project.DueDate || 
                        !isTaskDueDateValid || 
                        !isTaskOpenDateValid)
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    Task task = new Task
                    {
                        Name = dtoTask.Name,
                        OpenDate = taskOpenDate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)dtoTask.ExecutionType,
                        LabelType = (LabelType)dtoTask.LabelType
                    };

                    tasks.Add(task);
                }

                project.Tasks = tasks;
                validProjects.Add(project);

                sb.AppendLine($"Successfully imported project - {project.Name} with {project.Tasks.Count} tasks.");
            }

            context.Projects.AddRange(validProjects);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            List<Employee> validEmployees = new List<Employee>();

            List<int> tasksInDataBase = context.Tasks.Select(x => x.Id).ToList();

            var dtoEmplyees = JsonConvert.DeserializeObject<EmployeeImportModel[]>(jsonString);

            foreach (var dtoEmployee in dtoEmplyees)
            {
                List<EmployeeTask> validEmployeeTasks = new List<EmployeeTask>();

                if (!IsValid(dtoEmployee))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                Employee currentEmployee = new Employee
                {
                    Username = dtoEmployee.Username,
                    Email = dtoEmployee.Email,
                    Phone = dtoEmployee.Phone
                };

                foreach (var dtoEmployeeTaskId in dtoEmployee.Tasks.Distinct())
                {
                    if (!tasksInDataBase.Contains(dtoEmployeeTaskId))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    EmployeeTask currenEmployeeTask = new EmployeeTask
                    {
                        Employee = currentEmployee,
                        TaskId = dtoEmployeeTaskId
                    };

                    validEmployeeTasks.Add(currenEmployeeTask);
                }

                currentEmployee.EmployeesTasks = validEmployeeTasks;

                validEmployees.Add(currentEmployee);

                sb.AppendLine($"Successfully imported employee - {currentEmployee.Username} with {currentEmployee.EmployeesTasks.Count} tasks.");
            }

            context.Employees.AddRange(validEmployees);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}