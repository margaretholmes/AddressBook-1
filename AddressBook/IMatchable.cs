using System;
 

namespace AddressBook
{
    public interface IMatchable
    {
        bool Matches(string term);          //must always be Public so don't need to type Public
    }                                       //must be publicly implemented on the class
}
