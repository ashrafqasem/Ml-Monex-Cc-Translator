
using System;
using System.IO.Ports;
using System.Threading;
using System.Text;
using System.IO;
using System.Linq;
//
using System.Collections.Generic;
using MLmonexCCtranslator;

namespace ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MonexCCtranslator monexCCtranslator = new MonexCCtranslator("COM6");

            decimal salesTransactionAmount = 1.5m;

            TransactionResponse saleTransactionRequest = monexCCtranslator.SaleTransactionRequest(salesTransactionAmount); // 15000000000000000000
            TransactionResponse cancelTransactionRequest = monexCCtranslator.CancelTransactionRequest();
            TransactionResponse reversalTransactionRequest = monexCCtranslator.ReversalTransactionRequestCanada();

            TransactionResponse syncParametersRequest = monexCCtranslator.SyncParametersRequest("PS60G454");
            TransactionResponse hostDownloadRequest = monexCCtranslator.HostDownloadRequest();

            TransactionResponse preAuthTransactionRequest = monexCCtranslator.PreAuthTransactionRequestCanada(salesTransactionAmount);
            //TransactionResponse preAuthCompletionRequest = monexCCtranslator.PreAuthCompletionRequestCanada(preAuthTransactionRequest.ApprovedSalesAmount, preAuthTransactionRequest.RecordNumber, preAuthTransactionRequest.PAN, preAuthTransactionRequest.AuthorizationCode);
            ////TransactionResponse preAuthCompletionRequest = monexCCtranslator.PreAuthCompletionRequestCanada(85, "1234", "1234567\0", "123456\08");

            //TransactionResponse readCardDataRequest = monexCCtranslator.ReadCardDataRequestCanada();

            //string communicationPackageRequestHexStr = string.Empty;
            //communicationPackageRequestHexStr = "02 00 0D 10 27 00 00 00 00 00 00 00 00 00 3A";
            //communicationPackageRequestHexStr = "02 00 0A 10 29 00 00 00 00 96 11 B4";
            //TransactionResponse testCommunicationPackageRequestHexStr = monexCCtranslator.TestCommunicationPackageRequestHexStr(MonexCCtranslator.CommandEnum.PreAuthTransactionRequest, communicationPackageRequestHexStr);

        }
    }
}





 
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ConsoleTest
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//        }
//    }
//}
