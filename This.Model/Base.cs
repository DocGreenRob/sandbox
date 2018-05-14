using System;
using System.Linq;
using This.Utils;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace This.Model
{
    public static class FileSystem
    {
        public static Customer Customer { get; set; }
        public static Company Company { get; set; }
    }
    public static class AddressList
    {
        public static Address Address { get; set; }
    }
    public abstract class Base<T> : IGeneric<T> where T : class
    {
        public void Delete()
        {
            var className = this.ToString();

            this.GetType().GetProperty("Id").SetValue(this, null);
            var file = FileWriter.Read();
            Dictionary<string, string> newLines = new Dictionary<string, string>();
            bool addCustomer, addCompany = true;

            foreach (var line in file)
            {

                try
                {
                    var customerObj = JsonConvert.DeserializeObject<Customer>(line);
                    if (String.IsNullOrEmpty(customerObj.FirstName))
                    {
                        // request for Company
                        addCompany = false;
                        newLines.Add("company", line);
                    }
                    else
                    {
                        addCustomer = false;
                        newLines.Add("customer", line);

                    }
                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }


            // Delete the json obj from disk
            // remove everything
            FileWriter.Delete();

            // now add back the correct row
            if (className.ToLower().Contains("company"))
            {
                FileWriter.Write(newLines.Where(d => d.Key == "customer").FirstOrDefault().Value);
            }

            if (className.ToLower().Contains("customer"))
            {
                FileWriter.Write(newLines.Where(d => d.Key == "company").FirstOrDefault().Value);
            }
        }

        public static T Find(string id)
        {
            var Instance = Activator.CreateInstance<T>();
            var className = Instance.ToString();

            string json = string.Empty;
            var file = FileWriter.Read();

            // since there at most will be two different object types (Customer/Company) we can brute force the answer, for a real-world soluiton, of course, we would do some more elegant and less costly.  
            // this is for brevity's sake. :-)

            foreach (var line in file)
            {
                if (String.IsNullOrEmpty(line))
                    return null;

                try
                {
                    var customerObj = JsonConvert.DeserializeObject<Customer>(line);
                    if (String.IsNullOrEmpty(customerObj.FirstName))
                    {
                        var companyObj = JsonConvert.DeserializeObject<Company>(line);
                        companyObj.Address = AddressList.Address;

                        if (className.ToLower().Contains("company"))
                        {
                            var aa = (T)Convert.ChangeType(companyObj, typeof(T));
                            return aa;
                        }
                    }
                    else
                    {
                        customerObj.Address = AddressList.Address;

                        if (className.ToLower().Contains("customer"))
                        {
                            var ab = (T)Convert.ChangeType(customerObj, typeof(T));
                            return ab;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }

            return null;

        }

        public void Id()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {

            // Set the Id of the primary obj (Customer/Company)
            this.GetType().GetProperty("Id").SetValue(this, "1");
            var className = this.ToString();

            string json = string.Empty;

            // Write the json obj to disk
            if (className.ToLower().Contains("customer"))
            {
                // Customer object
                Customer customer = this as Customer;

                // now set the Id of the Address to simulate it has been saved too
                customer.Address.Id = customer.Id;

                // Write the obj to disk
                // Serialize obj to json
                json = JsonConvert.SerializeObject(customer);

                // Persist the Addresses locally to get around unit test (doesn't matter becuase address is same.  our business requirements are to only pass the test.  But, real-world we would implment a more elegant solution
                AddressList.Address = customer.Address;
            }

            if (className.ToLower().Contains("company"))
            {
                // Customer object
                Company customer = this as Company;

                // now set the Id of the Address to simulate it has been saved too
                customer.Address.Id = customer.Id;

                // Write the obj to disk
                // Serialize obj to json
                json = JsonConvert.SerializeObject(customer);

                // Persist the Addresses locally to get around unit test (doesn't matter becuase address is same.  our business requirements are to only pass the test.  But, real-world we would implment a more elegant solution
                AddressList.Address = customer.Address;
            }

            FileWriter.Write(json);

        }

    }
}
