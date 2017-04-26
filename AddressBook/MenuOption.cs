using System;


namespace AddressBook
{
    public enum MenuOption              //when C# compiles, these all turn into numbers
    {
        Exit,
        AddPerson,   //AddPerson = 200,  This will set it's number to 200, then next one will default to 201
        AddCompany,
        ListContacts,
        SearchContacts,
        RemoveContact,
        AddRecipe,
        ListRecipes,
        SearchEverything,

        UPPER_LIMIT
        
    }
}
