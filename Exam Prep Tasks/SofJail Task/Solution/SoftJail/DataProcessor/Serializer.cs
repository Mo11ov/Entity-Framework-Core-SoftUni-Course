namespace SoftJail.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Linq;

    public class Serializer
    {
        //Done
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                .Where(x => ids.Contains(x.Id))
                .ToList()
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.FullName,
                    CellNumber = x.Cell.CellNumber,
                    Officers = x.PrisonerOfficers.Select(p => new
                    {
                        OfficerName = p.Officer.FullName,
                        Department = p.Officer.Department.Name
                    })
                    .ToList()
                    .OrderBy(x => x.OfficerName),
                    TotalOfficerSalary = Math.Round(x.PrisonerOfficers.Sum(o => o.Officer.Salary), 2)
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList();

            var result = JsonConvert.SerializeObject(prisoners, Formatting.Indented);

            return result;
        }

        //Done
        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var names = prisonersNames.Split(',', StringSplitOptions.RemoveEmptyEntries);

            var prisoners = context.Prisoners
                .Where(x => names.Contains(x.FullName))
                .ToList()
                .Select(x => new PrisonerViewModel
                {
                    Id = x.Id,
                    Name = x.FullName,
                    IncarcerationDate = x.IncarcerationDate.ToString("yyyy-MM-dd"),
                    EncryptedMessages = x.Mails.Select(m => new EncryptedMessagesViewModel
                    {
                        Description = string.Join("", m.Description.Reverse())
                    }).ToArray()
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToArray();

            var result = XmlConverter.Serialize(prisoners, "Prisoners");

            return result;
        }
    }
}