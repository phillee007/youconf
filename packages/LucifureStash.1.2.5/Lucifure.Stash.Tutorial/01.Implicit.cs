using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeSuperior.Lucifure.Tutorial
{
	// -----------------------------------------------------------------------------------------------------------------
	// Implicit mode is supported to provide a measure of backward compatibility with the Microsoft Table Client.
	// Only PROPERTIES of a class with PUBLIC GET and PUBLIC SET, is implicitly supported.
	// Only AZURE table supported data types are implicitly supported.
	// Int32, Int64, Double, String, Boolean, DateTime, Guid & Byte[].
	// -----------------------------------------------------------------------------------------------------------------

	// -----------------------------------------------------------------------------------------------------------------
	// Define a Job Applicant class and expose a few properties.
	// Note: It is not necessary to inherit from a base class. However, if you want to encapsulate the partition and row
	// key you can do so. 
	// No base class is provided because Lucifure Stash allows, as seen later, the partition and 
	// row key to be named as desired.
	// -----------------------------------------------------------------------------------------------------------------

	public 
	class JobApplicant
	{
			// At least a 'PartitionKey' and 'RowKey" of type string must be defined 
			public 
			string								PartitionKey		{ get; set; }

			public 
			string								RowKey				{ get; set; }
	
			// A 'Timestamp' field is optional. If specified, the value is not sent but is received.
			public
			DateTime							Timestamp			{ get; set; }

			
			// Additional Properties
			public
			string								Name				{ get; set; }

			public 
			int									Skill_Level			{ get; set; }

#region Orthogonal Support Code
		// Override the Equals so we can use it to validate our tests
		public
		override
		bool
		Equals(
			object								obj)
		{
			JobApplicant							rhs;

			return		Object.ReferenceEquals(this, obj)
					||	
						(((rhs = obj as JobApplicant) != null) 
							&& PartitionKey == rhs.PartitionKey
							&& RowKey		==	rhs.RowKey
							&& Name			== rhs.Name
							&& Skill_Level	== rhs.Skill_Level);
		}

		// Override GetHashCode too, although we do not expect to use it
		public
		override
		int
		GetHashCode()
		{
			return (PartitionKey + RowKey).GetHashCode();
		}
#endregion x
	}


	// -----------------------------------------------------------------------------------------------------------------

	[TestClass]
	public 
	class TestImplicit
	{
		[TestMethod]
		public 
		void 
		Tutorial_01_Implicit_JobApplicant() 
		{
			//----------------------------------------------------------------------------------------------------------
			// Create the Stash Client. 
			// Our helper class offers various flavors of this. 

			StashClient<JobApplicant>
			client = StashConfiguration.GetClient<JobApplicant>();		
		
			//----------------------------------------------------------------------------------------------------------
			// Create the corresponding table or confirm that it exists
			// The table name can be inferred using various methods. In this case it is inferred from the class name, 
			// 'JobApplicant'.

			client.CreateTableIfNotExist();

			//----------------------------------------------------------------------------------------------------------
			// Create an instance of the class we want to Stash
			
			JobApplicant
			dataWritten = new JobApplicant {
							PartitionKey	= "A",
							RowKey			= Guid.NewGuid().ToString(),
							Name			= "John Doe",
							Skill_Level		= 1 };

			//----------------------------------------------------------------------------------------------------------
			// Stash it
			
			client.Insert(dataWritten);

			//----------------------------------------------------------------------------------------------------------
			// Read back the data, using the partition and row key, no need to use LINQ here.
			
			JobApplicant
			dataRead = client.Get(dataWritten.PartitionKey, dataWritten.RowKey); 

			//----------------------------------------------------------------------------------------------------------
			// Verify we got back what we put in
			
			Assert.IsTrue(dataWritten.Equals(dataRead));

			//----------------------------------------------------------------------------------------------------------
			// Lets change the SkillLevel and Update 

			dataWritten.Skill_Level += 1;
				
			client.Update(dataWritten);	

			//----------------------------------------------------------------------------------------------------------
			// Read back the data but this time lets use LINQ

			dataRead = client.CreateQuery()
								.Where(imp =>		imp.PartitionKey	== dataWritten.PartitionKey 
												&&	imp.RowKey			== dataWritten.RowKey)
								.FirstOrDefault();

			//----------------------------------------------------------------------------------------------------------
			// Again verify that we got back what we put in
			
			Assert.IsTrue(dataWritten.Equals(dataRead));

			//----------------------------------------------------------------------------------------------------------
			// now delete the entity

			client.Delete(dataWritten);

			//----------------------------------------------------------------------------------------------------------
			// And verify that it was actually deleted
			// by attempting to read back the data 

			var queryable =	from	imp in client.CreateQuery()
							where	imp.PartitionKey	== dataWritten.PartitionKey 
								&&	imp.RowKey			== dataWritten.RowKey
							select imp;

			Assert.IsTrue(queryable.ToList().Count == 0);

			//----------------------------------------------------------------------------------------------------------
			// So far so good. This concludes this part of the tutorial. Go Stash!
		}
	}

}
