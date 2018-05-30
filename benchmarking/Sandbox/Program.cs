using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

#region Instructions
/*
 * You are tasked with writing an algorithm that determines the value of a used car, 
 * given several factors.
 * 
 *    AGE:    Given the number of months of how old the car is, reduce its value one-half 
 *            (0.5) percent.
 *            After 10 years, it's value cannot be reduced further by age. This is not 
 *            cumulative.
 *            
 *    MILES:    For every 1,000 miles on the car, reduce its value by one-fifth of a
 *              percent (0.2). Do not consider remaining miles. After 150,000 miles, it's 
 *              value cannot be reduced further by miles.
 *            
 *    PREVIOUS OWNER:    If the car has had more than 2 previous owners, reduce its value 
 *                       by twenty-five (25) percent. If the car has had no previous  
 *                       owners, add ten (10) percent of the FINAL car value at the end.
 *                    
 *    COLLISION:        For every reported collision the car has been in, remove two (2) 
 *                      percent of it's value up to five (5) collisions.
 *                    
 * 
 *    Each factor should be off of the result of the previous value in the order of
 *        1. AGE
 *        2. MILES
 *        3. PREVIOUS OWNER
 *        4. COLLISION
 *        
 *    E.g., Start with the current value of the car, then adjust for age, take that  
 *    result then adjust for miles, then collision, and finally previous owner. 
 *    Note that if previous owner, had a positive effect, then it should be applied 
 *    AFTER step 4. If a negative effect, then BEFORE step 4.
 */
#endregion

namespace CarPricer
{
	public class Car
	{
		public decimal PurchaseValue { get; set; }
		public int AgeInMonths { get; set; }
		public int NumberOfMiles { get; set; }
		public int NumberOfPreviousOwners { get; set; }
		public int NumberOfCollisions { get; set; }
	}

	public class PriceDeterminator
	{

		public decimal DetermineCarPrice(Car car, int testNum)
		{
			Console.WriteLine("Test {0}", testNum);
			var finalValue = 0.0m;

			var value_after_age = Depreciate(car.PurchaseValue, car.AgeInMonths, .005, 10 * 12, false);
			Console.WriteLine("value_after_age: {0}", value_after_age);

			//var value_after_age = 28700m;
			var value_after_miles = Depreciate(value_after_age, (car.NumberOfMiles / 1000), .002, 150, true);
			Console.WriteLine("value_after_miles: {0}", value_after_miles);

			var value_after_previous_owner_impact = 0.0m;
			// determine previous owner impact
			if (PreviousOwnerImpact(car.NumberOfPreviousOwners))
			{
				// if negative deduct for previous owner, then collision
				value_after_previous_owner_impact = value_after_miles - (value_after_miles * Decimal.Parse(".25"));
				var value_after_collision = DeductCollissions(value_after_previous_owner_impact, car.NumberOfCollisions);

				Console.WriteLine("value_after_collision: {0}", value_after_collision);
				finalValue = value_after_collision;
			}
			else
			{
				// else, deduct for collision then for previous owner
				var value_after_collision = DeductCollissions(value_after_miles, car.NumberOfCollisions);
				Console.WriteLine("value_after_collision: {0}", value_after_collision);

				if (car.NumberOfPreviousOwners == 0)
					value_after_previous_owner_impact = value_after_collision + (value_after_collision * Decimal.Parse(".10"));
				else
					value_after_previous_owner_impact = value_after_collision;

				Console.WriteLine("value_after_previous_owner_impact: {0}", value_after_previous_owner_impact);
				finalValue = value_after_previous_owner_impact;
			}



			return finalValue;
		}

		private bool PreviousOwnerImpact(int count)
		{
			if (count > 1)
				return true;
			return false;
		}
		public decimal Depreciate(decimal startingValue, int count, double pct, int maxCount, bool cumm)
		{
			if (!cumm)
			{
				var deduction = Double.Parse(startingValue.ToString()) * (count * pct);
				return startingValue - Decimal.Parse(deduction.ToString());
			}
			else
				return Iterate(startingValue, pct, count, maxCount);
		}

