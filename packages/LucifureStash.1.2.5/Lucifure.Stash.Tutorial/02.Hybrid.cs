using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeSuperior.Lucifure.Tutorial
{
	// -----------------------------------------------------------------------------------------------------------------
	// Explicitly adding StashAttributes to your entity class allow additional capabilities.
	// Any FIELD or PROPERTY with any access modifier PUBLIC, PRIVATE, PROTECTED in INTERNAL can be Stashed.
	// In addition to AZURE table supported data types, other data type can be supported
	// 
	// This example, shows a hybrid mode. This mode retains all the implicitly supported public properties, and    
	// allows additional fields and properties to be enabled for Stashing.
	// -----------------------------------------------------------------------------------------------------------------

	// -----------------------------------------------------------------------------------------------------------------
	// Inherit from the JobApplicant class and add a few more properties decorating them with Stash attributes.
	// -----------------------------------------------------------------------------------------------------------------

	// -----------------------------------------------------------------------------------------------------------------
	// Use an attribute is to change the name of the table to which this class Stashes to.
	// The StashEntity attribute had a default mode of Hybrid. (Mode=StashMode.Hybrid.)
	// -----------------------------------------------------------------------------------------------------------------

    [StashEntity(Name="TempEmployee")]
    public 
    class TempHire							:	JobApplicant
    {
			// Change the name of the column this field Stashes to.
            [Stash(Name="Birthday")]
            public 
            DateTime							DateOfBirth;

			// We do not want to expose this property. So keep it private.
			[Stash]
            double								HourlyPay					{ get; set; }

			public
			void
			SetHourlyPay()
			{
				HourlyPay = 7.52 + (Skill_Level * 3.11);
			}				

#region Orthogonal Support Code
        // Override the Equals so we can use it to validate our tests
        public
        override
        bool
        Equals(
            object								obj)
        {
            TempHire							rhs;

            return		Object.ReferenceEquals(this, obj)
					||	(((rhs = obj as TempHire) != null) 
                            && (base.Equals(rhs))
                            && HourlyPay	== rhs.HourlyPay
                            && DateOfBirth	== rhs.DateOfBirth);
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
    class TestHybrid
    {
        [TestMethod]
        public 
        void 
        Tutorial_02_Hybrid_TempHire() 
        {
			//----------------------------------------------------------------------------------------------------------
			// Create the Stash Client. 

            StashClient<TempHire>
            client = StashConfiguration.GetClient<TempHire>();		
		
            //----------------------------------------------------------------------------------------------------------
            // Create a table with the class name ('TempEmployee') or confirm that it exists

            client.CreateTableIfNotExist();

            //----------------------------------------------------------------------------------------------------------
            // Create an instance of the class we what to Stash
			
            TempHire
            dataWritten = new TempHire {
                            PartitionKey	= "B",
                            RowKey			= Guid.NewGuid().ToString(),
                            Name			= "Jill Doe",
                            Skill_Level		= 5,
							DateOfBirth		= new DateTime(1990, 1, 1) };

			// update the private field
			dataWritten.SetHourlyPay();

            //----------------------------------------------------------------------------------------------------------
            // Stash it			 
			
            client.Insert(dataWritten);

            //----------------------------------------------------------------------------------------------------------
            // Read back the data
			
            TempHire
            dataRead = client.Get(dataWritten.PartitionKey, dataWritten.RowKey); 

            //----------------------------------------------------------------------------------------------------------
            // Verify we got back what we put in
			
            Assert.IsTrue(dataWritten.Equals(dataRead));

            //----------------------------------------------------------------------------------------------------------
            // Lets change the Skill level and update the Stash

            dataWritten.Skill_Level += 1;
			
			dataWritten.SetHourlyPay();		

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

            client.Delete(
						dataWritten.PartitionKey,
						dataWritten.RowKey);

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
