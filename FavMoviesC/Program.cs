using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Dapper;

namespace FavMoviesC
{
    internal class Program
    {

        static void Main()
        {
            Console.Clear();
            Console.WriteLine("If you have an account, press 1, otherwise press 2.");
            int j = Convert.ToInt32(Console.ReadLine());


            if (j == 1)
            {
                Console.Write("Username: ");
                string username = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();

                using (var connection = Connect())
                {
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @username AND UserPassword = @password";
                    int count = connection.QuerySingleOrDefault<int>(query, new { username, password });

                    if (count > 0)
                    {
                        Console.WriteLine("Login successful");
                        Entry();
                    }
                    else
                    {
                        Console.WriteLine("Incorrect username or password");
                        Main();
                    }
                }

            }
            else if(j== 2)
            {
                Console.WriteLine("Welcome to User Registration System!");
                Console.Write("Enter your Usurname: ");
                string username = Console.ReadLine();
                Console.Write("Enter your Password: ");
                string password = Console.ReadLine();
                Console.Write("Enter your First Name: ");
                string firstname = Console.ReadLine();
                Console.Write("Enter your Lastname: ");
                string lastname = Console.ReadLine();
                Console.Write("Enter your e-mail: ");
                string email = Console.ReadLine();

                var user = new Users
                {
                    UserName = username,
                    UserPassword = password,
                    FirstName = firstname,
                    LastName = lastname,
                    Email = email

                };

                
                var rowsAffected = Connect().Execute("INSERT INTO Users (UserName, UserPassword, FirstName, LastName, Email) VALUES (@userName, @userpassword, @firstname, @lastname, @email)", user);

                if (rowsAffected > 0)
                {
                    Console.WriteLine("User registration successful!");
                }
                else
                {
                    Console.WriteLine("User registration failed.");
                    
                }

            }


        }
        public static void Entry()
        {
            Console.Clear();
            Console.WriteLine("1-List Users\n2-List Movies\n3-Watched Movies\n4-Rate The Movie\n5-Add Movie\n6-Search Movie");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    ListUsers();
                    ShowMainMenu();
                    
                    break;
                case 2:
                    ListMovies();
                    ShowMainMenu();

                    break;
                case 3:
                    WatchedMovies();
                    ShowMainMenu();

                    break;
                case 4:
                    RateTheMovie();
                    ShowMainMenu();
                    break;
                case 5:
                    SaveFakeMoviesToDatabase();
                    ShowMainMenu();
                    break;
                case 6:
                    SearchMovie();
                    ShowMainMenu();
                    break;
                default:
                    Console.WriteLine("Invalid choice, try again");
                    ShowMainMenu();

                    break;
            }

        }

        public static void ListUsers()
        {
            var Users = Connect().Query<Users>("select UserName, FirstName,LastName, Email from Users").ToList();
            foreach (var item in Users)
            {
                Console.WriteLine($"{item.FirstName} {item.LastName}");
                Console.WriteLine($"{item.UserName} {item.Email}");
                Console.WriteLine("***********************");
            }


        }
        public static void ListMovies()
        {
            var Movies = Connect().Query<Movies>($"select * from Movies").ToList();
            foreach (var item in Movies)
            {
                Console.WriteLine($" Id: {item.Id} : Name: {item.MovieName} Year: {item.ProductionYear}");
                Console.WriteLine($"Genre: {item.Genre} Director: {item.Director}");
                Console.WriteLine("***********************");
            }
        }

        public static void WatchedMovies()
        {
            Console.WriteLine("Please enter the User ID:");
            string userId = Console.ReadLine();
            var watchedMovies = Connect().Query<UsersMovie>("select MovieName from Movies m inner join UsersMovie u on m.Id=u.MovieId Where u.UserId=@userId", new { userId });
            foreach (var item in watchedMovies)
            {
                Console.WriteLine($"{item.MovieName}");
            }
        }
        public static void RateTheMovie()
        {
            Console.WriteLine("Choice the Movie and Rate it.");
            ListMovies();
            Console.Write("Enter your User Id:");
            int userId = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter Movie Id: ");
            int movieId = Convert.ToInt32(Console.ReadLine());
            Console.Write("Please enter your movie rating: ");
            string ratingString = Console.ReadLine();
            if (decimal.TryParse(ratingString, out decimal rating))
            {
                var usersmovie = new UsersMovie
                {
                    UserId = userId,
                    MovieId = movieId,
                    Rating = rating,
                };

                try
                {
                    var affectedRows = Connect().Execute("insert into UsersMovie (UserId,MovieId,Rating, WatchDate) values (@userId , @movieId, @rating, GETDATE())", usersmovie);
                    if (affectedRows > 0)
                    {
                        Console.WriteLine($"Thank you for rating the movie {rating}.");
                    }
                    else
                    {
                        Console.WriteLine("Something went wrong. Please try again later.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while saving the rating: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid rating.");
            }



        }
        public static List<Movies> GetFakeMovies()
        {
            var faker = new Faker();
            var movies = new List<Movies>();
            for (int i =20; i < 60; i++)
            {
                var movie = new Movies()
                {
                   
                    MovieName = faker.Commerce.ProductName(),
                    Director = faker.Name.FullName(),
                    Actors = faker.Name.FullName(),
                    ProductionYear = faker.Random.Int(1900, DateTime.Now.Year),
                    Genre = faker.Random.Word(),
                    ImdbRating = faker.Random.Decimal(1, 10)
                };
                movies.Add(movie);

            }
           
           


            return movies;
        }
        public static void SaveFakeMoviesToDatabase()
        {
            var movies = GetFakeMovies();

            using (var connection = Connect())
            {
                foreach (var movie in movies)
                {
                    var result = connection.Execute(
                        "INSERT INTO Movies (MovieName, Director, Actors, ProductionYear, Genre, ImdbRating) VALUES (@MovieName, @Director, @Actors, @ProductionYear, @Genre, @ImdbRating)",
                        movie);
                }
            }
            ListMovies();


        }
        public static void SearchMovie()
        {
            Console.Write("Please enter the name of the movie:");
            string searchitem = Console.ReadLine();
            var SearchMovie = Connect().Query<Movies>("SELECT * FROM Movies WHERE MovieName LIKE '%' + @searchitem + '%'", new { searchitem }).ToList();
            foreach (var movie in SearchMovie)
            {
                Console.WriteLine(movie.MovieName);
                Console.WriteLine(movie.Director);
                Console.WriteLine(movie.ImdbRating);
            }
        }

        public static SqlConnection Connect()
        {
            return new SqlConnection("server=DESKTOP-TSB8VGO;Initial Catalog=FavMovie;Integrated Security=True");
        }
        public  static void ShowMainMenu()
        {
            Console.WriteLine("To proceed to the main menu, kindly press the number 1.");
            int choice = Convert.ToInt32(Console.ReadLine());
            if (choice == 1)
            {
                Console.Clear();
                Entry();
            }
          
            
        }


    }
}




