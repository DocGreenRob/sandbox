using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace sandbox
{
    public static class Extensions
    {
        public static bool IsLeapYear(this int year)
        {
            var jan_1_year = DateTime.ParseExact($"01-01-{year.Corrected()}", "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            var lastDate = new DateTime(jan_1_year.Year, 12, 31);
            var diff = lastDate - jan_1_year;

            if (diff.Days == 365)
                return true;

            return false;
        }

        public static string Corrected(this int year)
        {
            if (year >= 1000)
                return year.ToString();

            if (year < 1000 && year > 99)
                return $"0{year}";

            if (year < 100 && year > 10)
                return $"00{year}";

            if (year < 10)
                return $"000{year}";

            throw new Exception("Something is wrong!");
        }
    }
    class Program
    {
        private static List<Dictionary<string, Object>> PersonalizeCoupons(List<Dictionary<string, Object>> coupons,
                                                                           List<string> preferredCategories)
        {

            int couponLimit = 10;

            // Filter out all coupons that are not in the list of preferred categories
            List<Dictionary<string, Object>> goodCoupons = coupons.Where(c => preferredCategories.Contains(c.ElementAt(2).Value)).ToList();

            // Sort the coupons by the percentage off (highest first)
            // Take the first ten
            List<Dictionary<string, Object>> goodCouponsModified = goodCoupons.OrderByDescending(c => c.Values.ElementAt(4)).Take(10).ToList();

            // Remove the "code" entry from the Dictionary entries
            goodCouponsModified.ForEach(a => a.Remove("code"));

            return goodCouponsModified;
        }
        private static void PrintCoupon(Dictionary<string, Object> coupon)
        {
            System.Console.Write("{");
            System.Console.Write("\"couponAmount\":" + coupon["couponAmount"].ToString() + ",");
            System.Console.Write("\"upc\":\"" + coupon["upc"] + "\",");
            if (coupon.ContainsKey("code"))
            {
                System.Console.Write("\"code\":\"" + coupon["code"] + "\",");
            }
            System.Console.Write("\"itemPrice\":" + coupon["itemPrice"].ToString() + ",");
            System.Console.WriteLine("\"category\":\"" + coupon["category"] + "\"}");
        }

        private static Dictionary<string, Object> ReadCoupon(string input)
        {
            string[] couponItems = input.Split(',');

            var coupon = new Dictionary<string, Object>
            {
                { "upc", couponItems[0] },
                { "code", couponItems[1] },
                { "category", couponItems[2] },
                { "itemPrice", float.Parse(couponItems[3], CultureInfo.InvariantCulture) },
                { "couponAmount", float.Parse(couponItems[4], CultureInfo.InvariantCulture) }
            };

            //Dictionary<string, Object> dictionary = new Dictionary<string, object>();
            //dictionary.Add("upc", couponItems[0]);
            //dictionary.Add("code", couponItems[1]);
            //dictionary.Add("category", couponItems[2]);
            //dictionary.Add("itemPrice", float.Parse(couponItems[3], CultureInfo.InvariantCulture));
            //dictionary.Add("couponAmount", float.Parse(couponItems[4], CultureInfo.InvariantCulture));

            return coupon;
        }

        public enum X
        {
            val1,
            val2,
            val3
        }
        static void Main(string[] args)
        {
            List<X> xs = new List<X> { X.val1, X.val2 };
            Console.WriteLine(xs.Any(x => x.Equals(X.val3)));
            Console.ReadKey();
            return;
            Guid guid = new Guid("DP2WM8FQ0FW8V-1LT5QQ4MZ5-D1LW4VZ0FD-VZ2ZV4TW6");
            Console.WriteLine(guid.ToString());

            //var preferredCategories = Console.ReadLine().Split(',').ToList(); // cat1, cat2
            //var inputSize = Convert.ToInt32(Console.ReadLine()); // upc123, code123, cat1, 50, 5
            //var readCoupons = new List<Dictionary<string, Object>>();

            //for (int i = 0; i < inputSize; i++)
            //{
            //    readCoupons.Add(ReadCoupon(Console.ReadLine()));
            //}

            //PersonalizeCoupons(readCoupons, preferredCategories)
            //    .ForEach(coupon => PrintCoupon(coupon));


            var preferredCategories = "cat1,cat2".Split(',').ToList(); // 
            var inputSize = Convert.ToInt32("1"); // 
            var readCoupons = new List<Dictionary<string, Object>>();

            for (int i = 0; i < inputSize; i++)
            {
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,50,5"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,60,6"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,70,7"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,80,8"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,90,9"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,100,10"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,110,11"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,120,12"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,130,13"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,140,14"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,150,5"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,150,6"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,150,7"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,150,8"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,150,9"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,150,10"));
                readCoupons.Add(ReadCoupon("upc123,code123,cat1,150,11"));
            }

            PersonalizeCoupons(readCoupons, preferredCategories)
                .ForEach(coupon => PrintCoupon(coupon));

            //List<string> accountPositions = new List<string> { "B1A", "BA", "NFLX", "TSLA" };
            //List<string> mockPositionsList = new List<string> { "BA", "BA" };
            //var x = accountPositions.Where(a => a.Any(b => mockPositionsList.Contains(a.ToString())));
            //Console.WriteLine(x.Count());
            Console.ReadKey();
            return;


            List<int> years = new List<int> { 1, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 70, 1995, 17, 2012, 2013, 2014, 2015, 2016, 2017, 2018 };
            foreach(int year in years)
            {

                Console.WriteLine($"Year: {year}, IsLeapYear: {year.IsLeapYear()}");
            }

            Console.ReadKey();
            return;
            //string year = today.Year.ToString().Substring(2);
            //Console.WriteLine(year);
            //Console.ReadKey();
            return;
            double cp = 10.00;
            double cb = 1.0;

            Dictionary<double, double> priceAction = new Dictionary<double, double>();
            //priceAction.Add(.91, 1.00);
            //priceAction.Add(.85, 1.00);
            //priceAction.Add(.80, 1.00);
            //priceAction.Add(.81, 1.00);
            //priceAction.Add(.93, 1.00);
            //priceAction.Add(.82, 1.00);
            //priceAction.Add(.94, 1.00);
            //priceAction.Add(.95, 1.00);
            //priceAction.Add(.83, 1.00);
            //priceAction.Add(.75, 1.00);

            //priceAction.Add(.90, 1.00);
            //priceAction.Add(1.90, 1.00);
            //priceAction.Add(2.90, 1.00);
            //priceAction.Add(3.90, 1.00);
            //priceAction.Add(4.90, 1.00);
            //priceAction.Add(5.90, 1.00);
            //priceAction.Add(4.91, 1.00);
            //priceAction.Add(5.50, 1.00);
            //priceAction.Add(5.92, 1.00);
            //priceAction.Add(7.90, 1.00);
            //priceAction.Add(6.90, 1.00);
            //priceAction.Add(8.90, 1.00);
            //priceAction.Add(9.90, 1.00);
            //priceAction.Add(10.90, 1.00);

            priceAction.Add(1.45, 3.06);
            priceAction.Add(.79, 3.06);
            priceAction.Add(2.76, 3.06);
            priceAction.Add(2.10, 3.06);
            priceAction.Add(.80, 3.06);
            priceAction.Add(2.75, 3.06);
            priceAction.Add(3.42, 3.06);
            priceAction.Add(3.2, 3.06);
            priceAction.Add(3.64, 3.06);
            priceAction.Add(3.86, 3.06);

            priceAction.Add(3.65, 3.06);
            priceAction.Add(2.77, 3.06);
            priceAction.Add(3.66, 3.06);
            priceAction.Add(2.98, 3.06);
            priceAction.Add(3.41, 3.06);
            priceAction.Add(3.02, 3.06);
            priceAction.Add(4.55, 3.06);
            priceAction.Add(5.61, 3.06);
            priceAction.Add(4.95, 3.06);
            priceAction.Add(3.68, 3.06);
            priceAction.Add(7.62, 3.06);
            priceAction.Add(7.01, 3.06);
            priceAction.Add(7.89, 3.06);
            priceAction.Add(7.06, 3.06);

            foreach (KeyValuePair<double, double> kvp in priceAction)
            {
                FindPercentageChange(kvp.Key, kvp.Value);
            }

            Console.ReadKey();

            //Run();
        }

        private static void FindPercentageChange(double currentPrice, double costBasis)
        {
            double changeInDollars = currentPrice - costBasis;
            double percentAsDecimal = changeInDollars / costBasis;
            double percent = percentAsDecimal * 100;
            Console.WriteLine($"currentPrice: {currentPrice} - costBasis: {costBasis} = ${changeInDollars} --> change (decimal) = {percentAsDecimal} --> = {percent}%");
        }

        private static void Run()
        {
            Console.WriteLine("PLEASE PASS A STRING");
            string input = Console.ReadLine();

            if (!String.IsNullOrEmpty(input))
            {
                string result = String.Empty;

                foreach (string word in input.Split(String.Empty, StringSplitOptions.RemoveEmptyEntries))
                {
                    string newWord = Transform(word);
                    result += $"{newWord}{String.Empty}";
                }

                //result = result.Substring(0, result.Length - 1);
                Console.WriteLine($"{result}");
                Console.WriteLine("");
                Run();
            }
            else
            {
                throw new Exception("Must input a string.");
            }
        }

        private static string Transform(string word)
        {
            string result = String.Empty;

            // if the word is less than 3 characters this is the first and last
            // a --> a
            // i --> i
            // it --> it
            // be --> be
            if (word.Length < 3)

            {
                return word;
            }

            // Abe --> A1e
            // Bee --> b1e
            // A7z --> A1z
            if (word.All(Char.IsLetterOrDigit))
            {
                return $"{word.Substring(0, 1)}{word.Length - 2}{word.Substring(word.Length - 1, 1)}";
            }



            /*
            // G-d --> G-d
            // -ab --> -1b
            // ab- --> a1-
            // a1b --> a1b
            if (word.Length == 3 && !word.All(Char.IsLetterOrDigit))
            {
                // find position of Non-Alphanumeric character
                Regex regex = new Regex("[^a-zA-Z0-9]");
                int ctr = 1;

                foreach (char letter in word)
                {
                    if (regex.IsMatch(letter.ToString()))
                    {
                        result += letter.ToString();
                    }
                    else
                    {
                        // First character in the word and it is not a Non-Alphanumeric character

                        switch (ctr)
                        {
                            case 1:
                                result += letter.ToString();
                                break;
                            case 2:
                                result += "1";
                                break;
                            case 3:
                                result += letter.ToString();
                                break;
                        }

                    }

                    ctr += 1;
                }
                return result;
            }

            */
            // G-d --> G-d
            // -ab --> -1b
            // ab- --> a1-
            // a1b --> a1b
            else
            {
                // find position of Non-Alphanumeric character
                Regex regex = new Regex("[^a-zA-Z0-9]");
                int ctr = 1;

                if (word.Length == 3 && !word.All(Char.IsLetterOrDigit))
                {
                    foreach (char letter in word)
                    {
                        if (regex.IsMatch(letter.ToString()))
                        {
                            result += letter.ToString();
                        }
                        else
                        {
                            // First character in the word and it is not a Non-Alphanumeric character

                            switch (ctr)
                            {
                                case 1:
                                    result += letter.ToString();
                                    break;
                                case 2:
                                    result += "1";
                                    break;
                                case 3:
                                    result += letter.ToString();
                                    break;
                            }

                        }

                        ctr += 1;
                    }
                    return result;
                }
                else
                {
                    int lastInnerCharacter = word.Length - 1;
                    int wordLength = word.Length;
                    int innerCharacterCount = 0;

                    foreach (char letter in word)
                    {
                        //if (regex.IsMatch(letter.ToString()))
                        //{
                        //    result += letter.ToString();
                        //}
                        //else
                        //{

                        // Get 1st or last character
                        if (ctr == 1 || ctr == wordLength)
                        {
                            result += letter.ToString();
                        }
                        else
                        {
                            if (regex.IsMatch(letter.ToString()))
                            {
                                result += letter.ToString();
                                innerCharacterCount = 0;
                            }
                            else
                            {
                                innerCharacterCount += 1;
                                string lastCharacter = result.Substring(result.Length - 1);

                                try
                                {
                                    int.Parse(lastCharacter);
                                    result = result.Substring(0, result.Length - 1) + innerCharacterCount.ToString();
                                }
                                catch (Exception)
                                {

                                    result += innerCharacterCount.ToString();
                                }
                                
                                
                            }
                        }


                        //}

                        ctr += 1;
                    }
                    if (innerCharacterCount == 0)
                        return result;
                    else
                        return $"{result}";
                }

            }

            //string result = GetResult()

            // 4 Letters
            // Ball --> B2l
            // -ish --> -2h
            // abc- --> a2-
            // a-bc --> a-1c
            // ab-c --> a1-c

            // n Letters
            // Bazll --> b3l
            // -izsh --> -3h
            // azbc- --> a3-
            // a-zbc --> a-2c
            // abz-c --> a2-c
            // a(z-c --> a(1-c


            throw new NotImplementedException();
        }
    }


}
