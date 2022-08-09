namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class Deserializer
    {
        //Done
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var departments = new List<Department>();

            var dtoDepartmentCells = JsonConvert.DeserializeObject<DepartmentCellsInputModel[]>(jsonString);

            foreach (var dtoDepartment in dtoDepartmentCells)
            {
                if (!IsValid(dtoDepartment) || dtoDepartment.Cells.Count == 0 || !dtoDepartment.Cells.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Department department = new Department 
                {
                    Name = dtoDepartment.Name,
                    Cells = dtoDepartment.Cells.Select(x => new Cell 
                    { 
                        CellNumber = x.CellNumber,
                        HasWindow = x.HasWindow
                    })
                    .ToList()
                };

                departments.Add(department);

                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }

            context.Departments.AddRange(departments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
        //Done
        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var prisoners = new List<Prisoner>();

            var dtoPirsonersMails = JsonConvert.DeserializeObject<PrisonersMailsInputModel[]>(jsonString);

            foreach (var dtoPrisoner in dtoPirsonersMails)
            {
                if (!IsValid(dtoPrisoner) || !dtoPrisoner.Mails.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var incarcerationDate = DateTime.ParseExact(dtoPrisoner.IncarcerationDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture);

                var isReleasedDateValid = DateTime.TryParseExact(dtoPrisoner.ReleaseDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime releasedDate);

                var prisoner = new Prisoner
                {
                    FullName = dtoPrisoner.FullName,
                    Nickname = dtoPrisoner.Nickname,
                    Age = dtoPrisoner.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = isReleasedDateValid ? (DateTime?)releasedDate : null,
                    Bail = dtoPrisoner.Bail,
                    CellId = dtoPrisoner.CellId,
                    Mails = dtoPrisoner.Mails.Select(x => new Mail 
                    { 
                        Description = x.Description,
                        Sender = x.Sender,
                        Address = x.Address
                    }).ToArray()
                };

                prisoners.Add(prisoner);
                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var validOfficers = new List<Officer>();

            var dtoOfficerPrisoners = XmlConverter.Deserializer<OfficerPrisonerInputModel>(xmlString, "Officers");

            foreach (var dtoOfficer in dtoOfficerPrisoners)
            {
                if (!IsValid(dtoOfficer))
                {
                    sb.AppendLine($"Invalid Data");
                    continue;
                }

                var officer = new Officer
                {
                    FullName = dtoOfficer.Name,
                    Salary = dtoOfficer.Money,
                    DepartmentId = dtoOfficer.DepartmentId,
                    Position = Enum.Parse<Position>(dtoOfficer.Position),
                    Weapon = Enum.Parse<Weapon>(dtoOfficer.Weapon),
                    OfficerPrisoners = dtoOfficer.Prisoners.Select(x => new OfficerPrisoner 
                    {
                        PrisonerId = x.Id
                    })
                    .ToList()
                };

                validOfficers.Add(officer);
                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }

            context.Officers.AddRange(validOfficers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}