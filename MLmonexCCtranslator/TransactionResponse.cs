using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLmonexCCtranslator
{
    public class TransactionResponse
    {
        /* Request */
        public string CommunicationPackageRequestHexStr { get; set; }
        public string CommunicationPackageRequestHexStr2 { get; set; }
        public string CommandCodeRequest { get; set; }
        public string CommandCodeRequest2 { get; set; }
        public string CommandCodeRequestDefinition { get; set; }
        public string CommandCodeRequestDefinition2 { get; set; }

        /* Response */
        public string AsciiWindows1252stringResponse { get; set; }
        public string AsciiWindows1252stringResponse2 { get; set; }
        public string AsciiWindows1252stringResponse3 { get; set; }
        public string HexStringResponse { get; set; }
        public string HexStringResponse2 { get; set; }
        public string HexStringResponse3 { get; set; }
        public string CommandCodeResponse { get; set; }
        public string CommandCodeResponse2 { get; set; }
        public string CommandCodeResponse3 { get; set; }
        public string CommandCodeResponseDefinition { get; set; }
        public string CommandCodeResponseDefinition2 { get; set; }
        public string CommandCodeResponseDefinition3 { get; set; }
        public string ErrorCodeResponse { get; set; }
        public string ErrorCodeResponse2 { get; set; }
        public string ErrorCodeResponse3 { get; set; }
        public string ErrorCodeResponseDefinition { get; set; }
        public string ErrorCodeResponseDefinition2 { get; set; }
        public string ErrorCodeResponseDefinition3 { get; set; }
        public long TimeStampResponse { get; set; }
        public long TimeStampResponse2 { get; set; }
        public long TimeStampResponse3 { get; set; }
        public bool IsTransactionSucceeded { get; set; }
        public bool IsTransactionSucceeded2 { get; set; }
        public bool IsTransactionSucceeded3 { get; set; }

        public string ApplicationErrorMessage { get; set; }

        public decimal ApprovedSalesAmount { get; set; }
        public string TID { get; set; }
        public int CardTypeId { get; set; } 
        public string CardType { get; set; }
        public string PAN { get; set; }
        public string AuthorizationCode { get; set; }
        public string SequenceNumber { get; set; }
        public string CardHoldersName { get; set; }
        public string CardExpiration { get; set; }
        public string PayerIdentifier { get; set; }
        public string RecordNumber { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string TheReserved { get; set; }
        public string SignedPackag { get; set; }

        public string JsonStringResponse { get; set; }
    }
}

