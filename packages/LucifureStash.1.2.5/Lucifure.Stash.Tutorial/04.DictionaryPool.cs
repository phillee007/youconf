using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeSuperior.Lucifure.Tutorial
{
	// -----------------------------------------------------------------------------------------------------------------
	// Using a dictionary pool allows a flexible way to query an Azure table, the structure of which is unknown or 
	// flexible.
	// -----------------------------------------------------------------------------------------------------------------

	// -----------------------------------------------------------------------------------------------------------------
	// Define a generic pool class with the usual suspects and include a dictionary member duly annotated.
	// Note: A pool member must be defined as a type which implements an IDictionary<string, object> interface.
	// Only one pool per class is supported.
	// -----------------------------------------------------------------------------------------------------------------

    [StashEntity(Mode=StashMode.Explicit)]
    public 
    class GenericPool
    {
			// lets give the partition, row key and timestamp more meaningful names.
			[StashPartitionKey]
			public 
			string								PrimaryKey;

			[StashRowKey]
			public 
			string								SecondaryKey;

			[StashTimestamp]
			DateTime							AzureInternal;
	
			[StashPool]
			public 
			Dictionary<string, object>			Pool;
	}

	// -----------------------------------------------------------------------------------------------------------------

	[TestClass]
	public 
	class TestDictionaryPool
	{
		[TestMethod]
		public 
		void 
		Tutorial_04_DictionaryPool() 
		{
			//----------------------------------------------------------------------------------------------------------
			// Create a Stash Client for the Employee class

            StashClient<Employee>
            clientEmployee = StashConfiguration.GetClient<Employee>();		

			clientEmployee.CreateTableIfNotExist();

			//----------------------------------------------------------------------------------------------------------
			// Create an StashClient for the generic pool class, 
			// Since we want to read and write to the Employee table, notice how we setup the correct EntityName in 
			// the StashClientOptions. Otherwise the azure table implied would be "GenericPool".
			//
			// The call back method of naming the table has flexible capabilities. It can be used to determine 
			// the table name dynamically based on the data in the object instance. 
			// In this case it is used statically and called only once.

			StashClient<GenericPool>
			clientGenericPool = StashConfiguration.GetClient<GenericPool>(
									new StashClientOptions {
											OverrideEntitySetName			= o => "Employee",
											OverrideEntitySetNameIsDynamic	= false,
											Feedback						= StashConfiguration.TraceFeedback,
										});		
		
			//----------------------------------------------------------------------------------------------------------
			// Stash an Employee 
			
			const
			string departmentDev = "Dev";

            Employee
            dataEmployee = new Employee {
                            Department	= departmentDev,
                            EmployeeId	= Guid.NewGuid().ToString(),
                            Name		= "John Doe",
                            SkillLevel	= 8,
							DateOfBirth	= new DateTime(1990, 1, 1) };

			dataEmployee.SetAnnualSalary();

            clientEmployee.Insert(dataEmployee);

			//----------------------------------------------------------------------------------------------------------
			// and read it back thru the GenericPool
			
			GenericPool
			dataPool = clientGenericPool.Get(
											dataEmployee.Department, 
											dataEmployee.EmployeeId); 

			//----------------------------------------------------------------------------------------------------------
			// Verify we got back what we put in via the Employee type.
			
			Assert.IsTrue(
						dataPool.PrimaryKey				== dataEmployee.Department
					&&	dataPool.SecondaryKey			== dataEmployee.EmployeeId
					&&	dataPool.Pool["AnnualSalary"]	.Equals(dataEmployee.GetAnnualSalary())
					&&	dataPool.Pool["Birthday"]		.Equals(dataEmployee.DateOfBirth)
					&&	dataPool.Pool["SkillLevel"]		.Equals(dataEmployee.SkillLevel)
					&&	dataPool.Pool["Name"]			.Equals(dataEmployee.Name)
					&&	dataPool.Pool.Count				== 4 + 1);	// + 1 to the ETag

			//----------------------------------------------------------------------------------------------------------
			// Make a few changes and update

			dataPool.Pool["Name"]	= "Lucifure";			// name change
			dataPool.Pool["Unique"] = Guid.NewGuid();		// new field

			clientGenericPool.Update(dataPool);	

			// Note: Updates, Merges etc keep the original objects in sync with the latest ETags.
			// Even if an ETag is not specified for the type, if the type supports a Pool,
			// the ETag is maintained and synched in the pool

			//----------------------------------------------------------------------------------------------------------
			// Read back the data 

			GenericPool
			dataPoolRead = clientGenericPool.CreateQuery()
			                    .Where(imp =>		imp.PrimaryKey	== dataEmployee.Department 
												&&	imp.SecondaryKey == dataEmployee.EmployeeId)
			                    .FirstOrDefault();

			//----------------------------------------------------------------------------------------------------------
			// and verify that we got back what we put in
			
			Assert.IsTrue(
						dataPool.PrimaryKey			== dataPoolRead.PrimaryKey
					&&	dataPool.SecondaryKey		== dataPoolRead.SecondaryKey
					&& StashHelper.DictionaryEquals(
												dataPool.Pool, 
												dataPoolRead.Pool));

			//----------------------------------------------------------------------------------------------------------
			// Merge can be performed very efficiently with a StashPool. Only include the tables properties 
			// to be merge in the dictionary. No need to null out members or defined nullable value type members.

			// Create a new pool with just the objects of interest
			Dictionary<string,object>
			pool = new Dictionary<string,object>();

			// save the current etag (for now and later)
			string
			etagPreMerge = dataPoolRead.Pool[Literal.ETag].ToString();

			pool["Name"]		= "Stash";									// new value to merge
			pool[Literal.ETag]	= etagPreMerge;								// get the ETag too

			// create a new pool object for merging.
			GenericPool
			dataPoolMerged = new GenericPool {
									PrimaryKey = dataPoolRead.PrimaryKey,
									SecondaryKey	=	dataPoolRead.SecondaryKey,
									Pool			=	pool };

			clientGenericPool.Merge(dataPoolMerged, ETagMatch.Must);		// force ETag matching.

			// read back and verify
			GenericPool
			dataPoolMergedRead = clientGenericPool.Get(
											dataPoolMerged.PrimaryKey, 
											dataPoolMerged.SecondaryKey); 

			// validate
			Assert.IsTrue(dataPoolMergedRead.Pool.Count == dataPoolRead.Pool.Count 
							&&	dataPoolMergedRead.Pool["Name"] as String == "Stash");

			//----------------------------------------------------------------------------------------------------------
			// attempt to merge again, replacing the current ETag with the old one. 

			bool								isSuccess;

			try 
			{
				// use the defunct etag to exhibit this 
				dataPoolMerged.Pool[Literal.ETag] = etagPreMerge;

				clientGenericPool.Merge(dataPoolMerged);	
			
				isSuccess = false;
			}
			catch (Exception ex)
			{
				// Merge fails because the ETag was old and did not match.
				Assert.IsTrue((ex as StashException).Error == StashError.ETagMatchFailed);

				isSuccess = true;
			}

			Assert.IsTrue(isSuccess);

			//----------------------------------------------------------------------------------------------------------
			// attempt to merge yet again, this time by disabling ETag matching. 

			clientGenericPool.Merge(dataPoolMerged, ETagMatch.Unconditional);

			//----------------------------------------------------------------------------------------------------------
			// now attempt to delete the entity

			try 
			{
				clientGenericPool.Delete(dataPoolMergedRead);		// Implicit ETag matching, will make this fail too
																	// because the last update changed the ETag
				isSuccess = false;
			}
			catch (Exception ex)
			{
				Assert.IsTrue((ex as StashException).Error == StashError.ETagMatchFailed);

				isSuccess = true;
			}

			Assert.IsTrue(isSuccess);

			//----------------------------------------------------------------------------------------------------------
			// Do an unconditional delete

			clientGenericPool.DeleteUnconditional(dataPoolMergedRead);

			//----------------------------------------------------------------------------------------------------------
			// And verify that it was actually deleted
			// by attempting to read back the data 

			var queryable =	from	emp in clientEmployee.CreateQuery()
							where	emp.Department	== dataEmployee.Department
			                    &&	emp.EmployeeId	== dataEmployee.EmployeeId
			                select emp;

			Assert.IsTrue(queryable.ToList().Count == 0);

			//----------------------------------------------------------------------------------------------------------
			// So far so good. This concludes this part of the tutorial. Go Stash!
		}
	}

}
