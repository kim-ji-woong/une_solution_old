using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnEService_Core.Common;
using UnEService_Core.Interface;
using UnEService_Core.Manager;

namespace UnEService_Core.Service
{
    public class WebDBService : IWebDB
    {
        // Thread로부터 안전하게 사용하기 위하여 Dictionary 대신 ConcurrentDictionary를 사용한다.
        private ConcurrentDictionary<long, DBManager> m_dicTransactions = new ConcurrentDictionary<long, DBManager>();
        private ConcurrentQueue<DBManager> m_queueTransactions = new ConcurrentQueue<DBManager>();
        private System.Timers.Timer m_timer = null;
        private double m_dTransactionTimeoutSeconds = 5.0;

        private static readonly object _lock = new object();

        private static WebDBService _instance;
        public static WebDBService Instance
        {
            get
            {
                lock(_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new WebDBService();
                    }
                    return _instance;
                }
            }
        }

        public WebDBService()
        {
            DBManager.Host = Startup.Configuration.GetSection("AppConfiguration").GetSection("host").Value;
            DBManager.ID = Startup.Configuration.GetSection("AppConfiguration").GetSection("id").Value;
            DBManager.PW = Startup.Configuration.GetSection("AppConfiguration").GetSection("pw").Value;
            DBManager.CharSet = Startup.Configuration.GetSection("AppConfiguration").GetSection("charSet").Value;

            string strTimeout = Startup.Configuration.GetSection("AppConfiguration").GetSection("transactionTimeout").Value;
            m_dTransactionTimeoutSeconds = double.Parse(strTimeout);

            m_timer = new System.Timers.Timer(1000);
            m_timer.Elapsed += OnTimer;
            m_timer.Start();
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckTimeoutTransactions();
            Logger.Instance.RemoveOldLogs();
        }

        // Timeout을 경과한 Transaction이 있으면 제거한다.
        private void CheckTimeoutTransactions()
        {
            DBManager transactionOwner = null;
            DateTime dtNow = DateTime.Now;

            while (m_queueTransactions.Count > 0)
            {
                if (m_queueTransactions.TryPeek(out transactionOwner))
                {
                    TimeSpan span = dtNow - transactionOwner.CreateTime;

                    // Timeout이 경과하였는가?
                    if (span.TotalSeconds > m_dTransactionTimeoutSeconds)
                    {
                        if (transactionOwner.IsRemoved)
                            m_queueTransactions.TryDequeue(out transactionOwner);
                        else
                        {
                            transactionOwner.BatchRollback();

                            if (m_dicTransactions.TryRemove(transactionOwner.TransactionKey, out transactionOwner))
                            {
                                transactionOwner.IsRemoved = true;
                                m_queueTransactions.TryDequeue(out transactionOwner);
                            }
                        }
                    }
                    else
                        break;
                }
                else
                    break;
            }
        }

        /// <summary>
        /// Query를 실행시키고 그 결과를 확인한다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="dbType">대소문자는 상관없다. mysql, sqlserver 가운데 선택한다.</param>
        /// <param name="query"></param>
        /// <returns>배열의 첫번째 요소 : 쿼리의 성공 여부("1"이면 성공, "0"이면 실패)
        ///          배열의 두번째 요소 : 성공했을 경우(결과값의 개수), 실패했을 경우(에러 메시지)
        ///          결과값 : null인 값은 '~'으로 시작, null이 아닌값은 '!'으로 시작
        /// </returns>
        public string[] RunQuery(string dbName, string dbType, string query)
        {
            if (dbName == null || dbName.Length == 0)
                return ErrorMessage("[RunQuery] DB 이름을 알수 없습니다.");
            else if (dbType == null || dbType.Length == 0)
                return ErrorMessage("[RunQuery] DB Type을 알수 없습니다.");
            else if (query == null || query.Length == 0)
                return ErrorMessage("[RunQuery] 실행하여야 할 쿼리가 존재하지 않습니다.");

            if (string.Compare(dbType, "mysql", true) == 0)
                return MySQLManager.RunQuery(dbName, query, null);
            else if (string.Compare(dbType, "sqlserver", true) == 0)
                return SqlServerManager.RunQuery(dbName, query, null);
            //else
            return ErrorMessage("[RunQuery] " + dbType + "은 알수 없는 DB Type입니다.");
        }