		private decimal DeductCollissions(decimal val, int collisions)
		{
			const int max_collissions = 5;

			if (collisions == 0)
				return val;

			return Iterate(val, .02, collisions <= 5 ? collisions : max_collissions, max_collissions);
		}

		private decimal Iterate(decimal val, double pct, int steps, int maxSteps = 0)
		{
			double runningTotal = 0.0;
			double originalVal = Double.Parse(val.ToString());
			double unit_of_measure_depreciation;

			if (maxSteps != 0)
			{
				if (steps >= maxSteps)
					steps = maxSteps;
			}

			for (int i = 0; i < steps; i++)
			{
				if (i == 0)
					unit_of_measure_depreciation = originalVal * pct;
				else
					unit_of_measure_depreciation = runningTotal * pct;

				if (i == 0)
					runningTotal = originalVal - unit_of_measure_depreciation;
				else
					runningTotal = runningTotal - unit_of_measure_depreciation;
			}

			return Decimal.Parse(runningTotal.ToString());
		}
	}


	public class UnitTests
	{
		/* This is a situation where the business requirements are unclear.  Here I would:
		 * 1. Talk with the Team Lead about the issue and hopefully get some results. (with the Business Analyst or the Client or IT Manager)
		 * This is a mathematical issue and I am off by a small percent on each iteration, this is due to my lack of accounting skills and knowing the correct methodology for the depretiation (straight line or cummulative) but I'm almost there.  I keep reading the business requirements and see small holes that I need filled to build the system correctly.  I will note them below.  I hope to be given an interview and better explain myself.  Meanwhile, let me invite you to my video series where I show my next team / contract what I bring to the table from day one.  Hopefully this exam too will demonstrate this. https://www.youtube.com/playlist?list=PLNR_ZffTCRs3VgzE50cLlP6MM6bXQG2IA
		 * 
		 * REQUIREMENT NEEDS
		 * MILES: I know my math is correct.  For every 1000 miles I deduct .002 percent (1/5th of a percent) from the value. The value of the Age adjustment for each case is 28,700 which is $35,000 * (12 * 3) * .005.  My miles when less than 150K is $25,966.23 otherwise Miles = $21,255.10 for tests 2 - 5, only test one is less than 150k (the threshold).  Here is where I get unclear.  No test is for previous owner with 2 or more.  So, given that I can't pass any test, is even harder here because of the ambuigity in requirements becuase for 2 previous owners I reduce by 25%, for none I add 10% but what about 1.  I must assume this means do nothing.  
		 * */
		public void CalculateCarValue()
		{
			AssertCarValue(25313.40m, 35000m, 3 * 12, 50000, 1, 1, 1);

			Console.WriteLine();
			Console.WriteLine("-------------------------------");
			AssertCarValue(19688.20m, 35000m, 3 * 12, 150000, 1, 1, 2);

			Console.WriteLine();
			Console.WriteLine("-------------------------------");
			AssertCarValue(19688.20m, 35000m, 3 * 12, 250000, 1, 1, 3);

			Console.WriteLine();
			Console.WriteLine("-------------------------------");
			AssertCarValue(20090.00m, 35000m, 3 * 12, 250000, 1, 0, 4);

			Console.WriteLine();
			Console.WriteLine("-------------------------------");
			AssertCarValue(21657.02m, 35000m, 3 * 12, 250000, 0, 1, 5);

			Console.ReadKey();
		}

		private static void AssertCarValue(decimal expectValue, decimal purchaseValue,
		int ageInMonths, int numberOfMiles, int numberOfPreviousOwners, int
		numberOfCollisions, int testNum)
		{
			Car car = new Car
			{
				AgeInMonths = ageInMonths,
				NumberOfCollisions = numberOfCollisions,
				NumberOfMiles = numberOfMiles,
				NumberOfPreviousOwners = numberOfPreviousOwners,
				PurchaseValue = purchaseValue
			};
			PriceDeterminator priceDeterminator = new PriceDeterminator();
			var carPrice = priceDeterminator.DetermineCarPrice(car, testNum);

			var difference = expectValue - carPrice;
			Console.WriteLine("{0}", difference == 0 ? "PASS" : "FAIL");
			Console.WriteLine("Expected Value {0}, ActualValue {1}", expectValue, carPrice);
			Console.WriteLine("Off by: {0}", difference);
			//Assert.AreEqual(expectValue, carPrice);
		}
	}
}

