namespace This.Model
{
    public partial class Customer : Base<Customer>
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }

        public Customer() : this(string.Empty, string.Empty, new Address())
        {

        }
        public Customer(string firstName, string lastName, Address address)
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
        }

    }
}