        /// <summary>
        /// StoredProcedure를 실행시키고 그 결과를 확인한다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="dbType">대소문자는 상관없다. mysql, sqlserver 가운데 선택한다.</param>
        /// <param name="procedureName"></param>
        /// <param name="fieldNames"></param>
        /// <Param name="fieldValues"></Param>
        /// <returns>배열의 첫번째 요소 : 쿼리의 성공 여부("1"이면 성공, "0"이면 실패)
        ///          배열의 두번째 요소 : 성공했을 경우(결과값의 개수), 실패했을 경우(에러 메시지)
        ///          결과값 : null인 값은 '~'으로 시작, null이 아닌값은 '!'으로 시작
        /// </returns>
        public string[] RunStoredProcedure(string dbName, string dbType, string procedureName, List<string> fieldNames, List<string> fieldValues)
        {
            if (dbName == null || dbName.Length == 0)
                return ErrorMessage("[RunStoredProcedure] DB 이름을 알수 없습니다.");
            else if (dbType == null || dbType.Length == 0)
                return ErrorMessage("[RunStoredProcedure] DB Type을 알수 없습니다.");
            else if (procedureName == null || procedureName.Length == 0)
                return ErrorMessage("[RunStoredProcedure] 실행하여야 할 프로시저 이름이 존재하지 않습니다.");

            if (string.Compare(dbType, "mysql", true) == 0)
                return MySQLManager.RunStoredProcedure(dbName, procedureName, fieldNames, fieldValues, null);
            else
            if (string.Compare(dbType, "sqlserver", true) == 0)
                return SqlServerManager.RunStoredProcedure(dbName, procedureName, fieldNames, fieldValues, null);
            //else
            return ErrorMessage("[RunStoredProcedure] " + dbType + "은 알수 없는 DB Type입니다.");
        }

        /// <summary>
        /// Transaction 시작을 알린다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="dbType">대소문자는 상관없다. mysql, sqlserver 가운데 선택한다.</param>
        /// <param name="errorMessage">Transaction 시작이 성공하면 빈 문자열의 값을 갖는다.
        ///                            실패하면 에러 메시지를 갖는다.
        /// </param>
        /// <returns>Transaction 시작이 성공하면 TransactionKey를 리턴하며, 0보다 큰 값을 가진다.
        ///          이 Key는 BatchCommit이나 BatchRollback, BatchQuery를 호출할때 사용된다.
        ///          실패하면 0을 리턴한다.
        /// </returns>
        public long BeginBatch(string dbName, string dbType, out string errorMessage)
        {
            errorMessage = "";

            if (dbName == null || dbName.Length == 0)
                errorMessage = ErrorMessage2("[BeginBatch] DB 이름을 알수 없습니다.");
            else if (dbType == null || dbType.Length == 0)
                errorMessage = ErrorMessage2("[BeginBatch] DB Type을 알수 없습니다.");

            if (errorMessage.Length > 0)
                return 0;

            Guid id = Guid.NewGuid();
            byte[] bytes = id.ToByteArray();
            long transactionKey = BitConverter.ToInt64(bytes, 0);

            Logger.Instance.Write("[BeginBatch] " + transactionKey.ToString());

            if (m_dicTransactions.ContainsKey(transactionKey))
            {
                errorMessage = ErrorMessage2("[BeginBatch] Transaction이 이미 시작되어 있습니다.");
                return 0;
            }

            DBManager transactionOwner = null;
            string strResult = "";

            if (string.Compare(dbType, "mysql", true) == 0)
                transactionOwner = MySQLManager.BeginTransaction(dbName, out strResult);
            else if (string.Compare(dbType, "sqlserver", true) == 0)
                transactionOwner = SqlServerManager.BeginTransaction(dbName, out strResult);
            else
            {
                errorMessage = ErrorMessage2("[BeginBatch] " + dbType + "은 알수 없는 DB Type입니다.");
                return 0;
            }

            if (transactionOwner != null)
            {
                // Timeout이 경과한 Transaction 자동삭제를 위하여 Dictionary와 Queue에 따로 보관한다.
                m_dicTransactions[transactionKey] = transactionOwner;
                transactionOwner.TransactionKey = transactionKey;
                m_queueTransactions.Enqueue(transactionOwner);
            }

            return transactionKey;
        }

        /// <summary>
        /// Transaction을 커밋한다.
        /// </summary>
        /// <param name="transactionKey"></param>
        /// <returns>Transaction 커밋이 성공하면 빈 문자열을 리턴한다.
        ///          실패하면 에러 메시지를 리턴한다.
        /// </returns>
        public string BatchCommit(long transactionKey)
        {
            DBManager transactionOwner = null;

            if (m_dicTransactions.TryGetValue(transactionKey, out transactionOwner) == false)
                return ErrorMessage2("[BatchCommit] 유효하지 않은 TransactionKey 입니다. " + transactionKey.ToString());

            string strResult = transactionOwner.BatchCommit();

            if (m_dicTransactions.TryRemove(transactionKey, out transactionOwner))
            {
                transactionOwner.IsRemoved = true;
            }

            Logger.Instance.Write("[BatchCommit] " + transactionKey.ToString());
            return strResult;
        }

