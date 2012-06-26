// Type: Microsoft.WindowsAzure.StorageClient.TableServiceContext
// Assembly: Microsoft.WindowsAzure.StorageClient, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Program Files\Windows Azure SDK\v1.6\ref\Microsoft.WindowsAzure.StorageClient.dll

using Microsoft.WindowsAzure;
using System;
using System.Data.Services.Client;

namespace Microsoft.WindowsAzure.StorageClient
{
    /// <summary>
    /// Represents a <see cref="T:System.Data.Services.Client.DataServiceContext"/> object for use with the Windows Azure Table service.
    /// 
    /// </summary>
    public class TableServiceContext : DataServiceContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.WindowsAzure.StorageClient.TableServiceContext"/> class.
        /// 
        /// </summary>
        /// <param name="baseAddress">The Table service endpoint to use create the service context.</param><param name="credentials">The account credentials.</param>
        public TableServiceContext(string baseAddress, StorageCredentials credentials);

        /// <summary>
        /// Begins an asynchronous operation to save changes, using the retry policy specified for the service context.
        /// 
        /// </summary>
        /// <param name="callback">The callback delegate that will receive notification when the asynchronous operation completes.</param><param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// <returns>
        /// An <see cref="T:System.IAsyncResult"/> that references the asynchronous operation.
        /// </returns>
        public IAsyncResult BeginSaveChangesWithRetries(AsyncCallback callback, object state);

        /// <summary>
        /// Begins an asynchronous operation to save changes, using the retry policy specified for the service context.
        /// 
        /// </summary>
        /// <param name="options">Additional options for saving changes.</param><param name="callback">The callback delegate that will receive notification when the asynchronous operation completes.</param><param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// <returns>
        /// An <see cref="T:System.IAsyncResult"/> that references the asynchronous operation.
        /// </returns>
        public IAsyncResult BeginSaveChangesWithRetries(SaveChangesOptions options, AsyncCallback callback, object state);

        /// <summary>
        /// Ends an asynchronous operation to save changes.
        /// 
        /// </summary>
        /// <param name="asyncResult">An <see cref="T:System.IAsyncResult"/> that references the pending asynchronous operation.</param>
        /// <returns>
        /// A <see cref="T:System.Data.Services.Client.DataServiceResponse"/> that represents the result of the operation.
        /// </returns>
        public DataServiceResponse EndSaveChangesWithRetries(IAsyncResult asyncResult);

        /// <summary>
        /// Saves changes, using the retry policy specified for the service context.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.Data.Services.Client.DataServiceResponse"/> that represents the result of the operation.
        /// </returns>
        public DataServiceResponse SaveChangesWithRetries();

        /// <summary>
        /// Saves changes, using the retry policy specified for the service context.
        /// 
        /// </summary>
        /// <param name="options">Additional options for saving changes.</param>
        /// <returns>
        /// A <see cref="T:System.Data.Services.Client.DataServiceResponse"/> that represents the result of the operation.
        /// </returns>
        public DataServiceResponse SaveChangesWithRetries(SaveChangesOptions options);

        /// <summary>
        /// Gets or sets the retry policy requests made via the service context.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The retry policy.
        /// </value>
        public RetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// Gets the storage account credentials used by the service context.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The account credentials.
        /// </value>
        public StorageCredentials StorageCredentials { get; }
    }
}
