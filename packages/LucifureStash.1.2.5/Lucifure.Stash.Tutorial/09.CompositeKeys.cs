using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeSuperior.Lucifure.Tutorial
{
	// -----------------------------------------------------------------------------------------------------------------
	// Lucifure Stash supports a powerful abstracting of using a composite key.
	// This feature is available for both the Partition and the Row key.
	//
	// Azure table storage really has a single primary key (and index). For scalability purposes it is conceptually 
	// split into 2 parts and the onus of how to split the key is placed on the table user, rather that with the 
	// service itself. This a good thing since the user is expected to have a better idea of the nature of the data 
	// and so determine the scalability strategy.
	//
	// In reality the primary key is invariably split into more that 2 segments and in practice they may not even 
	// natively be strings. 
	//
	// Lucifer Stash recognizes this and using a combination of morphing and query time smarts, makes working with
	// composite keys trivial and intuitive for the end user working with the table.
	//
	// This tutorial also introduces another concept to enhance the performance of table storage access, when
	// multiple versions of a row exists and only the latest one needs to be retrieved. 
	//
	// -----------------------------------------------------------------------------------------------------------------
	//
	// Lets says an employer wishes to store the compensation history of an employee over time. 
	// The employer most often only want to access the most recent value, but also wants the capability to graphing 
	// say the commission over the employment period of the employee.
	// 
	// The primary key decided upon on is [Employee Id][Compensation Type][Date].
	// In the Azure table storage schema it is decide to make the [Employee Id] the partition key and 
	// the [Compensation Type] and [Date] the composite row key.
	//
	// -----------------------------------------------------------------------------------------------------------------

	// -----------------------------------------------------------------------------------------------------------------
	// Define a class to hold our composite row key
	// The CompensationType and Date are our two segments.

	public 
	class CompensationEvent
	{
			public
			const
			int									KeySize		= 1 + 5; // one character + 5 for the number of days

			// define an end date for our reverse order storage needs. Explained later...
			static
			DateTime							_endDate = new DateTime(2099, 12, 31).ToUniversalTime();

			// ---------------------------------------------------------------------------------------------------------
			// Actual class data

			public
			string								CompensationType;

			DateTime							_date;

			public
			DateTime
			Date
			{
				get	{return _date; }
				set { _date = value.ToUniversalTime(); }
			}

		// -------------------------------------------------------------------------------------------------------------
		// Define the following 2 member for the morpher to use, since we need to convert this key to a string
		public
		override
		string 
		ToString()
		{
			// compose the key as the 1 character compensation type + the date
			return CompensationType + DateToString();
		}

		public
		static
		CompensationEvent
		Parse(
			string								value)
		{
			// parse the key into its components. Note: Error handling not implemented for the sake of brevity
			return new CompensationEvent 
							{
								CompensationType	=	value.Substring(0, 1),
								// reverse the process to convert the number of days delta to the correct date
								Date				=	StringToDate(value.Substring(1))	
							};
		}							

		string
		DateToString()
		{
			// The number of day between our end date and the date.
			//
			// Important: We morph the date as the number of days from a future date so that the latest date has
			// the smallest value and so is the first row in Azure's ascending collating order.
			//
			// We only returns the number of days, if a date has been specified. 
			// This is important for composite key querying to work. 
			//
			return	Date != DateTime.MinValue
						?	((int)(_endDate - Date.Date).TotalDays).ToString("00000")
						:	"";
		}

		static
		DateTime
		StringToDate(
			string								value)
		{
			
			return	!String.IsNullOrEmpty(value)					// if a value has been specified
						?	_endDate.AddDays(-Int32.Parse(value))	// subtract the number of days from the end date
						:	DateTime.MinValue;						// if no values is specified, return the minimum date
		}

#region Orthogonal Support Code
        // Override the Equals so we can use it to validate our tests
        public
        override
        bool
        Equals(
            object								obj)
        {
            CompensationEvent					rhs;

            return	(Object.ReferenceEquals(this, obj)
                        ||	(((rhs = obj as CompensationEvent) != null) 
								&& CompensationType == rhs.CompensationType
								&& Date.Date == rhs.Date.Date					// we are only interested in the date
								));												// and not the time
        }

        // Override GetHashCode too, although we do not expect to use it
        public
        override
        int
        GetHashCode()
        {
			return (CompensationType.ToString() + Date.ToString()).GetHashCode();
        }

		// declare these too so that we can use them in the query
		public 
		static 
		bool operator >=(
			CompensationEvent					lhs,
			CompensationEvent					rhs)
		{
			return lhs._date >= rhs._date;
		}

		public 
		static 
		bool operator <=(
			CompensationEvent					lhs,
			CompensationEvent					rhs)
		{
			return lhs._date <= rhs._date;
		}

		public 
		static 
		bool operator <(
			CompensationEvent					lhs,
			CompensationEvent					rhs)
		{
			return lhs._date < rhs._date;
		}

		public 
		static 
		bool operator >(
			CompensationEvent					lhs,
			CompensationEvent					rhs)
		{
			return lhs._date > rhs._date;
		}

#endregion x

	}

	// -----------------------------------------------------------------------------------------------------------------
	// Declare some constants

	public 
	static
	class CompensatationTypeValue
	{
		public 
		static 
		string								Salary		= "S",
											Bonus		= "B",
											Commission	= "C";
	}

	// -----------------------------------------------------------------------------------------------------------------
	// Define a morpher for the Employee Id Guid. 
	//This is a simple morpher for a Guid, which can be reused elsewhere too.

	class MorphGuid							:	IStashMorph
	{
		public 
		bool  
		CanMorph(
			Type								type)
		{
 			return type == typeof(Guid);
		}

		public 
        bool
        IsCollationEquivalent
		{
			// Indicates that the collating sequence of the original and morphed values are identical,
			get { return true; }	
		}

		public 
		object  
		Into(
			object								value)
		{
			return ((Guid) value).ToString();
		}

		public 
		object  
		Outof(
			object								value)
		{
 			return Guid.Parse(value.ToString());
		}
	}



	// -----------------------------------------------------------------------------------------------------------------
	// Define a morpher for the CompensationEvent class. After all we need to convert from and to a string since
	// the azure row key needs to be a string.
	// This class really just called the methods in the CompensationEvent class, so is really simple. 

	class MorphCompensationEvent			:	IStashMorph, IStashKeyMediate
	{
		public 
		bool  
		CanMorph(
			Type								type)
		{
 			return type == typeof(CompensationEvent);
		}

		public 
        bool
        IsCollationEquivalent
		{
			// Indicates that the collating sequence of the original and morphed values are NOT the same,
			// because we actually order the date in reverse order.
			// (In most cases this is set to true, but in this case it is false.)
			get { return false; }
		}

		public 
		object  
		Into(
			object								value)
		{
			return ((CompensationEvent) value).ToString();
		}

		public 
		object  
		Outof(
			object								value)
		{
 			return CompensationEvent.Parse(value.ToString());
		}

		// IStashKeyMediate
		public 
		bool 
		IsCompleteKeyValue(
			string								value) 
		{
			// Called on query, return if the value supplied to the queue is the complete key or not.
			// Here this this down by comparing the key length because in this case, 
			//	the row key is always of the same length.
			// If the implementation had varying key lengths, the value could be parsed or searched for delimiters,
			//	to determine if it was a complete key value.
			return value.Length == CompensationEvent.KeySize;
		}
	}

	// -----------------------------------------------------------------------------------------------------------------
	// Define our Stash table 

    [StashEntity(Mode=StashMode.Explicit)]
    public 
    class EmployeeCompensation
    {
			// Both the keys are non-strings so we will use morphing
			[StashPartitionKey(Morpher=typeof(MorphGuid))]
			public 
			Guid								EmployeeId;
			
			// Why are we specifying the Key Mediator type?
			// The IStashKeyMediate interface is then available at query time to determine in the query value 
			//	supplied is the complete row key or not.
			// If not the completed row key, the query is modified appropriately.
			[StashRowKey(Morpher=typeof(MorphCompensationEvent), KeyMediator=typeof(MorphCompensationEvent))]
			public 
			CompensationEvent					CompDate;

			[Stash]
			public
			double								Amount;

#region Orthogonal Support Code
        // Override the Equals so we can use it to validate our tests
        public
        override
        bool
        Equals(
            object								obj)
        {
            EmployeeCompensation				rhs;

            return	(Object.ReferenceEquals(this, obj)
                        ||	(((rhs = obj as EmployeeCompensation) != null) 
								&& EmployeeId == rhs.EmployeeId
								&& CompDate == rhs.CompDate
								));
        }

        // Override GetHashCode too, although we do not expect to use it
        public
        override
        int
        GetHashCode()
        {
			return (EmployeeId.ToString() + CompDate.ToString()).GetHashCode();
        }
#endregion x
	}

    // -----------------------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------------------

    [TestClass]
    public 
    class TestCompositeKeys
    {
        [TestMethod]
        public 
        void 
        Tutorial_09_CompositeKeys() 
        {
			//----------------------------------------------------------------------------------------------------------
			// Create Stash Client 

            StashClient<EmployeeCompensation>
            client = StashConfiguration.GetClient<EmployeeCompensation>();		
		
			client.CreateTableIfNotExist();

			//----------------------------------------------------------------------------------------------------------
			// Populate the table with 10 years of salary information
			
			int 
			nbrOfYears = 10;

			DateTime 
			today = DateTime.Now.ToUniversalTime();

			DateTime
			hireDate = today.AddYears(-nbrOfYears);

			Guid
			employeeId = Guid.NewGuid();

			// Insert 10 years of data, such that each year the salary goes up by 5000.
			for(int yr = 0; yr < nbrOfYears; ++yr)
				client.Insert(
						new	EmployeeCompensation {
								EmployeeId	=	employeeId,
								CompDate	=	new CompensationEvent {
														CompensationType	= CompensatationTypeValue.Salary,
														Date				= hireDate.AddYears(yr)
													},
								Amount		=	65000 + (yr * 5000)
							});																

			//----------------------------------------------------------------------------------------------------------
			// Populate the table with the last 12 months of commission information
			
			int 
			nbrOfMths = 12;

			// Insert 12 months of data, such that each year the commission goes up by 50.
			for(int mth = 0; mth < nbrOfMths; ++mth)
				client.Insert(
						new	EmployeeCompensation {
								EmployeeId	=	employeeId,
								CompDate	=	new CompensationEvent {
														CompensationType	= CompensatationTypeValue.Commission,
														Date				= today.AddMonths(-mth)
													},
								Amount		=	1000 + (mth * 50)
							});																

			//----------------------------------------------------------------------------------------------------------
			// Get the last salary

			EmployeeCompensation
			employeeCompensation = 
				client
					.CreateQuery()
					.Where(e => 
									e.EmployeeId == employeeId 
								&&	e.CompDate == new CompensationEvent 
															{ CompensationType = CompensatationTypeValue.Salary })
									// no need to specify the latest date, we get the first row, which
									// has the latest date.
					.Take(1)		// first row
					.FirstOrDefault();

			// verify we got the last date and last salary amount
			Assert.IsTrue(employeeCompensation.CompDate.Date.Date	== hireDate.Date.AddYears((nbrOfYears -1)));
			Assert.IsTrue(employeeCompensation.Amount				== 65000 + ((nbrOfYears - 1) * 5000));

			// How does this work?
			// Stash converts the "Compensation Type == 'S'" into 
			// "RowKey >= 'S' and RowKey < 'T'" and asks to only return the first one (Take(1))
			// In this case the last dated event is the first row, since the date is morphed 
			// to the number of days before an arbitrary end date, such that the morphed date is 
			//	effectively in reverse order

			//----------------------------------------------------------------------------------------------------------
			// Get all the commissions
								 			
			List<EmployeeCompensation>
			commissions = 
				client
					.CreateQuery()
					.Where(e => 
									e.EmployeeId == employeeId 
								&&	e.CompDate == new CompensationEvent 
															{ CompensationType = CompensatationTypeValue.Commission })
									// no need to specify the date, we get all the commissions
									// for the employee because again the query mapped the '==' to a range '>= and <'.
					.ToList();

			Assert.IsTrue(commissions.Count == nbrOfMths);

			//----------------------------------------------------------------------------------------------------------
			// Get commissions for the 4 months, prior to the last 4 months
								 			
			commissions = 
				client
					.CreateQuery()
					.Where	(e => 
									e.EmployeeId == employeeId 
								&&	e.CompDate >= new CompensationEvent 
															{	CompensationType	= CompensatationTypeValue.Commission,
																Date				= today.AddDays(-5).AddMonths(-8)}
								&&	e.CompDate <= new CompensationEvent 
															{	CompensationType	= CompensatationTypeValue.Commission,
																Date				= today.AddDays(-5).AddMonths(-4)}
							)

					.ToList();

			Assert.IsTrue(commissions.Count == 4);

			// How does this work?
			// Stash converts the query from a '>= X && < Y' to 
			// < X && >= Y because the morpher indicated that 'IsCollationEquivalent' was false.
			// otherwise this query would have returned no rows because the range is inverted.  

			//----------------------------------------------------------------------------------------------------------
			
		}
	}
}