using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootyStreet.Data.DataAccessCommon
{
    public class DataAccessCommon
    {
        private const string FootyStreetConnectionString = "FootyStreetConnectionString";

        public static DataSet GetDataFromStoredProc(string storedProcedureName, string[] tableNames, params object[] inputParameterValues)
        {
            DataSet dsResults = new DataSet();
            Database dDatabase = DatabaseFactory.CreateDatabase(FootyStreetConnectionString);
            System.Data.Common.DbCommand dcCommand = dDatabase.GetSqlStringCommand(storedProcedureName);
            dcCommand.CommandType = CommandType.StoredProcedure;
            dDatabase.AssignParameters(dcCommand, inputParameterValues);
            dcCommand.CommandTimeout = 120;
            dDatabase.LoadDataSet(dcCommand, dsResults, tableNames);
            return dsResults;
        }
        public static DataSet GetDataFromSSPStoredProc(string storedProcedureName, string[] tableNames, params object[] inputParameterValues)
        {
            DataSet dsResults = new DataSet();
            Database dDatabase = DatabaseFactory.CreateDatabase(FootyStreetConnectionString);
            System.Data.Common.DbCommand dcCommand = dDatabase.GetSqlStringCommand(storedProcedureName);
            dcCommand.CommandType = CommandType.StoredProcedure;
            dDatabase.AssignParameters(dcCommand, inputParameterValues);
            dcCommand.CommandTimeout = 120;
            dDatabase.LoadDataSet(dcCommand, dsResults, tableNames);
            return dsResults;
        }

        public static int UpdateOrInsertData(string storedProcedureName, params object[] parameterValues)
        {
            return DatabaseFactory.CreateDatabase(FootyStreetConnectionString).ExecuteNonQuery(storedProcedureName, parameterValues);
        }

        //public static DataSet GetDataFromStoredProcUsingUdt(string storedProcedureName, params KeyValuePair<string, object>[] parameterValues)
        //{
        //    var helper = new SqlClientHelper();
        //    return helper.GetDataSetByProcedure(storedProcedureName, KyHbeConnectionString, true, parameterValues);
        //}
        public static object GetScalarValueFromSSPStoredProc(string storedProcedureName, params object[] inputParameterValues)
        {
            return DatabaseFactory.CreateDatabase(FootyStreetConnectionString).ExecuteScalar(storedProcedureName, inputParameterValues);

        }
        //public static object UpdateOrInsertDataUsingUdt(string storedProcedureName, params KeyValuePair<string, object>[] parameterValues)
        //{
        //    var helper = new SqlClientHelper();
        //    return helper.GetScalarByProcedure(storedProcedureName, KyHbeConnectionString, true, parameterValues);
        //}
    }
}
