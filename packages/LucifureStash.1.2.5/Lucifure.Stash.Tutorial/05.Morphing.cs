using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeSuperior.Lucifure.Tutorial
{
	// -----------------------------------------------------------------------------------------------------------------
	// Morphing provides the capability to transform an object from one state to another, including changing the 
	// type of the object.
	//
	// Morphing can be used for many purposes. 
	//
	// For example, to store a non-primitive data type, that is, a data type not supported by Azure Tables. 
	//
	// Implicit default morphs are provided for byte, sbyte, char, int16, uint16, uint32 which morph to int32; 
	// and for uint64 which morphs to int64. 
	//
	// Implicit default morphs are also provided to morph Enums appropriately.
	// 
	// If a morpher is not specified and the data type is not an Azure primitive or is not implicitly morphed,  
	// the StashClient defaults to serializing the type into an XML string using the DataContractSerializer.
	//
	// One can always override this default behavior by providing a type which implements IMorph.
	// -----------------------------------------------------------------------------------------------------------------

	// -----------------------------------------------------------------------------------------------------------------
	// This helper Morpher class implements IMorph and provides encryption and decryption, effectively morphing 
	// a plain text string into a byte[] and back.
	// Note: A Morpher must have a public default constructor so that we can create an instance of it.
	// -----------------------------------------------------------------------------------------------------------------

	public
	class MorphEncrypter					:	IStashMorph
	{
			// create the underlying encrypter class and initialize with encryption key
			static 
			Encrypter							_encrypter = new Encrypter("Lucifure", "Stash");

		// IMorph interface implementation
		public 
        bool
		CanMorph(
			Type								type)
		{
			return type == typeof(string);
		}

		public 
        bool
        IsCollationEquivalent
		{
			get { return true; }	// indicates that the collating sequence of the original and morphed values are identical  
		}

		public 
		object 
		Into(
			object								value) 
		{
			return _encrypter.Encrypt((string) value);				// convert string to byte[]
		}

		public 
		object 
		Outof(
			object								value) 
		{
			return _encrypter.DecryptToString((byte[]) value);		// convert byte[] back to string	
		}
	}

	// -----------------------------------------------------------------------------------------------------------------
	// Define an enum.
	// We can directly stash an enum value instead of a numeric value and having to cast it around.
	// -----------------------------------------------------------------------------------------------------------------

	public 
	enum Salutation : ushort	
	// stash supports the gamut of all possible of enum base types and Morphs to Int32 to Int64 if necessary.
	{
		Mr,
		Ms,
		Dr,
	}

	// -----------------------------------------------------------------------------------------------------------------
	// Extend the Employee class by adding a member to hold the employees social security number.
	// For security purposes we want to encrypt this at rest, so we pass in the MorphEncrypter type.
	// -----------------------------------------------------------------------------------------------------------------
	
    [StashEntity(Mode=StashMode.Explicit)]
    public 
    class EmployeeInfo						:	Employee							
    {
			// demonstrate morphing
			[Stash(Morpher=typeof(MorphEncrypter))]
			public 
			string								SSN;

			// demonstrate enum support
			[Stash]
			public 
			Salutation							Salutation { get; set; }

#region Orthogonal Support Code
        // Override the Equals so we can use it to validate our tests
        public
        override
        bool
        Equals(
            object								obj)
        {
            EmployeeInfo						rhs;

            return	(Object.ReferenceEquals(this, obj)
                        ||	(((rhs = obj as EmployeeInfo) != null) 
								&& base.Equals(rhs)
                                && SSN				== rhs.SSN)
                                && Salutation		== rhs.Salutation);
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
    class TestMorphing
    {
        [TestMethod]
        public 
        void 
        Tutorial_05_Morphing_EmployeeInfo() 
        {
			//----------------------------------------------------------------------------------------------------------
			// Create a Stash Client

            StashClient<EmployeeInfo>
            client = StashConfiguration.GetClient<EmployeeInfo>();		

			client.CreateTableIfNotExist();

			//----------------------------------------------------------------------------------------------------------
			// Create an instance of the class we what to Stash
			
			const 
			string departmentDev	= "Dev";

			const
			string ssn				= "123456789";

			EmployeeInfo
			dataWritten = new EmployeeInfo {
			                Department	= departmentDev,
			                EmployeeId	= Guid.NewGuid().ToString(),
			                Name		= "John Doe",
			                SkillLevel	= 8,
			                DateOfBirth	= new DateTime(1990, 1, 1),
							SSN			= ssn,
							Salutation	= Salutation.Dr };

			// update the private field
			dataWritten.SetAnnualSalary();

			//----------------------------------------------------------------------------------------------------------
			// Stash it			 
			
			client.Insert(dataWritten);

			//----------------------------------------------------------------------------------------------------------
			// Read back the data
			
			EmployeeInfo
			dataRead = client.Get(dataWritten.Department, dataWritten.EmployeeId); 

			//----------------------------------------------------------------------------------------------------------
			// Verify we got back what we put in
			
			Assert.IsTrue(dataWritten.Equals(dataRead));

			//----------------------------------------------------------------------------------------------------------
			// Now let us verify exactly what was written to table storage by using the Generic Pool class we wrote 
			// earlier.

			// set this up to target the correct table storage entity set
			StashClient<GenericPool>
			clientGenericPool = StashConfiguration.GetClient<GenericPool>(
									new StashClientOptions {
											OverrideEntitySetName			= o => "EmployeeInfo",
											OverrideEntitySetNameIsDynamic	= false,
											Feedback						= StashConfiguration.TraceFeedback,
										});		


			GenericPool
			dataPool = clientGenericPool.Get(
											dataRead.Department, 
											dataRead.EmployeeId); 

			// validate that the SSN value got morphed in to a byte[]
			Assert.IsTrue(dataPool.Pool["SSN"].GetType() == typeof(byte[]));

			//----------------------------------------------------------------------------------------------------------
			// Change the Skill level and update the Stash

			dataWritten.SkillLevel += 1;
			
			dataWritten.SetAnnualSalary();		

			// force an unconditional update since, dataWritten does not have a ETag.
			// Optionally copy over the ETag from dataRead.
			client.Update(dataWritten, ETagMatch.Unconditional);	

			//----------------------------------------------------------------------------------------------------------
			// Read back the data but this time lets use LINQ

			dataRead = client.CreateQuery()
			                    .Where(imp =>	imp.Department == departmentDev 
			                                 &&	imp.EmployeeId == dataWritten.EmployeeId)
			                    .FirstOrDefault();

			//----------------------------------------------------------------------------------------------------------
			// Again verify that we got back what we put in
			
			Assert.IsTrue(dataWritten.Equals(dataRead));

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
