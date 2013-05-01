//If you do not have the Storage Client installed, comment the following line to remove the dependency.
#define USE_STORAGE_CLIENT
using System;
using System.Linq;
using System.Configuration;
using System.Diagnostics;

#if USE_STORAGE_CLIENT
using Microsoft.WindowsAzure;
#endif

namespace CodeSuperior.Lucifure.Tutorial
{
	// -----------------------------------------------------------------------------------------------------------------

	enum ConfigurationType
	{
		StashCloud,					// Stash StorageAccountKey - Storage in the Cloud
		StashEmulator,				// Stash StorageAccountKey - Storage in the Emulator
		StorageAccountCloud,		// Azure Storage Client Credentials - Storage in the Cloud
		StorageAccountEmulator,		// Azure Storage Client Credentials - Storage in the Emulator
	}

	// -----------------------------------------------------------------------------------------------------------------

	public
	static  
	class StashConfiguration
	{
		// The options class allows additional parameters to be set.
		public
		static
		StashClientOptions
		GetDefaultOptions()
		{
			return new StashClientOptions {
										UseHttps			= false,
				                        Feedback			= TraceFeedback, 
									};
		}

		// This method, demonstrates various ways of setting up the credentials.
		public
		static
		StashClient<T>
		GetClient<T>(
			StashClientOptions					options)
		{
			// Change the type here to target different storage accounts and different credential methods.
			ConfigurationType
			configType = ConfigurationType.StashEmulator;

			StashClient<T>
			result = null;

			switch(configType)
			{
				// Use the StashCredential class.
				case ConfigurationType.StashCloud:
					
					result =	new StashClient<T>(
									new StorageAccountKey(
										ConfigurationManager.AppSettings["AccountName"],
										ConfigurationManager.AppSettings["key"]),
									options);		
					break;

				// The storage emulator credentials are built into Stash.
				case ConfigurationType.StashEmulator:

					result = new StashClient<T>(
											options);		
					break;

#if USE_STORAGE_CLIENT
				// Use the Azure Storage Client credentials infrastructure.
				case ConfigurationType.StorageAccountCloud:
					
					result = GetStasherUsingCloudStorageAccount<T>(
													"DataConnectionString",
													options);
					break;

				// Use the Azure Storage Client credentials infrastructure for the emulator.
				case ConfigurationType.StorageAccountEmulator:
					
					result = GetStasherUsingCloudStorageAccount<T>(
													"DataConnectionStringEmulator",
													options);
					break;
#endif
				default:
					throw new ApplicationException("Incorrect Configuration Type.");
			}

			return result;
		}

		/// <summary>
		/// Get the StashClient using default options and credentials.
		/// </summary>
		public
		static
		StashClient<T>
		GetClient<T>()
		{
			return GetClient<T>(GetDefaultOptions());
		}

#if USE_STORAGE_CLIENT
		/// <summary>
		/// Setup the StashClient by using elements from the CloudStorageAccount.
		/// </summary>
		static
		StashClient<T>
		GetStasherUsingCloudStorageAccount<T>(
			CloudStorageAccount					account, 
			StashClientOptions					options)
		{
			// indicate it https is selected.
			options.UseHttps = account.TableEndpoint.Scheme == "https";

			// instantiate the client with the SignRequestLite method from the Credentials.
			return new StashClient<T>(
							account.Credentials.AccountName,
							new SignRequest(h => account.Credentials.SignRequestLite(h)),
							options);		
		}

		static
		StashClient<T>
		GetStasherUsingCloudStorageAccount<T>(
			string								settings,
			StashClientOptions					options)
		{
			return GetStasherUsingCloudStorageAccount<T>(
									GetCloudStorageAccount(settings),
									options);		
		}

		// -------------------------------------------------------------------------------------------------------------
		// static constructor used for setting up the CloudStorageAccount so as to use the 
		// Microsoft storage client infrastructure for credentials
		
		static 
		StashConfiguration()
		{
		    CloudStorageAccount.SetConfigurationSettingPublisher(
		        (configName, configSetter) =>
		            {
		                configSetter(
		                    ConfigurationManager.AppSettings[configName]);
		            });

		}

		public
		static
		CloudStorageAccount
		GetCloudStorageAccount(
			string								setting)
		{
		    return CloudStorageAccount.FromConfigurationSetting(setting);
		}

		public
		static
		CloudStorageAccount
		GetCloudStorageAccount()
		{
		    return GetCloudStorageAccount("DataConnectionString");
		}

		public
		static
		CloudStorageAccount
		GetCloudStorageAccountEmulator()
		{
		    return GetCloudStorageAccount("DataConnectionStringEmulator");
		}
#endif

		// -------------------------------------------------------------------------------------------------------------

			static
			string								_formatUri = "Uri: {0} - {1}";

		/// <summary>
		/// Lucifure Stash invokes a trace feedback call if supplied. 
		/// This implementation, in debug mode, will output the request and response data for debugging purposes.
		/// </summary>
		public 
		static 
		void 
		TraceFeedback(
			object								obj)
		{
			StashRequestResponse				reqRes;
			
			if ((reqRes = obj as StashRequestResponse) != null)
			{	
				Trace.WriteLine("");

				Trace.WriteLine(
							String.Format(
										_formatUri,
										reqRes.Request.Method,
										reqRes.Request.RequestUri));
				
				if (!String.IsNullOrWhiteSpace(reqRes.RequestBody))
				{
					Trace.WriteLine("Request Body:");
					Trace.WriteLine(reqRes.RequestBody);
				}

				if (!String.IsNullOrWhiteSpace(reqRes.ResponseBody))
				{
					Trace.WriteLine("Response Body:");
					Trace.WriteLine(reqRes.ResponseBody);
				}

				Trace.Flush();
			}
		}

	}
}
