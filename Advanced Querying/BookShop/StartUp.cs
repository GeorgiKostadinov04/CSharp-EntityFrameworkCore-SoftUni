namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            Console.WriteLine(CountCopiesByAuthor(db));
        }



        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            if (!Enum.TryParse<AgeRestriction>(command, true, out var ageRestriction))
            {
                return $"{command} is not a valid restriction";
            }

            var books = context.Books.Where(r => r.AgeRestriction == ageRestriction)
                .Select(b => new
                {
                    b.Title
                })
                .OrderBy(b => b.Title).ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books.Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .Select(b => new
                {
                    b.BookId,
                    b.Title
                })
                .OrderBy(b => b.BookId).ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }


        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books.Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Price,
                    b.Title
                }).OrderByDescending(b => b.Price).ToList();

            var sb = new StringBuilder();

            foreach (var item in books)
            {
                sb.AppendLine($"{item.Title} - ${item.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books.Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .Select(b => new
                {
                    b.ReleaseDate, b.Title
                })
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            var books = context.Books
                .Where(b => b.BookCategories.Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .Select(b => new
                {
                    b.Title,
                    b.BookCategories
                })
                .OrderBy(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));

        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var books = context.Books.
                Where(b => b.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .Select(b => new
                {
                    b.EditionType,
                    b.Title,
                    b.Price,
                    b.ReleaseDate
                }).OrderByDescending(b => b.ReleaseDate).ToList();

            return string.Join(Environment.NewLine, books.Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}"));
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(n => n.FirstName.EndsWith(input))
                .Select(n => new
                {
                    FullName = n.FirstName + " " + n.LastName
                })
                .OrderBy(n => n.FullName).ToList();

            return string.Join(Environment.NewLine, authors.Select(n => n.FullName));
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books.Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => new
                {
                    b.Title
                }).OrderBy(b => b.Title).ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books.Include(x=> x.Author).Where(a=>a.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(x => new
                {
                    x.Title,
                    AuthorFirstName = x.Author.FirstName,
                    AuthorLastName = x.Author.LastName,
                    x.BookId
                }).OrderBy(x=> x.BookId).ToList();

            return string.Join(Environment.NewLine, books.Select(x => $"{x.Title} ({x.AuthorFirstName} {x.AuthorLastName})"));
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books.Where(b => b.Title.Length > lengthCheck);

            return books.Count();
             
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new
                {
                    AuhtorName = string.Join(" ", a.FirstName, a.LastName),
                    TotalBooks = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.TotalBooks).ToList();

            return string.Join(Environment.NewLine,
                authors.Select(a => $"{a.AuhtorName} - {a.TotalBooks}"));
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var profitByCategory = context.Categories
                .Select(c=> new
                {
                    CategoryName =c.Name,
                    TotalProfit = c.CategoryBooks.Sum(cb=> cb.Book.Copies * cb.Book.Price)
                    
                }).OrderByDescending(x=>x.TotalProfit)
                .ThenBy(c=> c.CategoryName)
                .ToList();

            foreach(var c in profitByCategory)
            {
                Console.WriteLine(c.CategoryName);
            }

            return string.Join(Environment.NewLine, profitByCategory.Select(pc => $"{pc.CategoryName} ${pc.TotalProfit}"));
        }


        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories.Select(c => new
            {
                CatName = c.Name,
                MostRecentBooks = c.CategoryBooks.OrderByDescending(b=>b.Book.ReleaseDate)
                .Take(3)
                .Select(b => new
                {
                    BookTitle = b.Book.Title,
                    b.Book.ReleaseDate!.Value.Year
                })
            }).OrderBy(c=>c.CatName).ToList();

            var result = new StringBuilder();
            foreach( var c in categories)
            {
                result.AppendLine($"--{c.CatName}");
                foreach(var mostRecentBook in c.MostRecentBooks)
                {
                    result.AppendLine($"{mostRecentBook.BookTitle} ({mostRecentBook.Year})");
                }
            }

            return result.ToString().TrimEnd();
        }


        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books.Where(b => b.ReleaseDate!.Value.Year < 2010);

            foreach( var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            context.ChangeTracker.Clear();

            var books = context.Books.Where(b => b.Copies < 4200);

            context.RemoveRange(books);

            return context.SaveChanges();
        }
    }
}


