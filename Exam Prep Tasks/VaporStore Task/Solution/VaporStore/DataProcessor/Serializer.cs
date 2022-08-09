namespace VaporStore.DataProcessor
{
	using System;
    using System.Globalization;
    using System.Linq;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			var genres = context.Genres
				.Where(x => genreNames.Contains(x.Name))
				.ToList()
				.Select(x => new
				{
					Id = x.Id,
					Genre = x.Name,
					Games = x.Games
					.Where(x => x.Purchases.Any())
					.Select(g => new
					{
						Id = g.Id,
						Title = g.Name,
						Developer = g.Developer.Name,
						Tags = string.Join(", ", g.GameTags.Select(t => t.Tag.Name)),
						Players = g.Purchases.Count()
					})
					.OrderByDescending(x => x.Players)
					.ThenBy(x => x.Id)
					.ToArray(),
					TotalPlayers = x.Games.Sum(x => x.Purchases.Count())
				})
				.OrderByDescending(x => x.Games.Select(g => g.Players).Count())
				.ThenBy(x => x.Id)
				.ToList();

			var result = JsonConvert.SerializeObject(genres, Formatting.Indented);

			return result;
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{

			var users = context.Users
				.ToList()
				.Where(x => x.Cards.Any(c => c.Purchases.Any(p => p.Type.ToString() == storeType)))
				.Select(x => new UsersViewModel
				{
					Username = x.Username,
					TotalSpendMoney = x.Cards
					.Sum(s => s.Purchases.Where(p => p.Type.ToString() == storeType).Sum(s => s.Game.Price)),
					Purchases = x.Cards.SelectMany(c => c.Purchases)
					.Where(p => p.Type.ToString() == storeType)
					.Select(s => new PurchasViewModel
					{
						Card = s.Card.Number,
						Cvc = s.Card.Cvc,
						Date = s.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
						Game = new GameViewModel
						{
							Title = s.Game.Name,
							Genre = s.Game.Genre.Name,
							Price = s.Game.Price
						}
					})
					.OrderBy(x => DateTime.ParseExact(x.Date, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture))
					.ToArray()
					
				})
				.OrderByDescending(x => x.TotalSpendMoney)
				.ThenBy(x => x.Username)
				.ToList();

			var result = XmlConverter.Serialize(users, "Users");

			return result;
		}
	}
}