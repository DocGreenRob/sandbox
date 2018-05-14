using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using This.Model;
using Newtonsoft.Json;

namespace This.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [TestCase]
        public void Clean_Up_File_Nuke_It()
        {

            Utils.FileWriter.Delete();
            var x = Utils.FileWriter.Read();
            NUnit.Framework.Assert.IsTrue(x.Length == 0);
        }

        [TestMethod]
        [TestCase]
        public void TestMethod1()
        {
            var address = new Address("56 Main St", "Mesa", "AZ", "38574");
            var customer = new Customer("John", "Doe", address);
            var company = new Company("Google", address);


            var json = JsonConvert.SerializeObject(customer);
            var json2 = JsonConvert.SerializeObject(company);

            Utils.FileWriter.Delete();
            Utils.FileWriter.Write(json);
            Utils.FileWriter.Write($"{System.Environment.NewLine}{json2}");
            var x = Utils.FileWriter.Read();

            var _json = JsonConvert.DeserializeObject<Customer>(json);
            var _json2 = JsonConvert.DeserializeObject<Company>(json2);

            NUnit.Framework.Assert.IsTrue(x.Length > 0);
        }

        [TestMethod]
        [TestCase]
        public void ProgrammerTest()
        {
            Clean_Up_File_Nuke_It();

            var address = new Address("56 Main St", "Mesa", "AZ", "38574");
            var customer = new Customer("John", "Doe", address);
            var company = new Company("Google", address);

            NUnit.Framework.Assert.IsNullOrEmpty(customer.Id);
            customer.Save();
            NUnit.Framework.Assert.IsNotNullOrEmpty(customer.Id);

            NUnit.Framework.Assert.IsNullOrEmpty(company.Id);
            company.Save();
            NUnit.Framework.Assert.IsNotNullOrEmpty(company.Id);

            

            Customer savedCustomer = Customer.Find(customer.Id);
            NUnit.Framework.Assert.IsNotNull(savedCustomer);
            NUnit.Framework.Assert.AreSame(customer.Address, address);
            NUnit.Framework.Assert.AreEqual(savedCustomer.Address, address);
            NUnit.Framework.Assert.AreEqual(customer.Id, savedCustomer.Id);
            NUnit.Framework.Assert.AreEqual(customer.FirstName, savedCustomer.FirstName);
            NUnit.Framework.Assert.AreEqual(customer.LastName, savedCustomer.LastName);
            //NUnit.Framework.Assert.AreEqual(customer, savedCustomer);
            NUnit.Framework.Assert.AreNotSame(customer, savedCustomer);

            Company savedCompany = Company.Find(company.Id);
            NUnit.Framework.Assert.IsNotNull(savedCompany);
            NUnit.Framework.Assert.AreSame(company.Address, address);
            NUnit.Framework.Assert.AreEqual(savedCompany.Address, address);
            NUnit.Framework.Assert.AreEqual(company.Id, savedCompany.Id);
            NUnit.Framework.Assert.AreEqual(company.Name, savedCompany.Name);
            //NUnit.Framework.Assert.AreEqual(company, savedCompany);
            NUnit.Framework.Assert.AreNotSame(company, savedCompany);

            customer.Delete();
            NUnit.Framework.Assert.IsNullOrEmpty(customer.Id);
            NUnit.Framework.Assert.IsNull(Customer.Find(customer.Id));

            company.Delete();
            NUnit.Framework.Assert.IsNullOrEmpty(company.Id);
            NUnit.Framework.Assert.IsNull(Company.Find(company.Id));
        }
    }
}
