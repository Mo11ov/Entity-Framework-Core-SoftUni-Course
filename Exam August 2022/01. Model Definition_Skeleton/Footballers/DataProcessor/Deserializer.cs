namespace Footballers.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";
        //Done
        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var validCoaches = new List<Coach>();
            
            var dtoCoaches = XmlConverter.Deserializer<ImportCoachesInputModel>(xmlString, "Coaches");

            foreach (var dtoCoach in dtoCoaches)
            {
                var validFootballers = new List<Footballer>();

                if (!IsValid(dtoCoach))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                foreach (var x in dtoCoach.Footballers)
                {
                    if (!IsValid(x))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    DateTime contractStartDate = DateTime.ParseExact(x.ContractStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime contractEndDate = DateTime.ParseExact(x.ContractEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    if (contractStartDate >= contractEndDate)
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    Footballer currFootballer = new Footballer
                    {
                        Name = x.Name,
                        ContractStartDate = contractStartDate,
                        ContractEndDate = contractEndDate,
                        PositionType = (PositionType) x.PositionType,
                        BestSkillType = (BestSkillType) x.BestSkillType
                    };

                    validFootballers.Add(currFootballer);
                }

                Coach coach = new Coach
                {
                    Name = dtoCoach.Name,
                    Nationality = dtoCoach.Nationality,
                    Footballers = validFootballers
                };

                validCoaches.Add(coach);
                sb.AppendLine($"Successfully imported coach - {coach.Name} with {coach.Footballers.Count} footballers.");

            }

            context.Coaches.AddRange(validCoaches);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
        
        //Done
        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var validTeams = new List<Team>();

            var dtoTeams = JsonConvert.DeserializeObject<TeamsInputModel[]>(jsonString);

            var footBallersIds = context.Footballers
                .Select(x => x.Id).ToArray();

            foreach (var dtoTeam in dtoTeams)
            {
                if (!IsValid(dtoTeam) || dtoTeam.Trophies <= 0)
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var team = new Team
                {
                    Name = dtoTeam.Name,
                    Nationality = dtoTeam.Nationality,
                    Trophies = dtoTeam.Trophies,
                };


                foreach (var footBallerId in dtoTeam.Footballers.Distinct())
                {
                    if (!footBallersIds.Contains(footBallerId))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }


                    TeamFootballer validTeamFootbaler = new TeamFootballer
                    {
                        FootballerId = footBallerId,
                        Team = team
                    };

                    team.TeamsFootballers.Add(validTeamFootbaler);
                }

               

                validTeams.Add(team);
                sb.AppendLine($"Successfully imported team - {team.Name} with {team.TeamsFootballers.Count} footballers.");
            }

            context.Teams.AddRange(validTeams);
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
