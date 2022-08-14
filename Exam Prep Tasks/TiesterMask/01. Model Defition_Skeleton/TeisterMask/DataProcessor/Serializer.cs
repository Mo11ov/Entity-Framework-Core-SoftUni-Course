namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projectWithTask = context.Projects
                 .Where(x => x.Tasks.Any())
                 .ToArray()
                 .Select(x => new ProjectViewModel
                 {
                     TasksCount = x.Tasks.Count,
                     ProjectName = x.Name,
                     HasEndDate = x.DueDate.HasValue ? "Yes" : "No",
                     Tasks = x.Tasks.Select(t => new TaskViewModel
                     {
                         Name = t.Name,
                         Label = t.LabelType.ToString()
                     })
                     .OrderBy(x => x.Name)
                     .ToList()
                 })
                 .OrderByDescending(x => x.TasksCount)
                 .ThenBy(x => x.ProjectName)
                 .ToArray();

            var result = XmlConverter.Serialize(projectWithTask, "Projects");

            return result;
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees
                .Where(x => x.EmployeesTasks.Any(t => t.Task.OpenDate >= date))
                .ToArray()
                .Select(x => new
                {
                    Username = x.Username,
                    Tasks = x.EmployeesTasks.Select(t => new
                    {
                        TaskName = t.Task.Name,
                        OpenDate = t.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = t.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = t.Task.LabelType.ToString(),
                        ExecutionType = t.Task.ExecutionType.ToString()
                    })
                    .ToArray()
                    .Where(x => DateTime.Parse(x.OpenDate) >= date)
                    .OrderByDescending(x => DateTime.Parse(x.DueDate))
                    .ThenBy(t => t.TaskName)
                })
                .OrderByDescending(x => x.Tasks.Count())
                .ThenBy(x => x.Username)
                .Take(10)
                .ToArray();

            var result = JsonConvert.SerializeObject(employees, Formatting.Indented);

            return result;
        }
    }
}