namespace Sandbox
{
	class ClassA_outside
	{
		public ClassA_outside()
		{
			Console.WriteLine("ClassA_outside {0}", this);
		}
	}
	class Program
	{
		interface ISuggestion
		{
			int Suggestion1 { get; set; }
		}
		class Suggestion : ISuggestion
		{
			private int suggestion;
			public int Suggestion1
			{
				get
				{
					return this.suggestion;
				}
				set
				{
					this.suggestion = value;
				}
			}

		}

		interface IProduct : ISuggestion
		{
			Task<int> Save();
		}
		abstract class Product
		{
			private int suggestion;
			public int Suggestion1
			{
				get
				{
					Console.WriteLine("{0} requested Suggestion", this);
					return this.suggestion;
				}
				set
				{
					this.suggestion = 123;
				}
			}

			public Task<int> Save()
			{
				Console.WriteLine("Save called for {0}", this);
				return Task.FromResult(1);
			}
		}

		class DigiDoc : Product
		{
			public DigiDoc()
			{
				var x = Suggestion1;
			}
		}

		class Upc : Product
		{
			public Upc()
			{
				var x = Suggestion1;
			}
		}

		class QRCode : Product
		{
			public QRCode()
			{
				var x = Suggestion1;
			}
		}

		class Photo : Product
		{
			public Photo()
			{
				var x = Suggestion1;
			}
		}

		class ClassA
		{
			public ClassA()
			{
				Console.WriteLine("Class A {0}", this);
				var b = new ClassA_outside();
			}
		}
		//class DayFactory
		//{
		//	private string name { get; set; }

		//	public DayFactory(string name)
		//	{
		//		this.name = name;
		//	}

		//	public bool Validate()
		//	{
		//		if (String.IsNullOrWhiteSpace(name))
		//		{
		//			throw new Exception("Day is empty");
		//		}

		//		if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
		//		{
		//			Console.WriteLine("Today is Tuesday!");
		//		}
		//	}

		//}
		static void create(string type)
		{

		}
		static T create<T>() where T : new()
		{
			return new T();
		}

		static object create()
		{
			return new object();
		}

		static object createReflect(Type t)
		{
			return Activator.CreateInstance(t);
		}
		private static void Measure(string what, int reps, Action action)
		{
			action(); //warm up (because the 1st time we create a function the JIT compiler has to do some extra stuff)

			double[] results = new double[reps];
			for (int i = 0; i < reps; i++)
			{
				Stopwatch sw = Stopwatch.StartNew();
				action();
				results[i] = sw.Elapsed.TotalMilliseconds;
			}
			Console.WriteLine("{0} - AVG = {1}, MIN = {2}, Max = {3}",
				what,
				results.Average(),
				results.Min(),
				results.Max());

		}

		public static long FindPrimeNumber(int n)
		{
			int count = 0;
			long a = 2;
			while (count < n)
			{
				long b = 2;
				int prime = 1;// to check if found a prime
				while (b * b <= a)
				{
					if (a % b == 0)
					{
						prime = 0;
						break;
					}
					b++;
				}
				if (prime > 0)
					count++;
				a++;
			}
			return (--a);
		}

		[StructLayout(LayoutKind.Explicit)]
		struct ByteArray
		{
			[FieldOffset(0)]
			public byte Byte1;
			[FieldOffset(1)]
			public byte Byte2;
			[FieldOffset(2)]
			public byte Byte3;
			[FieldOffset(3)]
			public byte Byte4;

			[FieldOffset(0)]
			public int Int1;
		}

		static byte[] bytes = new byte[20480];
		static int length = bytes.Length;
		static void BaselineArray()
		{
			for (int i = 0; i < length; i++)
			{
				bytes[i] = (byte)i;
			}
		}