        /// <summary>
        /// Transaction을 롤백한다.
        /// </summary>
        /// <param name="transactionKey"></param>
        /// <returns>Transaction 롤백이 성공하면 빈 문자열을 리턴한다.
        ///          실패하면 에러 메시지를 리턴한다.
        /// </returns>
        public string BatchRollback(long transactionKey)
        {
            DBManager transactionOwner = null;

            if (m_dicTransactions.TryGetValue(transactionKey, out transactionOwner) == false)
                return ErrorMessage2("[BatchRollback] 유효하지 않은 TransactionKey 입니다. " + transactionKey.ToString());

            string strResult = transactionOwner.BatchRollback();

            if (m_dicTransactions.TryRemove(transactionKey, out transactionOwner))
            {
                transactionOwner.IsRemoved = true;
            }

            Logger.Instance.Write("[BatchRollback] " + transactionKey.ToString());
            return strResult;
        }

        /// <summary>
        /// Transaction을 사용하여 Query를 실행시키고 그 결과를 확인한다.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="transactionKey"></param>
        /// <returns>배열의 첫번째 요소 : 쿼리의 성공 여부("1"이면 성공, "0"이면 실패)
        ///          배열의 두번째 요소 : 성공했을 경우(결과값의 개수), 실패했을 경우(에러 메시지)
        ///          결과값 : null인 값은 '~'으로 시작, null이 아닌값은 '!'으로 시작
        /// </returns>
        public string[] BatchQuery(string query, long transactionKey)
        {
            if (query == null || query.Length == 0)
                return ErrorMessage("[BatchQuery] 실행하여야 할 쿼리가 존재하지 않습니다.");

            DBManager transactionOwner;

            if (m_dicTransactions.TryGetValue(transactionKey, out transactionOwner) == false)
            {
                return ErrorMessage("[BatchQuery] 유효하지 않은 TransactionKey 입니다. " + transactionKey.ToString());
            }

            if (transactionOwner is MySQLManager)
                return MySQLManager.RunQuery(null, query, (MySQLManager)transactionOwner);
            else if (transactionOwner is SqlServerManager)
                return SqlServerManager.RunQuery(null, query, (SqlServerManager)transactionOwner);
            //else
            return ErrorMessage("[BatchQuery] 알수 없는 DB 오류입니다.");
        }

        /// <summary>
        /// StoredProcedure를 실행시키고 그 결과를 확인한다.
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="fieldNames"></param>
        /// <Param name="fieldValues"></Param>
        /// <param name="transactionKey"></param>
        /// <returns>배열의 첫번째 요소 : 쿼리의 성공 여부("1"이면 성공, "0"이면 실패)
        ///          배열의 두번째 요소 : 성공했을 경우(결과값의 개수), 실패했을 경우(에러 메시지)
        ///          결과값 : null인 값은 '~'으로 시작, null이 아닌값은 '!'으로 시작
        /// </returns>
        public string[] BatchStoredProcedure(string procedureName, List<string> fieldNames, List<string> fieldValues, long transactionKey)
        {
            if (procedureName == null || procedureName.Length == 0)
                return ErrorMessage("[BatchStoredProcedure] 실행하여야 할 프로시저 이름이 존재하지 않습니다.");

            DBManager transactionOwner;

            if (m_dicTransactions.TryGetValue(transactionKey, out transactionOwner) == false)
            {
                return ErrorMessage("[BatchStoredProcedure] 유효하지 않은 TransactionKey 입니다. " + transactionKey.ToString());
            }

            if (transactionOwner is MySQLManager)
                return MySQLManager.RunStoredProcedure(null, procedureName, fieldNames, fieldValues, (MySQLManager)transactionOwner);
            else if (transactionOwner is SqlServerManager)
                return SqlServerManager.RunStoredProcedure(null, procedureName, fieldNames, fieldValues, (SqlServerManager)transactionOwner);
            //else
            return ErrorMessage("[BatchStoredProcedure] 알수 없는 DB 오류입니다.");
        }

        public static string[] ErrorMessage(string strMessage)
        {
            Logger.Instance.Write(strMessage);

            string[] results = new string[2];
            results[0] = "0";
            results[1] = strMessage;
            return results;
        }

        public static string ErrorMessage2(string strMessage, string strMethod = null)
        {
            if (strMethod == null)
                Logger.Instance.Write(strMessage);
            else
                Logger.Instance.Write(strMethod + " : " + strMessage);

            return strMessage;
        }
    }
}
