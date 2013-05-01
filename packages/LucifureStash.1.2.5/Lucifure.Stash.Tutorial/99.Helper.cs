using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSuperior.Lucifure.Tutorial
{
	// -----------------------------------------------------------------------------------------------------------------
	// -----------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Before running any test, Modify App.Config appropriately
	/// </summary>
	public
	static  
	class StashHelper
	{
		// -------------------------------------------------------------------------------------------------------------

		public
		static 
		bool
		DictionaryEquals(
			IDictionary<string, object>			lhs,
			IDictionary<string, object>			rhs)
		{
			// skip the ETag value because it differs
			var 
			keysLhs = lhs.Where(x => x.Key != Literal.ETag).OrderBy(x => x.Key).ToList();

			var
			keysRhs = rhs.Where(x => x.Key != Literal.ETag).OrderBy(x => x.Key).ToList();

			return keysLhs.Count() == keysRhs.Count() 
				&& keysLhs.All(x => x.Value.ToString().Equals(rhs[x.Key].ToString())	// values are the same
					&& x.Value.GetType() == rhs[x.Key].GetType());						// types are the same
		}


		// -------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Example code on how to add a retry policy.
		/// Here, a wrapper is built around the default Exponential retry policy, such that calls to the retry 
		/// policy can be intercepted and logged if necessary.
		/// </summary>
		public 
		static 
		void 
		AddRetryInterceptor(
			StashClientOptions					options)
		{
			// Get a function which creates the policy and invoke it to get the actual policy
			var exponentatial = 
					RetryPolicies.GetExponential(
										Retryable.DefaultAttempts,
										Retryable.DefaultDeltaBackoff)();
			
			// create an interceptor which wraps the policy and assigns it to the option.
			options.RetryPolicy = 
				() => 
					(attempt, ex) => {
						System.Diagnostics.Debug.WriteLine(
														"Attempt = {0}. Exception = {1}",
														attempt,
														ex);

						return exponentatial(attempt, ex);
					};
					
		}

		// -------------------------------------------------------------------------------------------------------------
	}


}
