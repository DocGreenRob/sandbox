namespace This.Model
{
    // Mock File System object 
    // Created in attempts to get:
    // NUnit.Framework.Assert.AreEqual(customer, savedCustomer); and
    // NUnit.Framework.Assert.AreEqual(company, savedCompany);
    // tests to pass.
    public static class FileSystem
    {
        public static Customer Customer { get; set; }
        public static Company Company { get; set; }
        public static Address Address { get; set; }
    }
}
