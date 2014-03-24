using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootyStreet.Utilities
{
    public interface ISessionContainer
    {
        /// <summary>
        /// Method to get the session data of given type
        /// </summary>
        /// <typeparam name="TModel">type of data</typeparam>
        /// <returns>session data</returns>
        TModel GetSessionData<TModel>() where TModel : class;

        /// <summary>
        /// Method to get the session data of given type for a Key
        /// </summary>
        /// <typeparam name="TModel">Type of Data</typeparam>
        /// <param name="key">Session Key</param>
        /// <returns>Session Data</returns>
        TModel GetSessionData<TModel>(string key) where TModel : class;

        /// <summary>
        /// Method to check whether data exists in session or not
        /// </summary>
        /// <param name="key">session key</param>
        /// <returns>status</returns>
        bool IsDataExistsInSession(string key);

        /// <summary>
        /// Method to remove data for the specified key from session
        /// </summary>
        /// <param name="key">Session Key</param>
        void RemoveSessionData(string key);

        /// <summary>
        /// Method to clean up session
        /// </summary>
        void RemoveSessionData();

        /// <summary>
        /// Method to remove all keys except the input
        /// </summary>
        /// <param name="keysToRetain">list of keys to retain</param>
        void RemoveSessionData(IList<String> keysToRetain);

        /// <summary>
        /// Method to set data in session
        /// </summary>
        /// <typeparam name="TModel">type of data</typeparam>
        /// <param name="dataItem">value to be stored</param>
        /// <param name="key">Session Key</param>
        void SetSessionData<TModel>(TModel dataItem, string key) where TModel : class;
    }
}
