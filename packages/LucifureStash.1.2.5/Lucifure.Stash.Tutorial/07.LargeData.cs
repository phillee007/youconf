using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeSuperior.Lucifure.Tutorial
{
	// -----------------------------------------------------------------------------------------------------------------
	// Azure storage limits a table property to a size of 64K bytes.
	// Lucifure Stash removes this limitation for string and byte[] and will transparently split both these data type
	// into valid sized chunks on writes and merge them back on reads.
	// -----------------------------------------------------------------------------------------------------------------

	// -----------------------------------------------------------------------------------------------------------------
	// Define some large data 

    [StashEntity(Mode=StashMode.Explicit)]
    public 
    class EmployeeResume
    {
			[StashPartitionKey]
			public 
			string								Department;

			[StashRowKey]
			public 
			string								EmployeeId;

			[Stash]
			public
			string								Text;

			[Stash]
			public
			byte[]								Photograph;


#region Orthogonal Support Code
        // Override the Equals so we can use it to validate our tests
        public
        override
        bool
        Equals(
            object								obj)
        {
            EmployeeResume						rhs;

            return	(Object.ReferenceEquals(this, obj)
                        ||	(((rhs = obj as EmployeeResume) != null) 
								&& Department == rhs.Department
								&& EmployeeId == rhs.EmployeeId
								&& Text == rhs.Text
								&& Enumerable.SequenceEqual(Photograph, rhs.Photograph)));
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
    class TestLargeData
    {
        [TestMethod]
        public 
        void 
        Tutorial_07_LargeData() 
        {
			//----------------------------------------------------------------------------------------------------------
			// Create Stash Client 

            StashClient<EmployeeResume>
            client = StashConfiguration.GetClient<EmployeeResume>();		
		
			client.CreateTableIfNotExist();

			//----------------------------------------------------------------------------------------------------------
			// Lets create an instance of the class we what to 'stash'
			
			const 
			string departmentDev = "Dev";

			EmployeeResume
			dataWritten = new EmployeeResume {
			                Department	= departmentDev,
			                EmployeeId	= Guid.NewGuid().ToString(),
							
							// 1 char = 2 bytes so this random string is 2.5 time the size of a table property
							Text		= BuildString((int) (32 * 1024 * 2.5)),	
							
							// almost 2 time the size of a table property
							Photograph	= new byte[(int) (64 * 1024 * 1.9)],
			                };

			// generate random bytes for the photograph
			(new Random()).NextBytes(dataWritten.Photograph);
			 
			//----------------------------------------------------------------------------------------------------------
			// Stash it			 
			
			client.Insert(dataWritten);

			//----------------------------------------------------------------------------------------------------------
			// Read back the data
			
			EmployeeResume
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
											OverrideEntitySetName			= o => typeof(EmployeeResume).Name,
											OverrideEntitySetNameIsDynamic	= false,
											Feedback						= StashConfiguration.TraceFeedback,
										});		


			GenericPool
			dataPool = clientGenericPool.Get(
											dataRead.Department, 
											dataRead.EmployeeId); 

			// All split items are suffixed with the index base A, in the format "_A". 
			// Because the "_" is used for demarcating collection indexes, explicit defined members are not allowed
			// to have the embedded "_" character in the table property name.

			// Test for Text
			Assert.IsTrue(
				dataPool.Pool.Where(
						kv =>
							kv.Key.IndexOf("Text_") == 0).ToList().Count == 3);

			Assert.IsTrue(
				dataPool.Pool.Where(
						kv =>
							kv.Key.IndexOf("Photograph_") == 0).ToList().Count == 2);

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

        static
        public
        string
        BuildString(
            int									len)
        {
			Random								rnd = new Random();

            StringBuilder
            sb = new StringBuilder();

            for (int i = 0; i < len; ++i)
                sb.Append((Char)rnd.Next(32, 128));

            return sb.ToString();
        }
	}
}
