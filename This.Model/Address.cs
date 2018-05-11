using System;
using System.Collections.Generic;
using System.Text;

namespace This.Model
{
    public class Address : Base<Address>
    {
        public string Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public Address() : this(string.Empty, string.Empty, string.Empty, string.Empty)
        {

        }

        public Address(string street, string city, string state, string zip)
        {
            Street = street;
            City = city;
            State = state;
            Zip = zip;
        }
    }
}
