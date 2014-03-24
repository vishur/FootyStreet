using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace FootyStreet.Utilities
{
    /// <summary>
    /// Class is responsible for session related functionality
    /// </summary>
    public class SessionContainer : ISessionContainer
    {
        #region Private Variables
        private const string MultipleCasesOrDashboardIndicatorDataKey = "MultipleCasesOrDashboardIndicatorDataKey";
        private const string CasesAndApplicationsForDashboardDataKey = "CasesAndApplicationsForDashboardDataKey";
        private const string IndividualDashboardDataKey = "IndividualDashboardDataKey";
        private const string PrimaryApplicationIndividualDataKey = "PrimaryApplicationIndividualDataKey";
        private const string UserContextDataKey = "UserContextDataKey";
        private const string PrimaryIndividualDataKey = "PrimaryIndividualDataKey";
        private const string PrimaryApplicantDataKey = "PrimaryApplicantDataKey";
        private const string ApplicationDataKey = "ApplicationDataKey";
        private const string SessionTableKey = "SessionTableKey";
        public const string EmployeeRosterDataKey = "EmployeeRosterDataKey";
        public const string IssuerSesssionDataKey = "consumerInformation";
        private HttpSessionState userSession = null;
        private Hashtable table = null;

        #endregion Private Variables

        #region Constructor

        public SessionContainer()
        {
            userSession = HttpContext.Current.Session;
            if (userSession[SessionTableKey] != null)
            {
                table = userSession[SessionTableKey] as Hashtable;
            }
            else
            {
                table = new Hashtable();
            }
        }

        #endregion Constructor

        #region ISessionContainer Implementation

        /// <summary>
        /// Method to get the session data of given type for a Key
        /// </summary>
        /// <typeparam name="TModel">Type of Data</typeparam>
        /// <param name="key">Session Key</param>
        /// <returns>Session Data</returns>
        public TModel GetSessionData<TModel>(string key)
            where TModel : class
        {
            if (!table.ContainsKey(key))
            {
                return null;
            }
            else
            {
                TModel model = HttpContext.Current.Session[key] as TModel;
                return (model);
            }
        }

        /// <summary>
        /// Method to get the session data of given type
        /// </summary>
        /// <typeparam name="TModel">type of data</typeparam>
        /// <returns>session data</returns>
        public TModel GetSessionData<TModel>()
            where TModel : class
        {
            if (!table.ContainsValue(typeof(TModel).FullName))
            {
                return null;
            }
            else
            {
                string key = string.Empty;
                foreach (DictionaryEntry entry in table)
                {
                    if (entry.Value == typeof(TModel).FullName)
                    {
                        key = entry.Key.ToString();
                    }
                }
                TModel model = HttpContext.Current.Session[key] as TModel;
                return (model);
            }
        }

        /// <summary>
        /// Method to set data in session
        /// </summary>
        /// <typeparam name="TModel">type of data</typeparam>
        /// <param name="dataItem">value to be stored</param>
        /// <param name="key">Session Key</param>
        public void SetSessionData<TModel>(TModel dataItem, string key)
            where TModel : class
        {
            if (!table.ContainsKey(key))
            {
                table.Add(key, typeof(TModel).FullName);
            }
            HttpContext.Current.Session[key] = dataItem;
            userSession[SessionTableKey] = table;
        }

        /// <summary>
        /// Method to check whether data exists in session or not
        /// </summary>
        /// <param name="key">session key</param>
        /// <returns>status</returns>
        public bool IsDataExistsInSession(string key)
        {
            return (table.ContainsKey(key));

        }

        /// <summary>
        /// Method to remove data for the specified key from session
        /// </summary>
        /// <param name="key">Session Key</param>
        public void RemoveSessionData(string key)
        {
            if (table.ContainsKey(key))
            {
                table.Remove(key);
                HttpContext.Current.Session.Remove(key);
                userSession[SessionTableKey] = table;
            }

        }

        /// <summary>
        /// Method to clean up session
        /// </summary>
        public void RemoveSessionData()
        {
            List<string> keyslist = new List<string>();
            foreach (string key in table.Keys)
            {
                keyslist.Add(key);
            }
            foreach (string key in keyslist)
            {
                //List<string> keylist=new List<string>(){"PrimaryApplicantDataKey","ApplicationDataKey","PrimaryIndividualDataKey","UserContextDataKey","PrimaryApplicationIndividualDataKey"};
                List<string> keylist = new List<string>() { PrimaryApplicantDataKey, PrimaryIndividualDataKey, PrimaryApplicationIndividualDataKey, UserContextDataKey, CasesAndApplicationsForDashboardDataKey, ApplicationDataKey, IndividualDashboardDataKey, MultipleCasesOrDashboardIndicatorDataKey, IssuerSesssionDataKey };
                if (!keylist.Contains(key))
                {
                    table.Remove(key);
                    HttpContext.Current.Session.Remove(key);
                }
            }
            userSession[SessionTableKey] = table;
        }

        /// <summary>
        /// Method to remove all keys except the input
        /// </summary>
        /// <param name="keysToRetain">list of keys to retain</param>
        public void RemoveSessionData(IList<String> keysToRetain)
        {
            if (keysToRetain == null || !keysToRetain.Any())
            {
                return;
            }
            List<string> keyslist = table.Keys.Cast<string>().ToList();
            foreach (var keyToRemove in keyslist.Where(key => !keysToRetain.Contains(key)))
            {
                table.Remove(keyToRemove);
                HttpContext.Current.Session.Remove(keyToRemove);
            }
        }

        #endregion ISessionContainer Implementation
    }
}
