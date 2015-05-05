using System;

using System.Transactions;

namespace ServerInfra.Classes
{
    public class TransactionUtils
    {
        private static TimeSpan _transactionTimeout;
        private static IsolationLevel _isolationLevel = IsolationLevel.RepeatableRead;

        static TransactionUtils()
        {
            _transactionTimeout = new TimeSpan(0, 10, 0);
        }

        public static TransactionScope CreateTransactionScope(TransactionScopeOption transactionScope)
        {
            var transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = _isolationLevel;
            transactionOptions.Timeout = _transactionTimeout;
            return new TransactionScope(transactionScope,
                transactionOptions);
        }





    }
}
