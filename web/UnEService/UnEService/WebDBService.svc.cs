using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Collections.Concurrent;

namespace UnEService
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "WebDBService"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 WebDBService.svc나 WebDBService.svc.cs를 선택하고 디버깅을 시작하십시오.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class WebDBService : IWebDB
    {
        // Thread로부터 안전하게 사용하기 위하여 Dictionary 대신 ConcurrentDictionary를 사용한다.
        private ConcurrentDictionary<long, DBManager> m_dicTransactions = new ConcurrentDictionary<long, DBManager>();
        private ConcurrentDictionary<long, List<string>> m_dicMultiQueries = new ConcurrentDictionary<long, List<string>>();
        private ConcurrentQueue<DBManager> m_queueTransactions = new ConcurrentQueue<DBManager>();
        private System.Timers.Timer m_timer = null;
        private double m_dTransactionTimeoutSeconds = 5.0;
        
        public WebDBService()
        {
            DBManager.Host = System.Configuration.ConfigurationManager.AppSettings["host"].ToString();
            DBManager.ID = System.Configuration.ConfigurationManager.AppSettings["id"].ToString();
            DBManager.PW = System.Configuration.ConfigurationManager.AppSettings["pw"].ToString();
            DBManager.CharSet = System.Configuration.ConfigurationManager.AppSettings["charSet"].ToString();

            string strTimeout = System.Configuration.ConfigurationManager.AppSettings["transactionTimeout"].ToString();
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
        /// Multi 쿼리 시작을 알린다.
        /// Multi 쿼리는 Select가 아닌 Insert, Update, Delete 문에서만 사용할 수 있다.
        /// </summary>
        /// <returns>
        /// MultiQuery가 정상적으로 시작되면 MultiQuery Key값이 리턴되며, 0보다 큰 값을 가진다.
        /// 이 값은 AddMultiQuery와 RunMultiQuery를 호출할 때 사용한다.
        /// 실패하면 0을 리턴한다.
        /// </returns>
        public long BeginMultiQuery()
        {
            Guid id = Guid.NewGuid();
            byte[] bytes = id.ToByteArray();
            long multiQueryKey = BitConverter.ToInt64(bytes, 0);

            m_dicMultiQueries[multiQueryKey] = new List<string>();
            return multiQueryKey;
        }

        /// <summary>
        /// Multi 쿼리를 추가한다.
        /// </summary>
        /// <param name="strSQL">추가할 쿼리</param>
        /// <param name="key">BeginMultiQuery의 리턴값</param>
        /// <returns>
        /// 취소가 성공하면 빈 문자열을 리턴한다.
        /// 실패하면 에러 메시지를 리턴한다.
        /// </returns>
        public string AddMultiQuery(string strSQL, long key)
        {
            List<string> queries;

            if (m_dicMultiQueries.TryGetValue(key, out queries) == false)
                return ErrorMessage2("[AddMultiQuery] 알수없는 MultiQuery Key입니다." + key.ToString());

            queries.Add(strSQL);
            return "";
        }

        /// <summary>
        /// Multi 쿼리를 취소합니다.
        /// </summary>
        /// <param name="key">BeginMultiQuery의 리턴값</param>
        /// <returns>
        /// 취소가 성공하면 빈 문자열을 리턴한다.
        /// 실패하면 에러 메시지를 리턴한다.
        /// </returns>
        public string CancelMultiQuery(long key)
        {
            List<string> queries;
            m_dicMultiQueries.TryRemove(key, out queries);
            return "";
        }

        /// <summary>
        /// Multi 쿼리를 실행시킨다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="dbType">대소문자는 상관없다. mysql, sqlserver 가운데 선택한다.</param>
        /// <param name="key">AddMultiQuery시 사용한 키값</param>
        /// <returns>
        /// 배열의 첫번째 요소 : 쿼리의 성공 여부("1"이면 성공, "0"이면 실패)
        /// 배열의 두번째 요소 : 성공했을 경우(결과값의 개수), 실패했을 경우(에러 메시지)
        /// 결과값 : null인 값은 '~'으로 시작, null이 아닌값은 '!'으로 시작
        /// </returns>
        public string[] RunMultiQuery(string dbName, string dbType, long key)
        {
            List<string> queries;

            if (m_dicMultiQueries.TryGetValue(key, out queries) == false)
                return ErrorMessage("[RunMultiQuery] 알수없는 MultiQuery Key입니다." + key.ToString());

            if (dbName == null || dbName.Length == 0)
                return ErrorMessage("[RunMultiQuery] DB 이름을 알수 없습니다.");
            else if (dbType == null || dbType.Length == 0)
                return ErrorMessage("[RunMultiQuery] DB Type을 알수 없습니다.");
            else if (queries.Count == 0)
                return ErrorMessage("[RunMultiQuery] 실행하여야 할 쿼리가 존재하지 않습니다.");

            string strQueries = "";

            foreach (string query in queries)
            {
                strQueries += query;
            }

            string[] results = null;

            if (string.Compare(dbType, "mysql", true) == 0)
                results = MySQLManager.RunMultiQuery(dbName, strQueries, null);
            else if (string.Compare(dbType, "sqlserver", true) == 0)
                results = SqlServerManager.RunMultiQuery(dbName, strQueries, null);
            else
                results = ErrorMessage("[RunMultiQuery] " + dbType + "은 알수 없는 DB Type입니다.");

            m_dicMultiQueries.TryRemove(key, out queries);
            return results;
        }

        /// <summary>
        /// 트랜잭션과 함께 Multi 쿼리를 실행시킨다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="dbType">대소문자는 상관없다. mysql, sqlserver 가운데 선택한다.</param>
        /// <param name="key">AddMultiQuery시 사용한 키값</param>
        /// <param name="transactionKey"></param>
        /// <returns>
        /// 배열의 첫번째 요소 : 쿼리의 성공 여부("1"이면 성공, "0"이면 실패)
        /// 배열의 두번째 요소 : 성공했을 경우(결과값의 개수), 실패했을 경우(에러 메시지)
        /// 결과값 : null인 값은 '~'으로 시작, null이 아닌값은 '!'으로 시작
        /// </returns>
        public string[] BatchMultiQuery(long key, long transactionKey)
        {
            List<string> queries;

            if (m_dicMultiQueries.TryGetValue(key, out queries) == false)
                return ErrorMessage("[BatchMultiQuery] 알수없는 MultiQuery Key입니다." + key.ToString());

            DBManager transactionOwner;

            if (m_dicTransactions.TryGetValue(transactionKey, out transactionOwner) == false)
            {
                return ErrorMessage("[BatchMultiQuery] 유효하지 않은 TransactionKey 입니다. " + transactionKey.ToString());
            }

            if (queries.Count == 0)
                return ErrorMessage("[BatchMultiQuery] 실행하여야 할 쿼리가 존재하지 않습니다.");

            string strQueries = "";

            foreach (string query in queries)
            {
                strQueries += query + ";";
            }

            string[] results = null;

            if (transactionOwner is MySQLManager)
                results = MySQLManager.RunMultiQuery(null, strQueries, (MySQLManager)transactionOwner);
            else if (transactionOwner is SqlServerManager)
                results = SqlServerManager.RunMultiQuery(null, strQueries, (SqlServerManager)transactionOwner);
            else
                results = ErrorMessage("[BatchMultiQuery] 알수 없는 DB 타입입니다.");

            m_dicMultiQueries.TryRemove(key, out queries);
            return results;
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
        public string BatchCommmit(long transactionKey)
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
