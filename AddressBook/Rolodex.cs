using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace AddressBook
{
    public class Rolodex
    {
        public Rolodex(string connectionString)
        {
            _connectionString = connectionString;
            _contacts = new List<Contact>();
            _recipes = new Dictionary<RecipeType, List<Recipe>>();

            _recipes.Add(RecipeType.Appetizers, new List<Recipe>());
            _recipes[RecipeType.Entrees] = new List<Recipe>();
            _recipes[RecipeType.Desserts] = new List<Recipe>();
        }

        public void DoStuff()
        {
            // Print a menu
            ShowMenu();
            // Get the user's choice
            MenuOption choice = GetMenuOption();

            // while the user does not want to exit
            while (choice != MenuOption.Exit)
            {
                // figure out what they want to do
                // get information
                // do stuff
                switch (choice)
                {
                    case MenuOption.AddPerson:
                        DoAddPerson();
                        break;
                    case MenuOption.AddCompany:
                        DoAddCompany();
                        break;
                    case MenuOption.ListContacts:
                        DoListContacts();
                        break;
                    case MenuOption.SearchContacts:
                        DoSearchContacts();
                        break;
                    case MenuOption.RemoveContact:
                        DoRemoveContact();
                        break;
                    case MenuOption.AddRecipe:
                        DoAddRecipe();
                        break;
                    case MenuOption.SearchEverything:
                        DoSearchEverything();
                        break;
                    case MenuOption.ListRecipes:
                        ;
                        DoListRecipes();
                        break;

                }
                ShowMenu();
                choice = GetMenuOption();
            }
        }

        private void DoListRecipes()
        {
            Console.Clear();
            Console.WriteLine("RECIPES!");
            Console.WriteLine("From database:");


            //string connectionString;
            //connectionString = "Server=localhost;Database=AddressBook;Trusted_Connection=True;";

            //SqlConnection connection;
            //connection = new SqlConnection(connectionString);

            //the using statement takes the place of try and finally loop
            using (SqlConnection connection = new SqlConnection(_connectionString))

            //try
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;

                command = connection.CreateCommand();
                command.CommandText = "SELECT RecipeType, RecipeName FROM Recipe ORDER BY RecipeType, RecipeName";
                SqlDataReader reader = command.ExecuteReader();




                while (reader.Read())
                {
                    //int rowId = reader.GetInt32(0);
                    string type = reader.GetString(0);
                    string name = reader.GetString(1);
                    Console.WriteLine($"{type}, {name}");
                    


                }

                Console.WriteLine();
                //Console.ReadLine();
            }
            //finally
            //{
            //    connection.Dispose();
            //}

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = "Recipes.Txt";
            string fullPath = Path.Combine(desktopPath, fileName);

            Console.WriteLine("From file: ");
            using (StreamReader reader = File.OpenText(fullPath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split('|');  //splits them apart
                    Console.WriteLine(string.Join("\t", parts));  //puts back together with slashes
                    
                }
                Console.WriteLine();
                Console.WriteLine("Press Enter to continue.");
                Console.ReadLine();

            }



        }

        private void DoSearchEverything()
        {
            Console.Clear();
            Console.WriteLine("SEARCH EVERYTHING!");
            Console.Write("Please enter a search term: ");
            string term = GetNonEmptyStringFromUser();

            List<IMatchable> matchables = new List<IMatchable>();
            matchables.AddRange(_contacts);
            matchables.AddRange(_recipes[RecipeType.Appetizers]);
            matchables.AddRange(_recipes[RecipeType.Entrees]);
            matchables.AddRange(_recipes[RecipeType.Desserts]);


            foreach (IMatchable matcher in matchables)
            {
                if (matcher.Matches(term))
                {
                    Console.WriteLine($"> {matcher}");

                }

            }
            Console.ReadLine();
        }

        private void DoAddRecipe()
        {
            //string connectionString;
            //connectionString = "Server=localhost;Database=AddressBook;Trusted_Connection=True;";

            //SqlConnection connection;
            //connection = new SqlConnection(connectionString);

            using (SqlConnection connection = new SqlConnection(_connectionString))

            //try
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;

                Console.Clear();

                Console.WriteLine("What kind of recipe is this?");
                for (int i = 0; i < (int)RecipeType.UPPER_LIMIT; i++)
                {
                    Console.WriteLine($"{i}. {(RecipeType)i}");
                }
                RecipeType choice = (RecipeType)int.Parse(Console.ReadLine());
                string specificRecipes = choice.ToString();

                Console.WriteLine("Please enter the title of your recipe.");
                string name = GetNonEmptyStringFromUser();



                command = connection.CreateCommand();
                command.CommandText = @"
                    insert into Recipe(RecipeType, RecipeName)
                    values(@RecipeType, @RecipeName);
                    ";
                command.Parameters.AddWithValue("@RecipeType", specificRecipes);
                command.Parameters.AddWithValue("@RecipeName", name);

                command.ExecuteNonQuery();

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = "Recipes.Txt";
                string fullPath = Path.Combine(desktopPath, fileName);

                using (StreamWriter writer = File.AppendText(fullPath))
                {
                    writer.WriteLine(string.Join("|", "Recipe:  ", specificRecipes, name));
                }




            }
            //finally
            //{
            //    connection.Dispose();
            //}


        }





        private void DoRemoveContact()
        {
            Console.Clear();
            Console.WriteLine("REMOVE A CONTACT!");
            Console.Write("Search for a contact: ");
            string term = GetNonEmptyStringFromUser();

            foreach (Contact contact in _contacts)
            {
                if (contact.Matches(term))
                {
                    Console.Write($"Remove {contact}? (y/N)");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        _contacts.Remove(contact);
                        return;
                    }
                }
            }

            Console.WriteLine("No more contacts found.");
            Console.WriteLine("Press Enter to return to the menu...");
            Console.ReadLine();
        }

        private void DoSearchContacts()
        {
            Console.Clear();
            Console.WriteLine("SEARCH!");
            Console.Write("Please enter a search term: ");
            string term = GetNonEmptyStringFromUser();

            foreach (Contact contact in _contacts)
            {
                if (contact.Matches(term))
                {
                    Console.WriteLine($"> {contact}");
                }
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private void DoListContacts()
        {
            Console.Clear();
            Console.WriteLine("YOUR CONTACTS");



            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = "Contacts.Txt";
            string fullPath = Path.Combine(desktopPath, fileName);

            using (StreamReader reader = File.OpenText(fullPath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split('|');  //splits them apart
                    Console.WriteLine(string.Join("\t", parts));  //puts back together with slashes
                }
            }


            //foreach (Contact contact in _contacts)
            //{
            //    Console.WriteLine($"> {contact}");
            //}

            Console.WriteLine();
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();


        }

        private void DoAddCompany()
        {
            Console.Clear();
            Console.WriteLine("Please enter information about the company.");
            Console.Write("Company name: ");
            string name = Console.ReadLine();

            Console.Write("Phone number: ");
            string phoneNumber = GetNonEmptyStringFromUser();

            _contacts.Add(new Company(name, phoneNumber));


            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = "Contacts.Txt";
            string fullPath = Path.Combine(desktopPath, fileName);
            //using (StreamWriter writer = File.CreateText(fullPath))

            //{
            //    writer.WriteLine("Company Name|Company Phone Number");
            //}

            using (StreamWriter writer = File.AppendText(fullPath))
            {
                writer.WriteLine(string.Join("|", "Company:  ", name, phoneNumber));
            }
        }


    

    private void DoAddPerson()
    {
        Console.Clear();
        Console.WriteLine("Please enter information about the person.");
        Console.Write("First name: ");
        string firstName = Console.ReadLine();

        Console.Write("Last name: ");
        string lastName = GetNonEmptyStringFromUser();

        Console.Write("Phone number: ");
        string phoneNumber = GetNonEmptyStringFromUser();

        _contacts.Add(new Person(firstName, lastName, phoneNumber));

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = "Contacts.Txt";
            string fullPath = Path.Combine(desktopPath, fileName);
            //using (StreamWriter writer = File.CreateText(fullPath))

            //{ 
            //    writer.WriteLine("Company Name|Company Phone Number");
            //}

            using (StreamWriter writer = File.AppendText(fullPath))
            {
               writer.WriteLine(string.Join("|", "Person:  ", $"{firstName} {lastName}", phoneNumber));
                
            }
        }

    private string GetNonEmptyStringFromUser()
    {
        string input = Console.ReadLine();
        while (input.Length == 0)
        {
            Console.WriteLine("That is not valid.");
            input = Console.ReadLine();
        }
        return input;
    }


    private int GetNumberFromUser()
    {
        while (true)
        {
            try
            {
                string input = Console.ReadLine();
                return int.Parse(input);
            }
            catch (FormatException)
            {
                Console.WriteLine("You should type a number.");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("That was bad!  Do again!");
            }
            finally    //stuff in this block will always happen no matter if exception or not
            {
                Console.WriteLine("This will always print.");
            }
        }
    }
    private MenuOption GetMenuOption()
    {

        int choice = GetNumberFromUser();


        while (choice < 0 || choice >= (int)MenuOption.UPPER_LIMIT) //put the (int) because it's an enum
        {                                                          //not a variable, known at compile time
            Console.WriteLine("That is not valid.");
            choice = GetNumberFromUser();
        }

        return (MenuOption)choice;  //cast, turn the integer to a menu option, ex. 1 = AddPerson
    }

    private void ShowMenu()
    {
        Console.Clear();
        Console.WriteLine($"ROLODEX! ({_contacts.Count}) ({_recipes.Count})");
        Console.WriteLine("1. Add a person");
        Console.WriteLine("2. Add a company");
        Console.WriteLine("3. List all contacts");
        Console.WriteLine("4. Search contacts");
        Console.WriteLine("5. Remove a contact");
        Console.WriteLine("-------------------");
        Console.WriteLine("6. Add a recipe");
        Console.WriteLine("7. List recipes");
        Console.WriteLine("8. Search Everything!");
        Console.WriteLine();
        Console.WriteLine("0. Exit");
        Console.WriteLine();
        Console.Write("What would you like to do? ");
    }

    private readonly List<Contact> _contacts;  //prevents you from accidentally creating a new list
                                               //and naming it _contacts
    private Dictionary<RecipeType, List<Recipe>> _recipes;
    private readonly string _connectionString;  //read only variable, can not be updated
}


}
