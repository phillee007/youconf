using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeSuperior.Lucifure.Tutorial
{
	// -----------------------------------------------------------------------------------------------------------------
	// Lucifure Stash supports stashing items in a collection as individual Azure table properties. This alleviates the 
	//	need to unroll a collection explicitly especially when the collections maybe of variable length.
	//
	// Any collection with supports the IList or IList<T> interface is supported. This includes single dimension array 
	// support too.
	// 
	// If the collection is strongly type, morphing is supported. 
	// NOTE: For heterogeneous collections, (list of objects), morphing is not supported. Collection of objects 
	// can only be of Azure table storage primitive types. (This is because the original type information is lost and 
	// cannot be recovered at read time.)
	//
	// Collection support is implemented orthogonally such that all features available to scalar members are 
	// available to collections too.
	// -----------------------------------------------------------------------------------------------------------------

	// Define a class to hold chronological salary information so we can store it as a collection
	public 
	class SalaryInfo
	{
			public 
			DateTime							DateStart { get; set; }

			public 
			DateTime							DateEnd { get; set; }
	
			public 
			double								Salary { get; set; }

#region Orthogonal Support Code
        public
        override
        bool
        Equals(
            object								obj)
        {
            SalaryInfo							rhs;

            return	(Object.ReferenceEquals(this, obj)
                        ||	(((rhs = obj as SalaryInfo) != null) 
								&& DateStart	== rhs.DateStart
								&& DateEnd		== rhs.DateEnd
								&& (Math.Abs(Salary - rhs.Salary) < 0.00001)));
        }

        // Override GetHashCode too, although we do not expect to use it
        public
        override
        int
        GetHashCode()
        {
			return (DateStart.ToString() + DateEnd.ToString() + Salary.ToString()).GetHashCode();
        }
#endregion x
	}

	// -----------------------------------------------------------------------------------------------------------------
	// and an enumeration of programming languages, we will store a collection of enums too.

	public
	enum ProgramingLanguages
	{
		Assembly,
		C,
		CPP,
		CSharp,
		FSharp,
		VisualBasic,
	}

	// -----------------------------------------------------------------------------------------------------------------
	// -----------------------------------------------------------------------------------------------------------------
	// Lets be explicit about our collections and decorate the member with the StashCollection attribute

    [StashEntity(Mode=StashMode.Explicit)]
    public 
    class EmployeeSkills					:	EmployeeInfo							
    {
		[StashCollection]
		public 
		List<ProgramingLanguages>				Languages;

		[StashCollection]
		public 
		ArrayList								Certifications;							

		// Holds an array of salary history objects. This auto morphs using the DataContract serializer
		[StashCollection]
		public 
		SalaryInfo[]							SalaryHistory;


		// Guess what happens it we decorate with Stash instead of StashCollection?
		[Stash]
		public
		int[]									MysteryNumbers;

#region Orthogonal Support Code
        // Override the Equals so we can use it to validate our tests
        public
        override
        bool
        Equals(
            object								obj)
        {
            EmployeeSkills						rhs;

            return	(Object.ReferenceEquals(this, obj)
                        ||	(((rhs = obj as EmployeeSkills) != null) 
								&& base.Equals(rhs)
								&& Enumerable.SequenceEqual(Languages, rhs.Languages)	
								&& ListEquals(Certifications, rhs.Certifications)
								&& Enumerable.SequenceEqual(SalaryHistory, rhs.SalaryHistory)
								&& Enumerable.SequenceEqual(MysteryNumbers, rhs.MysteryNumbers)));
        }

		static
		bool
		ListEquals(
			ArrayList							lhs,
			ArrayList							rhs)
		{
			int									count;
			bool								result;

			if (result = (lhs == null && rhs == null))
			{
				;
			}
			else if (lhs != null && rhs != null && (count = lhs.Count) == rhs.Count)
			{
				bool							isEqual = true;

				for(int i = 0; i < count; ++i)
					if (!lhs[i].Equals(rhs[i])) 
					{
						isEqual = false;
						break;
					}
				
				result = isEqual;
			}

			return result;
		}

        // Override GetHashCode too, although we do not expect to use it
        public
        override
        int
        GetHashCode()
        {
			return (Department + EmployeeId).GetHashCode();
        }
#endregion x
	}

    // -----------------------------------------------------------------------------------------------------------------

    [TestClass]
    public 
    class TestCollections
    {
        [TestMethod]
        public 
        void 
        Tutorial_06_Collections() 
        {
			//----------------------------------------------------------------------------------------------------------
			// Create a Stash Client

            StashClient<EmployeeSkills>
            client = StashConfiguration.GetClient<EmployeeSkills>();		

			client.CreateTableIfNotExist();

			//----------------------------------------------------------------------------------------------------------
			// Lets create an instance of the class we what to 'stash'
			
			const 
			string departmentDev	= "Dev";

			const
			string ssn				= "123456789";

			EmployeeSkills
			dataWritten = new EmployeeSkills {
			                Department	= departmentDev,
			                EmployeeId	= Guid.NewGuid().ToString(),
			                Name		= "John Doe",
			                SkillLevel	= 8,
			                DateOfBirth = new DateTime(1990, 1, 1),
							SSN			= ssn,
							Salutation	= Salutation.Ms };

			// update the private field
			dataWritten.SetAnnualSalary();
			
			// Populate Languages, certification and salary history
			dataWritten.Languages = new List<ProgramingLanguages> { 
												ProgramingLanguages.Assembly, 
												ProgramingLanguages.VisualBasic };
												 
			const 
			string								cert0 = "MSSQL",
												cert1 = "MSCPP",
												cert2 = "MSRP";

			dataWritten.Certifications = new ArrayList { 
												cert0, 
												cert1, 
												cert2 };

			dataWritten.SalaryHistory = new SalaryInfo[] {
												new SalaryInfo { 
													DateStart = new DateTime(2009, 1, 1),
													DateEnd = new DateTime(2010, 1, 1),
													Salary = 100000 },
												new SalaryInfo { 
													DateStart = new DateTime(2010, 1, 1),
													DateEnd = new DateTime(2012, 1, 1),
													Salary = 110000 }, 
												};

			dataWritten.MysteryNumbers = new int[] { 3, 2, 1, 0 };

			//----------------------------------------------------------------------------------------------------------
			// Stash it			 
			
			client.Insert(dataWritten);

			//----------------------------------------------------------------------------------------------------------
			// Read back the data
			
			EmployeeSkills
			dataRead = client.Get(dataWritten.Department, dataWritten.EmployeeId); 

			//----------------------------------------------------------------------------------------------------------
			// Verify we got back what we put in
			
			Assert.IsTrue(dataWritten.Equals(dataRead));

			//----------------------------------------------------------------------------------------------------------
			// Verify exactly what was written to table storage by using the Generic Pool class we wrote 
			// earlier.

			// set this up to target the correct table storage entity set
			StashClient<GenericPool>
			clientGenericPool = StashConfiguration.GetClient<GenericPool>(
									new StashClientOptions {
											OverrideEntitySetName			= o => "EmployeeSkills",
											OverrideEntitySetNameIsDynamic	= false,
											Feedback						= StashConfiguration.TraceFeedback,
										});		


			GenericPool
			dataPool = clientGenericPool.Get(
											dataRead.Department, 
											dataRead.EmployeeId); 

			// All collection items are suffixed with the index base 0, in the format "_000". 
			// Because the "_" is used for demarcating collection indexes, explicit defined members are not allowed
			// to have the embedded "_" character in the table property name.

			// Test for languages
			Assert.IsTrue(
				dataPool.Pool.Where(
						kv =>
							kv.Key == "Languages_000" 
						&& ((ProgramingLanguages)kv.Value == ProgramingLanguages.Assembly)).ToList().Count == 1);


			Assert.IsTrue(
				dataPool.Pool.Where(
						kv =>
							kv.Key == "Languages_001" 
						&& ((ProgramingLanguages)kv.Value == ProgramingLanguages.VisualBasic)).ToList().Count == 1);

			// Test for certification
			Assert.IsTrue(
				dataPool.Pool.Where(
						kv =>
							kv.Key == "Certifications_000" 
						&& ((string) kv.Value == cert0)).ToList().Count == 1);

			Assert.IsTrue(
				dataPool.Pool.Where(
						kv =>
							kv.Key == "Certifications_001" 
						&& ((string) kv.Value == cert1)).ToList().Count == 1);

			Assert.IsTrue(
				dataPool.Pool.Where(
						kv =>
							kv.Key == "Certifications_002" 
						&& ((string) kv.Value == cert2)).ToList().Count == 1);

			// Test salary history - key only
			Assert.IsTrue(
				dataPool.Pool.Where(
						kv =>
							kv.Key == "SalaryHistory_000").ToList().Count == 1);

			Assert.IsTrue(
				dataPool.Pool.Where(
						kv =>
							kv.Key == "SalaryHistory_001").ToList().Count == 1);

			// Mystery numbers ... well they were persisted using the DataContract Serializer 
			// and persisted as XML as a single table property.
			Assert.IsTrue(
				dataPool.Pool.Where(
						kv =>
							kv.Key == "MysteryNumbers").ToList().Count == 1);

			//----------------------------------------------------------------------------------------------------------
			// now delete the entity 

			client.Delete(
			            departmentDev,
			            dataWritten.EmployeeId);

			//----------------------------------------------------------------------------------------------------------
			// And verify that it was actually deleted
			// by attempting to read back the data 

			var queryable =	from	imp in client.CreateQuery()
			                where	imp.Department	== dataWritten.Department
			                    &&	imp.EmployeeId	== dataWritten.EmployeeId
			                select imp;

			Assert.IsTrue(queryable.ToList().Count == 0);

			//----------------------------------------------------------------------------------------------------------
			// So far so good. This concludes this part of the tutorial. Go Stash!
		}
	}
}
