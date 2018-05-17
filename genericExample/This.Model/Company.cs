
namespace This.Model
{
    public class Company : Base<Company>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }

        public Company() : this(string.Empty, new Address())
        {

        }

        public Company(string name, Address address)
        {
            Name = name;
            Address = address;

            FileSystem.Company = this;
        }
    }
}
