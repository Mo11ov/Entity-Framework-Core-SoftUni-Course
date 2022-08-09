namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
	{
		//Done
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			StringBuilder sb = new StringBuilder();
			var validGames = new List<Game>();
			
			var dtoGames = JsonConvert.DeserializeObject<GamesInputModel[]>(jsonString);

			var genres = new List<Genre>();
			var developers = new List<Developer>();
			var tags = new List<Tag>();

            foreach (var dtoGame in dtoGames)
            {
                if (!IsValid(dtoGame) || !dtoGame.Tags.Any())
                {
					sb.AppendLine("Invalid Data");
					continue;
                }

				DateTime validDate;
				bool isDateValid = DateTime.TryParseExact(
					dtoGame.ReleaseDate,
					"yyyy-MM-dd",
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out validDate);
                
				if (!isDateValid)
                {
					sb.AppendLine("Invalid Data");
					continue;
				}

				Game game = new Game
				{
					Name = dtoGame.Name,
					Price = dtoGame.Price,
					ReleaseDate = validDate,
				};

                Genre genre = genres.FirstOrDefault(x => x.Name == dtoGame.Genre);

                if (genre == null)
                {
                    Genre newGenre = new Genre { Name = dtoGame.Genre };
                    genres.Add(newGenre);

                    game.Genre = newGenre;
                }
                else
                {
                    game.Genre = genre;
                }

                Developer developer = developers.FirstOrDefault(x => x.Name == dtoGame.Developer);

                if (developer == null)
                {
                    Developer newDevoper = new Developer { Name = dtoGame.Developer };
                    developers.Add(newDevoper);

                    game.Developer = newDevoper;
                }
                else
                {
                    game.Developer = developer;
                }

                foreach (var tagName in dtoGame.Tags)
                {
                    if (string.IsNullOrEmpty(tagName))
                    {
						sb.AppendLine("Invalid Data");
						continue;
					}

					Tag tag = tags.FirstOrDefault(x => x.Name == tagName);
                    
					if (tag == null)
                    {
						Tag newTag = new Tag { Name = tagName };
						tags.Add(newTag);
						
						GameTag gameTag = new GameTag { Tag = newTag };
						game.GameTags.Add(gameTag);
                    }
                    else
                    {
						GameTag gameTag = new GameTag { Tag = tag };
						game.GameTags.Add(gameTag);
					}
                }

                if (game.GameTags.Count == 0)
                {
					sb.AppendLine("Invalid Data");
					continue;
				}

				validGames.Add(game);
				sb.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags");
            }

			context.Games.AddRange(validGames);
			context.SaveChanges();

			return sb.ToString().TrimEnd();
		}
		//Done
		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			StringBuilder sb = new StringBuilder();
			var validUsers = new List<User>();
			var dtoUsers = JsonConvert.DeserializeObject<UsersImportModel[]>(jsonString);

            foreach (var dtoUser in dtoUsers)
            {
				var validCards = new List<Card>();

				if (!IsValid(dtoUser) || !dtoUser.Cards.Any())
                {
					sb.AppendLine("Invalid Data");
					continue;
                }

                foreach (var dtoCard in dtoUser.Cards)
                {
					CardType cardType;
					bool isValidCard = Enum.TryParse<CardType>(dtoCard.Type, out cardType);

                    if (!isValidCard)
                    {
						sb.AppendLine("Invalid Data");
						continue;
					}

					Card card = new Card
					{
						Number = dtoCard.Number,
						Cvc = dtoCard.CVC,
						Type = cardType
					};

					validCards.Add(card);
                }

				User user = new User
				{
					FullName = dtoUser.FullName,
					Username = dtoUser.Username,
					Email = dtoUser.Email,
					Age = dtoUser.Age,
					Cards = validCards
				};

				validUsers.Add(user);
				sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
            }

			context.Users.AddRange(validUsers);
			context.SaveChanges();

			return sb.ToString().TrimEnd();
		}
		//Done
		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			StringBuilder sb = new StringBuilder();
			var validPurchases = new List<Purchase>();

			var dtoPurchases = XmlConverter.Deserializer<PurchaseImportModel>(xmlString, "Purchases");

            foreach (var dtoPurchase in dtoPurchases)
            {
                if (!IsValid(dtoPurchase))
                {
					sb.AppendLine("Invalid Data");
					continue;
                }

				DateTime validDate;
				bool isDateValid = DateTime.TryParseExact(dtoPurchase.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out validDate);

                if (!isDateValid)
                {
					sb.AppendLine("Invalid Data");
					continue;
				}

				Purchase purchase = new Purchase 
				{
					Game = context.Games.FirstOrDefault(x => x.Name == dtoPurchase.Title),
					Type = Enum.Parse<PurchaseType>(dtoPurchase.Type),
					ProductKey = dtoPurchase.Key,
					Card = context.Cards.FirstOrDefault(x => x.Number == dtoPurchase.Card),
					Date = validDate
				};

				validPurchases.Add(purchase);
				sb.AppendLine($"Imported {purchase.Game.Name} for {purchase.Card.User.Username}");
            }

			context.AddRange(validPurchases);
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