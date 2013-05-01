using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeSuperior.Lucifure.Tutorial
{
	// -----------------------------------------------------------------------------------------------------------------
	// Lucifure Stash supports multiple interactions with Table Storage via the use of a context similar to the 
	// Microsoft DataServiceContext.
	// Lucifure Stash goes a step further and supports multiple ways of committing data which is contained in the context.
	// -----------------------------------------------------------------------------------------------------------------

    [TestClass]
    public 
    class TestContext
    {
        [TestMethod]
        public 
        void 
        Tutorial_08_Context() 
        {
			//----------------------------------------------------------------------------------------------------------
			// Create Stash Client and get a new context

            StashClient<Employee>
            client = StashConfiguration.GetClient<Employee>();		
		
			client.CreateTableIfNotExist();
			
			StashContext<Employee>
			context = client.GetContext();

			//----------------------------------------------------------------------------------------------------------
            // Create instances of the class, place in the context and commit the changes.

			const 
			int insertCount = 10;
			
			string departmentDev = Guid.NewGuid().ToString();

			// create n employees and insert into the context
			Enumerable
				.Range(1, insertCount)
				.Select(
					idx => 
						{
							Employee 
							employee =	new Employee {
											Department	= departmentDev,
											EmployeeId	= Guid.NewGuid().ToString(),
											Name		= "John Doe",
											SkillLevel	= 8,
											DateOfBirth = new DateTime(1990 - idx, 1, 1) };

							// update the private field
							employee.SetAnnualSalary();

							return employee;
						} )
				.ToList()
				.ForEach(
					item => context.Insert(item));	
		
			// validate the the context now contains n items with the correct state.
			int contextInsertCount = context
										.GetTrackedEntities(EntityState.Inserted)
										.Count;

			Assert.IsTrue(contextInsertCount == insertCount);

			// commit the context
			// CommitStrategy.Serial, commits each insert as a single request
			// This is the default strategy and need not be passed in - context.Commit() would work the same
			context.Commit(CommitStrategy.Serial);
			
			// On a successful commit, the state of the entity changes to EntityState.Unchanged.
			// validate that the context contains n items with the correct state
			int contextUnchangedState = context
											.GetTrackedEntities(EntityState.Unchanged)
											.Count;

			Assert.IsTrue(contextInsertCount == contextUnchangedState);

            //----------------------------------------------------------------------------------------------------------
			// The context continues to hold on to the entities so it is often a good idea to either clear the context
			// if the entities are no longer needed or better still to just create a new context.
            //----------------------------------------------------------------------------------------------------------

			// Get a new context and query for the rows we inserted earlier by using the same partition key, 
			// departmentDev
			context = client.GetContext();

			context.CreateQuery().Where(emp => emp.Department == departmentDev).ToList();

			// Confirm we have the corrent number of items in the context
			Assert.IsTrue(context.GetTrackedEntities().Count == insertCount);

			// Make changes to this collection of employees in the context, modifying some and deleting the others.
			int entityIdx	= 0;
			int updateCount = 0;
			int deleteCount = 0;

			context
				.GetTrackedEntities()
				.ForEach(
					entityDescriptor => 
						{
							Employee
							employee = entityDescriptor.Entity;

							if (entityIdx % 3 == 0) 
							{
								context.Delete(employee);			// delete every 3rd one
								++deleteCount;
							}
							else									// update the others
							{
								++employee.SkillLevel;
						
								employee.SetAnnualSalary();			// change the salary of of the employee

								context.Update(employee);			// and update in the context
						
								++updateCount;				
							}

							++entityIdx;
						});

			// Commit the context, this time use the Parallel strategy.
			// The Parallel strategy will perform all requests in parallel. In the event of an error, 
			// a single aggregate StashAggregateException is returned containing one or more of the errors.
			context.Commit(CommitStrategy.Parallel);

			// Performing a delete will remove an entity form the context. Validate this.
			Assert.IsTrue(context.GetTrackedEntities().Count == insertCount - deleteCount);

            //----------------------------------------------------------------------------------------------------------
			// Now delete all the other entities in the context

			context.GetTrackedEntities().ForEach(
				entityDescriptor => context.Delete(entityDescriptor.Entity));

			// Commit the context, this time use the batch strategy.
			// In a nutshell, Batch is a mode supported by Azure table storage which allows, 
			// a number and size limited request to be made. 
			// Batch is only allowed on entities within the same Partition and has an all or none success guarantee.
			context.Commit(CommitStrategy.Batch);			

			// If all were deleted successfully the context should not be tracking any entities.
			Assert.IsTrue(context.GetTrackedEntities().Count == 0);

			//----------------------------------------------------------------------------------------------------------
			// So far so good. This concludes this part of the tutorial. Go Stash!
		}	
	}
}
