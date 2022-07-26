namespace BookShop
{
    using System;
    using System.Linq;
    using Data;
    using Initializer;
    using BookShop.Models.Enums;
    using System.Text;
    using System.Globalization;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //Task 02
            //string ageCmd = Console.ReadLine();
            //Console.WriteLine(GetBooksByAgeRestriction(db, ageCmd));

            //Task 03
            //Console.WriteLine(GetGoldenBooks(db));

            //Task 04
            //Console.WriteLine(GetBooksByPrice(db));

            //Task 05
            //int year = int.Parse(Console.ReadLine());
            //Console.WriteLine(GetBooksNotReleasedIn(db, year));

            //Task 06
            //string input = Console.ReadLine();
            //Console.WriteLine(GetBooksByCategory(db, input));

            //Taks 07
            //string date = Console.ReadLine();
            //Console.WriteLine(GetBooksReleasedBefore(db, date));

            //Task 08. Author Search
            //string input = Console.ReadLine();
            //Console.WriteLine(GetAuthorNamesEndingIn(db, input));

            //Task 09. Book Search
            //string input = Console.ReadLine();
            //Console.WriteLine(GetBookTitlesContaining(db, input));

            //Task 10
            //string input = Console.ReadLine();
            //Console.WriteLine(GetBooksByAuthor(db, input));

            //Task 11
            //int length = int.Parse(Console.ReadLine());
            //Console.WriteLine(CountBooks(db, length));

            //Task 12
            //Console.WriteLine(CountCopiesByAuthor(db));

            //Task 13
            //Console.WriteLine(GetTotalProfitByCategory(db));

            //Task 14
            //Console.WriteLine(GetMostRecentBooks(db));

            //Task 15 
            //IncreasePrices(db);

            //Task 16
            Console.WriteLine(RemoveBooks(db));

        }

        //Task 02. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            AgeRestriction ageRestriction;

            bool parsedEnum = Enum.TryParse(command, true, out ageRestriction);

            if (!parsedEnum)
            {
                return null;
            }

            string[] books = context.Books
                .Where(x => x.AgeRestriction == ageRestriction)
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToArray();

            return String.Join(Environment.NewLine, books);
        }


        //Task 03. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            string[] books = context.Books
                .Where(x => x.EditionType == EditionType.Gold && x.Copies < 5000)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToArray();

            return String.Join(Environment.NewLine, books);
        }


        //Task 04. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(x => x.Price > 40)
                .Select(x => new
                {
                    x.Title,
                    x.Price
                })
                .OrderByDescending(x => x.Price)
                .ToArray();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 05. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            string[] books = context.Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToArray();

            return String.Join(Environment.NewLine, books);
        }


        //Task 06. Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            string[] books = context.Books
                .Where(x => x.BookCategories.Any(x => categories.Contains(x.Category.Name.ToLower())))
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToArray();

            return String.Join(Environment.NewLine, books);   
        }


        //Task 07. Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(x => x.ReleaseDate.Value < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new
                {
                    x.Title,
                    x.EditionType,
                    x.Price
                })
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 08. Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context.Authors
                .Where(x => x.FirstName.EndsWith(input))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToArray();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.FirstName} {author.LastName}");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 09. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            string[] books = context.Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToArray();

            return String.Join(Environment.NewLine, books);
        }


        //Task 10. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(x => x.BookId)
                .Select(x => new
                {
                    x.Title,
                    AuthorsFirstName = x.Author.FirstName,
                    AuthorsLastName = x.Author.LastName
                })
                .ToArray();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.AuthorsFirstName} {book.AuthorsLastName})");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 11. Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books
                .Where(x => x.Title.Length > lengthCheck)
                .Count();
        }


        //Task 12. Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var authorsAndCopies = context.Authors
                .Select(x => new
                {
                    FullName = x.FirstName + " " + x.LastName,
                    Count = x.Books.Sum(x => x.Copies)
                })
                .OrderByDescending(x => x.Count)
                .ToArray();

            foreach (var author in authorsAndCopies)
            {
                sb.AppendLine($"{author.FullName} - {author.Count}");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 13. Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var profitByCategory = context.Categories
                .Select(x => new
                {
                    x.Name,
                    Profit = x.CategoryBooks.Sum(x => x.Book.Copies * x.Book.Price)
                })
                .OrderByDescending(x => x.Profit)
                .ToArray();

            foreach (var category in profitByCategory)
            {
                sb.AppendLine($"{category.Name} ${category.Profit:f2}");
            }

            return sb.ToString().TrimEnd();
        }


        //Task 14. Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categoryBooks = context.Categories
                .Select(x => new
                {
                    Category = x.Name,
                    Top3Books = x.CategoryBooks
                    .Select(x => new
                    {
                        x.Book
                    })
                    .OrderByDescending(x => x.Book.ReleaseDate)
                    .Take(3)
                    .ToArray()
                })
                .OrderBy(x => x.Category)
                .ToArray();

            foreach (var category in categoryBooks)
            {
                sb.AppendLine($"--{category.Category}");
                
                foreach (var book in category.Top3Books)
                {
                    sb.AppendLine($"{book.Book.Title} ({book.Book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }


        //Task 15. Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .ToArray();

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }


        //Task 16. Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var booksToRemove = context.Books
                .Where(x => x.Copies < 4200)
                .ToArray();

            int count = booksToRemove.Count();

            foreach (var book in booksToRemove)
            {
                context.Remove(book);
            }

            context.SaveChanges();

            return count;
        }
    }
}
