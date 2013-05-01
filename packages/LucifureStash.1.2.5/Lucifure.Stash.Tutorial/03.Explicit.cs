using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeSuperior.Lucifure.Tutorial
{
	// -----------------------------------------------------------------------------------------------------------------
	// Explicit mode allows purely opt-in capabilities.
	// Any FIELD or PROPERTY (with get/set) with any access modifier PUBLIC, PRIVATE, PROTECTED in INTERNAL can be Stashed.
	// In addition to AZURE table supported data types, other data type can be supported
	// -----------------------------------------------------------------------------------------------------------------

	// -----------------------------------------------------------------------------------------------------------------
	// Create a new class and mark it as Explicit.
	// -----------------------------------------------------------------------------------------------------------------

    [StashEntity(Mode=StashMode.Explicit)]
    public 
    class Employee
    {
			// Give the partition, row key and timestamp more meaningful names
			[StashPartitionKey]
			public 
			string								Department;

			[StashRowKey]
			public 
			string								EmployeeId;

			[StashTimestamp]
			DateTime							AzureInternal;

			// Provide an ETag so that we can have optimistic concurrency support.
			[StashETag]
			public
			string								ETagInternal;

			[Stash]
			public
			string								Name				{ get; set; }

			[Stash]
			public 
			int									SkillLevel			{ get; set; }

            [Stash(Name="Birthday")]
            public 
            DateTime							DateOfBirth;

			[Stash(Name="AnnualSalary")]
            double								_annualSalary		{ get; set; }


			public
			void
			SetAnnualSalary()
			{
				_annualSalary = 50000 + (SkillLevel * 8000);
			}			
			
			public
			double
			GetAnnualSalary()
			{
				return _annualSalary;
			}	

#region Orthogonal Support Code
        // Override the Equals so we can use it to validate our tests
        public
        override
        bool
        Equals(
            object								obj)
        {
            Employee							rhs;

            return	(Object.ReferenceEquals(this, obj)
                        ||	(((rhs = obj as Employee) != null) 
                                && Department		== rhs.Department)
                                && EmployeeId		== rhs.EmployeeId
                                && Name				== rhs.Name
                                && SkillLevel		== rhs.SkillLevel
								&& _annualSalary	== rhs._annualSalary);
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
    class TestExplicit
    {
        [TestMethod]
        public 
        void 
        Tutorial_03_Explicit_Employee() 
        {
			//----------------------------------------------------------------------------------------------------------
			// Create the Stash Client. 

            StashClient<Employee>
            client = StashConfiguration.GetClient<Employee>();		
		
            //----------------------------------------------------------------------------------------------------------
            // Create the underlying table if it does not already exists

            client.CreateTableIfNotExist();

            //----------------------------------------------------------------------------------------------------------
            // Create an instance of the class 
			
			const 
			string departmentDev = "Dev";

            Employee
            dataWritten = new Employee {
                            Department	= departmentDev,
                            EmployeeId	= Guid.NewGuid().ToString(),
                            Name		= "John Doe",
                            SkillLevel	= 8,
							DateOfBirth = new DateTime(1990, 1, 1) };

			// update the private field
			dataWritten.SetAnnualSalary();

            //----------------------------------------------------------------------------------------------------------
            // Stash it			 
			
            client.Insert(dataWritten);		

			// Note: On inserts, updates and merges, the ETag is overwritten from the ETag returned to keep the data
			// in sync.

            //----------------------------------------------------------------------------------------------------------
            // And read it back 
			
            Employee
            dataRead = client.Get(dataWritten.Department, dataWritten.EmployeeId); 

            //----------------------------------------------------------------------------------------------------------
            // Verify we got back what we put in
			
            Assert.IsTrue(dataWritten.Equals(dataRead));

            // Verify that the 2 ETags are identical
            Assert.IsTrue(dataRead.ETagInternal == dataWritten.ETagInternal);

            //----------------------------------------------------------------------------------------------------------
            // Change the Skill level and update the Stash

            dataWritten.SkillLevel += 1;
			
			dataWritten.SetAnnualSalary();		

            client.Update(dataWritten);	

            //----------------------------------------------------------------------------------------------------------
	        // Read back the data but this time lets use LINQ

			Employee
            dataReadUpdated = client.CreateQuery()
									.Where(imp =>	imp.Department == departmentDev 
										         &&	imp.EmployeeId == dataWritten.EmployeeId)
									.FirstOrDefault();

            //----------------------------------------------------------------------------------------------------------
            // Again verify that we got back what we put in
			
            Assert.IsTrue(dataWritten.Equals(dataReadUpdated));

            // Verify that the 2 ETags are identical
            Assert.IsTrue(dataWritten.ETagInternal == dataReadUpdated.ETagInternal);

            //----------------------------------------------------------------------------------------------------------
			// now attempt to update the data read the first time around
			// this should fail and throw an exception since the data was updated since the row was last read and 
			// so has a stale ETag.
			// Uses the implied update mode here by default. That is, if the StashETag attribute is applied to a 
			// member in the class, implies ETag must match,

			bool								isSuccess;

			try 
			{
				client.Update(dataRead);	
			
				isSuccess = false;
			}
			catch (Exception ex)
			{
				Assert.IsTrue((ex as StashException).Error == StashError.ETagMatchFailed);

				isSuccess = true;
			}

			Assert.IsTrue(isSuccess);

            //----------------------------------------------------------------------------------------------------------
			// attempt to update it again but this time ignore ETag matching. This should succeed.
			// and dataRead has the latest ETag

            client.UpdateUnconditional(dataRead);	
			
            //----------------------------------------------------------------------------------------------------------
            // now attempt to delete the entity using the data written entity
			// this should fail and throw an exception since the data was recently updated and so has a new ETag
			// Uses the implied update mode here by default. That is, if the StashETag attribute is applied to a 
			// member in the class, implies ETag must match,

			try
            {
				client.Delete(dataReadUpdated);

				isSuccess = false;
			}
			catch (Exception ex)
			{
				Assert.IsTrue((ex as StashException).Error == StashError.ETagMatchFailed);

				isSuccess = true;
			}

			Assert.IsTrue(isSuccess);

            //----------------------------------------------------------------------------------------------------------
            // now delete the entity unconditionally without the need for ETag Matching.
			// Can instead also use client.DeleteUnconditional here too.

            client.Delete(dataRead, ETagMatch.Unconditional);

            //----------------------------------------------------------------------------------------------------------
			// And verify that it was actually deleted
			// by attempting to read back the data 

            var queryable =	from	imp in client.CreateQuery()
                            where	imp.Department	== dataWritten.Department
                                &&	imp.EmployeeId	== dataWritten.EmployeeId
                            select imp;

            Assert.IsTrue(queryable.ToList().Count == 0);

            //----------------------------------------------------------------------------------------------------------
			// Essentially the ETag gist is this.
			// Define and decorate a member in your type with an ETag if you want to optimistic concurrency support.
			//   Stash will keep you ETag in sync across inserts, updates and merges.
			//   There are multiple ways to implicitly override ETag matching if that is what is wanted.
			// The implicit way to disable optimistic concurrency support is not to define an ETag member.

            //----------------------------------------------------------------------------------------------------------
			// So far so good. This concludes this part of the tutorial. Go Stash!
		}
	}
}
