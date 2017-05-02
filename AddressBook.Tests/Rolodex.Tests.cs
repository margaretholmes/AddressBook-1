using NSubstitute;
using NUnit.Framework;

namespace AddressBook.Tests
{
    [TestFixture]
    public class RolodexTests
    {
        private IGetInputFromUsers _input;
        private IHandleContacts _contacts;
        private IHandleRecipes _recipes;
        private Rolodex _rolodex;

        [SetUp]    //runs before each test runs
        public void BeforeEachTest()
        {
            //Substitute dynamically creates a class in the background that
            //implements the interface
            _input = Substitute.For<IGetInputFromUsers>();
            _contacts = Substitute.For<IHandleContacts>();
            _recipes = Substitute.For<IHandleRecipes>();
            _rolodex = new Rolodex(_contacts, _recipes, _input);
        }

        [Test]
        public void ExitJustDoesNothing()
        {
            //Arrange
            _input.GetNumber().Returns(0);

            //Act
            _rolodex.DoStuff();

            //Assert
            _input.Received().GetNumber();
            _contacts.DidNotReceive().GetAllContacts();
            _recipes.DidNotReceive().GetAllRecipes();
            _contacts.DidNotReceiveWithAnyArgs().CreateCompany(null, null);
        }
        //[Test, Ignore("Broken due to hard drive size")]
        [Test]
        public void AddPersonAsExpected()
        {
            _input.GetNumber().Returns(1, 0);
            _input.GetNonEmptyString().Returns("Bob", "Marley", "555-555-1212");
            
            //Act
            _rolodex.DoStuff();

            //Assert
            _input.Received(2).GetNumber();
            _contacts.DidNotReceive().GetAllContacts();
            _recipes.DidNotReceive().GetAllRecipes();
            _contacts.DidNotReceiveWithAnyArgs().CreateCompany(null, null);
            _contacts.Received().CreatePerson("Bob", "Marley", "555-555-1212");

        }

        [Test]
        public void AddRecipeAsExpected()
        {
            _input.GetNumber().Returns(6, 0, 0);
            _input.GetNonEmptyString().Returns("Sweet Potato Fries");

            //Act
            _rolodex.DoStuff();

            //Assert
            _input.Received(3).GetNumber();
            _contacts.DidNotReceive().GetAllContacts();
            _recipes.DidNotReceive().GetAllRecipes();
            _contacts.DidNotReceiveWithAnyArgs().CreateCompany(null, null);
            _contacts.Received().CreatePerson(null, null, null);
            _recipes.Received().Create("Sweet Potato Fries", RecipeType.Appetizers);

        }
    }
}