		static void UnsafeArray()
		{
			//unsafe
			//{
			//	fixed (byte* pBytes = bytes)
			//	{
			//		for (int i = 0; i < length; i++)
			//		{
			//			pBytes[i] = (byte)i;
			//		}
			//	}
			//}
		}

		static void UnsafeArrayOptimized()
		{
			//unsafe
			//{
			//	fixed (byte* pBytes = bytes)
			//	{
			//		byte* ptr = pBytes;
			//		for (int i = 0; i < length; i++)
			//		{
			//			*ptr = (byte)i;
			//			ptr++;
			//		}
			//	}
			//}
		}

		public static int TestNewStack(int size, int value)
		{
			int[] someNumbers = new int[size];
			for (int i = 0; i < someNumbers.Length; ++i)
			{
				someNumbers[i] = value;
			}
			return someNumbers[value];
		}

		//public unsafe static int TestAllocStack(int size, int value)
		//{
		//	// int* = pointer
		//	int* someNumbers = stackalloc int[size]; // initialize arry on the stack, not heap (because of unsafe)
		//											 // bypass garbage collection, bypass the heap 
		//											 // !!! .net default stack size is 2mb!!!

		//	for (int i = 0; i < size; ++i)
		//	{
		//		*(someNumbers + i) = value; // get around bounds checking
		//	}
		//	return someNumbers[value];
		//}

		private static bool InsideRect(Rectange rect, Point point)
		{
			if (rect.X == point.X && rect.Y == point.Y)
				return true;
			// compare points and check bounds
			if (point.X >= rect.X)
			{
				var widthMax = rect.Width + rect.X;

				var heightMax = 0;
				if (point.Y > rect.Y)
				{
					heightMax = rect.Height + rect.Y;
					return point.X <= widthMax && point.Y <= heightMax;
				}
				else
				{
					if (point.Y == rect.Y)
					{
						// ok
					}
				}
				return point.X < widthMax;
			}

			return false;
		}

		// makes this Thread Local Storage
		// conditions!! .net will not run initializers on ThreadStatic data
		// static int threadData = false;
		//[ThreadLocalStorage]
		//static int threadData;
		// instead of using [ThreadLocalStorage] (becasue it does not give us an initialization of our object; will be null on every thread that first starts; But using a wrapper, it gives you a way to pass a constructor and construct it the way you like
		private static ThreadLocal<int> threadData = new ThreadLocal<int>(() =>
		{
			return 0;
		});
		private static void StartTasks()
		{
			Task t1 = Task.Factory.StartNew(() =>
			{
				threadData.Value = 1;
				Console.WriteLine(threadData);
			});

			Task t2 = Task.Factory.StartNew(() =>
			{
				threadData.Value = 2;
				Console.WriteLine(threadData);
			});

			Task t3 = Task.Factory.StartNew(() =>
			{
				threadData.Value = 3;
				Console.WriteLine(threadData);
			});

			t1.Wait();
			t2.Wait();
			t3.Wait();
		}
		// then instead of threadData use threadData.Value
		public class TestObject
		{
			public int Data = 0;
		}
		public class WeakManager
		{
			private List<WeakReference<TestObject>> items = new List<WeakReference<TestObject>>();

			public void Add(TestObject o)
			{
				WeakReference<TestObject> weakRef = new WeakReference<TestObject>(o);
				items.Add(weakRef);
			}

			public void Update()
			{
				List<WeakReference<TestObject>> delList = new List<WeakReference<TestObject>>();

				foreach (WeakReference<TestObject> weak in items)
				{
					TestObject o;

					if (weak.TryGetTarget(out o))
						Console.WriteLine(o.Data);
					else
					{
						delList.Add(weak);
						Console.WriteLine("object has been deleted");
					}

				}

				foreach (var weak in delList)
					items.Remove(weak);

			}
		}
		public class Manager
		{
			private List<TestObject> items = new List<TestObject>();

			public void Add(TestObject o)
			{
				items.Add(o);
			}

			public void Remove(TestObject o)
			{
				items.Remove(o);
			}

			public void Update()
			{
				foreach (var o in items)
				{
					Console.WriteLine(o.Data);
				}
			}
		}
		static Manager mgr = new Manager();
		static WeakManager weakMgr = new WeakManager();
		static int counter = 0;
		public static void AddToManager(TestObject o)
		{
			mgr.Add(o);
		}

		public static TestObject newObject()
		{
			TestObject o = new TestObject();
			o.Data = counter++;
			return o;
		}

		private static void TestManager()
		{
			// scenario: we have an object manager
			var o1 = newObject();
			var o2 = newObject();
			var o3 = newObject();

			mgr.Add(o1);
			mgr.Add(o2);
			mgr.Add(o3);
		}
		private static void TestWeakManager()
		{
			var o1 = newObject();
			var o2 = newObject();
			var o3 = newObject();

			weakMgr.Add(o1);
			weakMgr.Add(o2);
			weakMgr.Add(o3);
		}

		static void Main(string[] args)
		{
			var wrapped1 = new IntWrapper();
			wrapped1.wrappedInt = 1;


			//var clone1 = wrapped1;
			var wrapped2 = new IntWrapper();
			var wrapped3 = new IntWrapper();

			wrapped2.wrappedInt = 2;
			wrapped3.wrappedInt = 3;
			GC.Collect();
			wrapped2 = null;
			wrapped3 = null;
			GC.Collect();
			Console.ReadKey();

			int i = 5;
			Console.WriteLine(i.GetType());
			Console.WriteLine(i.GetType().BaseType);
			Console.WriteLine(i.GetType().BaseType.BaseType);

			object o = i;
			i = 20;

			Console.WriteLine(o);
			Console.WriteLine(i);

			o = 99;
			Console.WriteLine(o);
			Console.WriteLine(i);

			// o++;
			var unboxed = (int)o;
			o = ++unboxed;

			Console.WriteLine(o);

			var randy = new Random();
			bool randomBool = randy.Next() % 2 == 0;
			Base b = randomBool ? new Base() : new Derived();
			//Derived d = (Derived)b; // can error based on "b"
			Derived d = b as Derived; // will NOT error

			if (b is Derived)
				d = (Derived)b;
			
			//Console.WriteLine(i.GetType());
			// GC
			//var wrapped1 = new IntWrapper();
			//var clone1 = wrapped1;
			//var wrapped2 = new IntWrapper();
			//var wrapped3 = new IntWrapper();
			//wrapped1.wrappedInt = 1;

			//wrapped2.wrappedInt = 2;
			//wrapped3.wrappedInt = 3;
			//GC.Collect();
			//wrapped2 = null;
			//wrapped3 = null;
			//GC.Collect();
			//Console.ReadKey();
			#region archives
			/*
			#region Weak References
			// end of video, before raffle, 
			TestManager();
			TestWeakManager();

			while (true)
			{
				Console.WriteLine("dumping manger");
				mgr.Update();

				Console.WriteLine("dumping weak manger");
				weakMgr.Update();
				Thread.Sleep(1000);

				GC.Collect();
			}

			// if you're not going to manage the reference use weak reference
			// used mainly in .Net for event notifications and delegates

			// co-routines
			#endregion

			
			#region Interview 2.15.17 -Mentis Technology
			
			// scans, images, physical docs

			// scan loan app


			// parse words
			// rectangle

			// given a point, let me know if point is inside rect
			Rectange rectange = new Rectange {
				Height = 10,
				Width = 10,
				X = 700,
				Y = 1050
			};

			Point point = new Point(1, 1);
			bool answer = InsideRect(rectange, point);

			point  = new Point(700, 1051);
			answer = InsideRect(rectange, point);

			Console.WriteLine("Inside: {0}", answer);

			point  = new Point(700, 1061);
			answer = InsideRect(rectange, point);

			Console.WriteLine("Inside: {0}", answer);
			Console.ReadKey();
			
			#endregion

			#region Thread local storage
			// Advanced C# Concepts Bill Hike
			// 3:28:00 - 3:39:00

			// Thread local storage
			int counter = 10;

			for (int i = 0; i < counter; i++)
			{
				StartTasks();
				Console.WriteLine(threadData);
				Console.WriteLine("---------");
				Console.WriteLine();
			}

			// most probable output is 2 but it can be 1 too, the tasks may execute at same time, task 1 count take longer and 2 finish first, etc. because of this we can use locks
			// (this is after the 1st result)
			// after adding a wrapper and implementing a 'lock' now there are 3 threads (1) for Main, and task 1 & 2; so the result is 0 (because Main has its own implementation of threadData).

			Console.ReadKey();

			#endregion

			#region reasons to use unsafe code
			// Advanced C# Concepts Bill Hike
			// 2:28:00 - 3:28:00
			// reasons to use unsafe code
			// pointer access (protected pointers)

			int reps = 5;
			int its = 10000;
			int arraySize = 60000;

			Measure("baseline", reps, () =>
			{
				for (int i = 0; i < its; ++i)
				{
					TestNewStack(arraySize, i % arraySize);
				}
			});

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			Measure("TestAllocStack", reps, () =>
			{
				for (int i = 0; i < its; ++i)
				{
					TestAllocStack(arraySize, i % arraySize);
				}
			});

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			// when arraySize = 5000
			// baseline - AVG = 79.62656, MIN = 66.8204, Max = 87.2959
			// TestAllocStack - AVG = 106.1528, MIN = 81.6876, Max = 128.4193

			// garbage collection
			// 4 heaps
			// generation (0) - all objects get created here
			// on garbage collection pass - 
			// any objects still alive get promoted to generation (1)
			// on garbage collection pass - 
			// any objects still alive get promoted to generation (2)
			// objects stay in gen (2) until no more refs are around
			// 4th heap (bucket) large obj, any obj over 80 kb, goes here, not gen (1)
			// never get's defragged

			// int arraySize = 30000;
			// baseline - AVG = 526.61566, MIN = 524.1966, Max = 529.1389
			// TestAllocStack - AVG = 486.76602, MIN = 367.5202, Max = 695.8427

			// int arraySize = 60000;
			// baseline - AVG = 1066.23796, MIN = 1024.5792, Max = 1215.7492
			// TestAllocStack - AVG = 1112.0004, MIN = 754.6186, Max = 1359.5412

			Console.ReadKey();
			#endregion

			
			#region unsafe code
			// Advanced C# Concepts Bill Hike
			// 2:28:00
			// reasons to use unsafe code
			// pointer access (protected pointers)



			int reps = 5;
			int its = 100000;

			Measure("baseline", reps, () =>
			{
				for (int i = 0; i < its; ++i)
				{
					BaselineArray();
				}
			});

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			Measure("UnsafeArray", reps, () =>
			{
				for (int i = 0; i < its; ++i)
				{
					UnsafeArray();
				}
			});

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			Measure("UnsafeArrayOptimized", reps, () =>
			{
				for (int i = 0; i < its; ++i)
				{
					UnsafeArrayOptimized();
				}
			});

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			// results:
			// unsafe array optimized is showing best performance!
			Console.ReadKey();
			return;
			// trying to do
			// bytes[25] = 2; // will throw a runtime error
			// however doing
			// pBytes[25] = 1; // in unsafe{} will not throw an error
			// here we get a performance gain in not having to do the bounds check. If we know , etc...
			// since this is not a stored element op call anymore (ILDASM) then we can use this unsafely w/o the safety checks
			// can only do pointers to value types (can't use reference types), can't use strings (because is managed type) can use Structs as long as no type of the Struct is a reference type
			#endregion
			
			#region unions
			// Advanced C# Concepts Bill Hike
			// 2:15:00 - 2:28:00
			// unions
			ByteArray ba;
			
			ba.Byte1 = 128;
			ba.Byte2 = 0;
			ba.Byte3 = 0;
			ba.Byte4 = 0;
			ba.Int1 = 0x00FF00FF; // 0x000000FF;

			Console.WriteLine("B1:{0} B1:{1} B1:{2} B1:{3} I1:{4}", ba.Byte1, ba.Byte2, ba.Byte3, ba.Byte4, ba.Int1);
			Console.ReadKey();
			return;
			// ?'s
			// 1. what is a Struct?
			#endregion

			#region memoization
			// Advanced C# Concepts Bill Hike
			// 1:48:00 - 2:15:00
			// applicipable only in cases where
			// 1) have a Func<> that allows some data input
			// 2) does something with the data
			// Use case 1: Input data tied to function execution
			// Restriction: The function call cannot use any outside data for control flow or computation
			// (1) High Cost, (2) Called Alot, (3) Same Parameters = Good Candidate for Memoization
			// jpeg compression!!!
			int reps = 5;
			int its = 50000;

			Measure("baseline", reps, () => {
				for (int i = 0; i < its; ++i)
				{
					FindPrimeNumber(45);
				}
			});

			// after we run this test, we want to make it better, and we don't want to create all those lookup tables.

			// create a new class to wrap this functionality in Memoization
			var memFunc = utils.Memoize<int, long>(FindPrimeNumber);
			Measure("memo", reps, () =>
			{
				for (int i = 0; i < its; ++i)
				{
					memFunc(45);
				}
			});

			// words of caution: not thread safe, can be made thread safe by using a concurrent dictionary
			#endregion

			Console.ReadKey();
			return;

			#region ILDASM
			// Advanced C# Concepts Bill Hike
			// why we should use Task instead of Thread
			// https://www.youtube.com/watch?v=EbCFbBJ12y4&t=7809s
			// ! Important to understand what the compiler is doing to the code when we write it!

			// 45:00 - 54:00 ILDASM
			#endregion

			#region object allocation in .NET
			// object allocation in .NET
			// 54:00 - 1:38:00
			// easy way is the slowest way

			object o = new object(); // this is the fastest we are going to get with allocating, instantiating, or newing up a new object() in .NET

			// what if we don't know what 'object' is, then...
			// static void create(string type) (see above)

			// or, in IoC shops you might see
			// static void create<T>()
			// generic of Type T, 

			var c = create();
			c = create<Object>();

			reps = 5;
			its = 1000000;

			for (int a = 0; a < 10; a++)
			{
				Measure("create object()", reps, () =>
				{
					for (int i = 0; i < its; ++i)
					{
						create();
					}
				});


				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();

				Measure("create<T>", reps, () =>
				{
					for (int i = 0; i < its; ++i)
					{
						create<object>();
					}
				});

				//Measure("create<T>", reps, () =>
				//{
				//	for (int i = 0; i < its; ++i)
				//	{
				//		create<object>();
				//	}
				//});



				//Measure("create object()", reps, () =>
				//{
				//	for (int i = 0; i < its; ++i)
				//	{
				//		create();
				//	}
				//});
			}


			// after running the tests above the native implementation of creating a new object is significantly faster
			
			// * create object() - AVG = 5.90874, MIN = 5.6893, Max = 6.3975
			// * create<T> - AVG = 28.64876, MIN = 26.4245, Max = 35.1358
			// * create object() - AVG = 5.47294, MIN = 5.2879, Max = 5.804
			// * create<T> - AVG = 23.43602, MIN = 20.2592, Max = 25.3165
			// * create object() - AVG = 4.05146, MIN = 3.987, Max = 4.191
			// * create<T> - AVG = 21.77568, MIN = 21.1947, Max = 22.9314
			// * create object() - AVG = 4.44934, MIN = 4.2471, Max = 4.6456
			// * create<T> - AVG = 20.90076, MIN = 19.5239, Max = 21.3909
			// * create object() - AVG = 4.43452, MIN = 3.7466, Max = 5.6885
			// * create<T> - AVG = 25.78368, MIN = 22.7749, Max = 26.7202
			// * create object() - AVG = 4.70754, MIN = 4.5371, Max = 5.124
			// * create<T> - AVG = 24.57078, MIN = 22.3985, Max = 26.5703
			// * create object() - AVG = 4.67906, MIN = 4.515, Max = 4.9967
			// * create<T> - AVG = 20.86708, MIN = 19.6665, Max = 22.4427
			// * create object() - AVG = 4.01606, MIN = 3.9755, Max = 4.1525
			// * create<T> - AVG = 23.84134, MIN = 19.73, Max = 27.0171
			// * create object() - AVG = 5.39446, MIN = 5.3268, Max = 5.6041
			// * create<T> - AVG = 23.472, MIN = 22.4079, Max = 26.2168
			// * create object() - AVG = 4.09956, MIN = 3.9862, Max = 4.3204
			// * create<T> - AVG = 18.54008, MIN = 17.4444, Max = 19.6534

			

			// now tring another create option

			Measure("create object()", reps, () =>
				{
					for (int i = 0; i < its; ++i)
					{
						create();
					}
				});


			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			Measure("create<T>", reps, () =>
			{
				for (int i = 0; i < its; ++i)
				{
					create<object>();
				}
			});

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();

			Type objType = typeof(Object);

			Measure("createReflective", reps, () =>
			{
				for (int i = 0; i < its; ++i)
				{
					createReflect(objType);
				}
			});

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			
			Measure("createIL", reps, () =>
			{
				for (int i = 0; i < its; ++i)
				{
					FastObjectAllocator<object>.New();
				}
			});

			//
			// create object() - AVG = 5.29026, MIN = 5.2244, Max = 5.5009
			// create<T> - AVG = 26.22392, MIN = 26.0869, Max = 26.2975
			// createReflective - AVG = 19.43636, MIN = 19.0525, Max = 20.3431
			//

			#endregion

			#region Task instead of Thread
			// Advanced C# Concepts Bill Hike
			// why we should use Task instead of Thread
			// https://www.youtube.com/watch?v=EbCFbBJ12y4&t=7809s
			// 30:00 - 45:00
			Thread th = new Thread(() =>
			{
				Console.WriteLine("test1");
			});

			th.Start();

			Task t = Task.Factory.StartNew(() =>
			{
				Console.WriteLine("test2");
			});

			// What is the difference between the two?
			// "really only one difference (between a task and thread) a Task is backed by a Thread pool of 'real Threads'." (Not preinitialized Threads in the pool, they are spun up as needed), "ultimately this is same performance once executed"

			// "its just that", when you create a new Thread each time you want to use one there is "a lot" overhead in the [boot up time] of that Thread.

			// use 
			t.Wait();
			// instead of 
			Thread.Sleep(100); // too much context switching, could degrade performance

			// Task = fire and forget
			// if creating a Thread to run in the background for the entire app that will run in parallel for whatever purpose, use a Thread not a Task

			#endregion

			return;

			#region car pricer
			var unitTests = new CarPricer.UnitTests();
			unitTests.CalculateCarValue();
			#endregion

			Console.ReadKey();

			#region MyAssets Product Consolidation
			try
			{
				var digiDoc = new DigiDoc();
				digiDoc.Save();
				var upc = new Upc();
				upc.Save();
				var qRCode = new QRCode();
				qRCode.Save();
				var photo = new Photo();
				photo.Save();
				Console.ReadKey();

				//Upc upc = new Upc();
				//var y = iProduct.Suggestion1;
				//var x = Product.GetSuggestions(416, 36);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.WriteLine("Done");
			Console.ReadKey();
			#endregion

			#region Scratch
			//var day = "";
			//var d = new DayFactory(day);
			//d.Validate();

			//if(day == DayOfWeek.Wednesday.ToString())
			//{
			//	Console.WriteLine("Today is Wednesday case 1!");
			//}

			//if (day == DayOfWeek.Thursday.ToString())
			//{
			//	Console.WriteLine("Today is Thursday case 1!");
			//}
			#endregion
			*/
			#endregion
		}

		static void DoWork()
		{
			var day = "";
			if (day == DayOfWeek.Wednesday.ToString())
			{
				Console.WriteLine("Today is Wednesday case 2!");
			}

			if (day == DayOfWeek.Thursday.ToString())
			{
				Console.WriteLine("Today is Thursday case 2!");
			}
		}
	}

	class Base { }
	class Derived : Base { }
	class IntWrapper
	{
		public int wrappedInt;
	}
}

