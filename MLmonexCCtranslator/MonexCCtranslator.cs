using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;
using MLmonexCCtranslator;
using System.Text.Json;
using System.Globalization;
using System.Security.Cryptography;

namespace MLmonexCCtranslator 
{
    public class MonexCCtranslator
    {
        public MonexCCtranslator(string portName)
        {
            _portName = portName.Trim();
            _serialPort = SetSerialPort();
        }

        public MonexCCtranslator(string portName, decimal creditCardReaderSoftwareVersion)
        {
            _creditCardReaderSoftwareVersion = creditCardReaderSoftwareVersion;

            _portName = portName.Trim();
            _serialPort = SetSerialPort();
        }

        public MonexCCtranslator(string portName, decimal creditCardReaderSoftwareVersion, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake, bool dtrEnable, int readTimeout, int writeTimeout, Encoding encoding) 
        {
            _creditCardReaderSoftwareVersion = creditCardReaderSoftwareVersion;

            _portName = portName;
            _baudRate = baudRate;
            _parity = parity;
            _dataBits = dataBits;
            _stopBits = stopBits;
            _handshake = handshake;
            _dtrEnable = dtrEnable;
            _readTimeout = readTimeout;
            _writeTimeout = writeTimeout;
            _encoding = encoding;

            _serialPort = SetSerialPort();
        }

        private static SerialPort _serialPort;
        private static string _portName;
        private static int _baudRate = 115200;
        private static Parity _parity = Parity.None;
        private static int _dataBits = 8;
        private static StopBits _stopBits = StopBits.One;
        private static Handshake _handshake = Handshake.None;
        private static bool _dtrEnable = true;
        private static int _readTimeout = 50;
        private static int _writeTimeout = 50;
        private static string _encodingWindows1252 = "Windows-1252";
        private static Encoding _encoding = System.Text.Encoding.GetEncoding(_encodingWindows1252); // System.Text.Encoding.Default;

        private static string _transactionCommandRequest;
        private static decimal _salesAmountDecimal = 0;
        private static string _STX = "02";
        private static string _MESSAGE_HEAD = string.Empty;
        private static int _MESSAGE_HEAD_LENGTH = 5;
        private static string _MESSAGE_BODY = string.Empty;
        private static int _MESSAGE_BODY_LENGTH = 0;
        private static string _LRC = string.Empty;

        private static decimal _creditCardReaderSoftwareVersion = 1.0m;
        private static string _MSG_LENGTH_HEX;
        private static string _VERSION = string.Empty;
        private static string _COMMAND = string.Empty;
        private static string _RROR_CODE = "00";
        private static string _TID_REQUEST = "PS60G454";

        /* Request */
        private static string _communicationPackageRequestHexStr = string.Empty;
        private static string _communicationPackageRequestHexStr2 = string.Empty;
        private static string _commandCodeRequest = string.Empty;
        private static string _commandCodeRequest2 = string.Empty;
        private static string _commandCodeRequestDefinition = string.Empty;
        private static string _commandCodeRequestDefinition2 = string.Empty;

        /* Response */
        private static List<Tuple<int, char, byte, string>> _responseCollectedTupleList;
        private static List<Tuple<int, char, byte, string>> _responseTupleList;
        private static List<Tuple<int, char, byte, string>> _responseTupleList2;
        private static List<Tuple<int, char, byte, string>> _responseTupleList3;
        private static AutoResetEvent _autoResetEventWaiter;
        private static AutoResetEvent _autoResetEventWaiter2;

        private static string _responsCharAsciiWindows1252strSpaceSeparated = string.Empty;
        private static string _responsCharAsciiWindows1252strSpaceSeparated2 = string.Empty;
        private static string _responsCharAsciiWindows1252strSpaceSeparated3 = string.Empty;
        private static string _responsByteStrSpaceSeparated = string.Empty;
        private static string _responsByteStrSpaceSeparated2 = string.Empty;
        private static string _responsByteStrSpaceSeparated3 = string.Empty;
        private static string _responseHexStrSpaceSeparated = string.Empty;
        private static string _responseHexStrSpaceSeparated2 = string.Empty;
        private static string _responseHexStrSpaceSeparated3 = string.Empty;

        private static string _commandCodeResponse = string.Empty;
        private static string _commandCodeResponse2 = string.Empty;
        private static string _commandCodeResponse3 = string.Empty;
        private static string _commandCodeResponseDefinition = string.Empty;
        private static string _commandCodeResponseDefinition2 = string.Empty;
        private static string _commandCodeResponseDefinition3 = string.Empty;
        private static string _errorCodeResponse = string.Empty;
        private static string _errorCodeResponse2 = string.Empty;
        private static string _errorCodeResponse3 = string.Empty;
        private static string _errorCodeResponseDefinition = string.Empty;
        private static string _errorCodeResponseDefinition2 = string.Empty;
        private static string _errorCodeResponseDefinition3 = string.Empty;
        private static long _timeStampResponse = 0;
        private static long _timeStampResponse2 = 0;
        private static long _timeStampResponse3 = 0;
        private static bool _isTransactionSucceeded = false;
        private static bool _isTransactionSucceeded2 = false;
        private static bool _isTransactionSucceeded3 = false;

        private static int _bytesCounter = 0;
        private static bool _isExitLooping = false;
        private static bool _isTimeout = false;

        private static TransactionResponse _transactionResponse;
        private static string _applicationErrorMessage = string.Empty;

        private static decimal _approvedSalesAmountDecimal = 0;
        private static string _terminalId16Bytes = string.Empty;
        private static int _cardTypeId2Bytes = -1;
        private static string _cardType = string.Empty;
        private static string _panCardNumber8Bytes = string.Empty;
        private static string _authCode8Bytes = string.Empty;
        private static string _sequenceNumber10Bytes = string.Empty;

        private static string _cardNumber21Bytes = string.Empty;
        private static string _expiryDate4Bytes = string.Empty;
        private static string _theReserved23Bytes = string.Empty;
        private static string _signedPackage20Bytes = string.Empty;

        private static string _recordNumber4Bytes = string.Empty;

        private SerialPort SetSerialPort()
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = _portName;
            _serialPort.BaudRate = _baudRate;
            _serialPort.Parity = _parity;
            _serialPort.DataBits = _dataBits;
            _serialPort.StopBits = _stopBits;
            _serialPort.Handshake = _handshake;
            _serialPort.DtrEnable = _dtrEnable;
            _serialPort.ReadTimeout = _readTimeout;
            _serialPort.WriteTimeout = _writeTimeout;
            _serialPort.Encoding = _encoding;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived); /* Register the event and attach a method to be called when there is data waiting in the port's buffer */

            return _serialPort;
        }

        private static void ResetResponseValues()
        {
            _responseCollectedTupleList = new List<Tuple<int, char, byte, string>>();
            _responseTupleList = new List<Tuple<int, char, byte, string>>();
            _autoResetEventWaiter = new AutoResetEvent(false);

            /* Request */
            _transactionCommandRequest = string.Empty;
            _communicationPackageRequestHexStr = string.Empty;
            _commandCodeRequest = string.Empty;
            _commandCodeRequestDefinition = string.Empty;

            /* Response */
            _responsCharAsciiWindows1252strSpaceSeparated = string.Empty;
            _responsByteStrSpaceSeparated = string.Empty;
            _responseHexStrSpaceSeparated = string.Empty;
            _commandCodeResponse = string.Empty;
            _commandCodeResponseDefinition = string.Empty;
            _errorCodeResponse = string.Empty;
            _errorCodeResponseDefinition = string.Empty;
            _timeStampResponse = 0;
            _isTransactionSucceeded = false;

            _bytesCounter = 0;
            _isExitLooping = false;
            _isTimeout = false;

            _transactionResponse = new TransactionResponse();
            _applicationErrorMessage = string.Empty;

            _approvedSalesAmountDecimal = 0;
            _terminalId16Bytes = string.Empty;
            _cardTypeId2Bytes = -1;
            _cardType = string.Empty;
            _panCardNumber8Bytes = string.Empty;
            _authCode8Bytes = string.Empty;
            _sequenceNumber10Bytes = string.Empty;

            _cardNumber21Bytes = string.Empty;
            _expiryDate4Bytes = string.Empty;
            _theReserved23Bytes = string.Empty;
            _signedPackage20Bytes = string.Empty;

            _recordNumber4Bytes = string.Empty;
        }

        private static void ResetResponseValues2()
        {
            _responseCollectedTupleList = new List<Tuple<int, char, byte, string>>();
            _responseTupleList2 = new List<Tuple<int, char, byte, string>>();
            _autoResetEventWaiter2 = new AutoResetEvent(false);

            /* Request */
            _transactionCommandRequest = string.Empty;
            _communicationPackageRequestHexStr2 = string.Empty;
            _commandCodeRequest2 = string.Empty;
            _commandCodeRequestDefinition2 = string.Empty;

            /* Response */
            _responsCharAsciiWindows1252strSpaceSeparated2 = string.Empty;
            _responsByteStrSpaceSeparated2 = string.Empty;
            _responseHexStrSpaceSeparated2 = string.Empty;
            _commandCodeResponse2 = string.Empty;
            _commandCodeResponseDefinition2 = string.Empty;
            _errorCodeResponse2 = string.Empty;
            _errorCodeResponseDefinition2 = string.Empty;
            _timeStampResponse2 = 0;
            _isTransactionSucceeded2 = false;

            _bytesCounter = 0;
            _isExitLooping = false;
            _isTimeout = false;
        }

        /* Appendix A: Command definition */
        public enum CommandEnum
        {
            SaleTransactionRequest
            , SaleTransactionResponse
            , ReversalTransactionRequest
            , ReversalTransactionResponse
            , HostDownloadRequest
            , HostDownloadResponse
            , SyncParametersRequest
            , SyncParametersResponse
            , CancelTransactionRequest
            , CancelTransactionResponse
            , GetDeviceInformationRequest
            , GetDeviceInformationResponse
            , SetDeviceInformationRequest
            , SetDeviceInformationResponse
            , SetNetworkRequest
            , SetNetworkResponse
            , NewSaleTransactionRequest
            , NewSaleTransactionResponse
            , GetStatusOfNewSaleTransactionRequest
            , GetStatusOfNewSaleTransactionResponse
            , GetResponseOfNewSaleTransactionRequest
            , GetResponseOfNewSaleTransactionResponse
            , NewSaleTransactionWithExtraInformationRequest
            , NewSaleTransactionWithExtraInformationResponse
            , DisplaysInformationWhileTerminalIsIdleRequest
            , DisplaysInformationWhileTerminalIsIdleResponse
            , DisplaysQRWhileTerminalIsIdleRequest
            , DisplaysQRWhileTerminalIsIdleResponse
            , SetHostIPURLAndPortRequest
            , SetHostIPURLAndPortResponse
            , ReadCardDataRequest
            , ReadCardDataResponse
            , ClearCardDataRequest
            , ClearCardDataResponse
            , PreAuthTransactionRequest
            , PreAuthTransactionResponse
            , PreAuthCompletionRequest
            , PreAuthCompletionResponse
            , CommandForSTXErrorORSomeInvalidLengthError
        }

        /* Appendix A: Command definition - COMMAND: 1 byte. Min value is 0x01, max value is 0xff. Please see Appendix A for detail. */
        private Dictionary<string, string> GetCommandDict()
        {
            Dictionary<string, string> commandDict = new Dictionary<string, string>();
            commandDict.Add(CommandEnum.SaleTransactionRequest.ToString(), "12");
            commandDict.Add(CommandEnum.SaleTransactionResponse.ToString(), "72");

            commandDict.Add(CommandEnum.ReversalTransactionRequest.ToString(), "13");
            commandDict.Add(CommandEnum.ReversalTransactionResponse.ToString(), "73");

            commandDict.Add(CommandEnum.HostDownloadRequest.ToString(), "14");
            commandDict.Add(CommandEnum.HostDownloadResponse.ToString(), "74");

            commandDict.Add(CommandEnum.SyncParametersRequest.ToString(), "15");
            commandDict.Add(CommandEnum.SyncParametersResponse.ToString(), "75");

            commandDict.Add(CommandEnum.CancelTransactionRequest.ToString(), "18");
            commandDict.Add(CommandEnum.CancelTransactionResponse.ToString(), "78");

            commandDict.Add(CommandEnum.GetDeviceInformationRequest.ToString(), "19");
            commandDict.Add(CommandEnum.GetDeviceInformationResponse.ToString(), "79");

            commandDict.Add(CommandEnum.SetDeviceInformationRequest.ToString(), "1A");
            commandDict.Add(CommandEnum.SetDeviceInformationResponse.ToString(), "7A");

            commandDict.Add(CommandEnum.SetNetworkRequest.ToString(), "1B");
            commandDict.Add(CommandEnum.SetNetworkResponse.ToString(), "7B");

            commandDict.Add(CommandEnum.NewSaleTransactionRequest.ToString(), "1F");
            commandDict.Add(CommandEnum.NewSaleTransactionResponse.ToString(), "7F");

            commandDict.Add(CommandEnum.GetStatusOfNewSaleTransactionRequest.ToString(), "20");
            commandDict.Add(CommandEnum.GetStatusOfNewSaleTransactionResponse.ToString(), "80");

            commandDict.Add(CommandEnum.GetResponseOfNewSaleTransactionRequest.ToString(), "21");
            commandDict.Add(CommandEnum.GetResponseOfNewSaleTransactionResponse.ToString(), "81");

            commandDict.Add(CommandEnum.NewSaleTransactionWithExtraInformationRequest.ToString(), "22");
            commandDict.Add(CommandEnum.NewSaleTransactionWithExtraInformationResponse.ToString(), "82");

            commandDict.Add(CommandEnum.DisplaysInformationWhileTerminalIsIdleRequest.ToString(), "23");
            commandDict.Add(CommandEnum.DisplaysInformationWhileTerminalIsIdleResponse.ToString(), "83");

            commandDict.Add(CommandEnum.DisplaysQRWhileTerminalIsIdleRequest.ToString(), "24");
            commandDict.Add(CommandEnum.DisplaysQRWhileTerminalIsIdleResponse.ToString(), "84");

            commandDict.Add(CommandEnum.SetHostIPURLAndPortRequest.ToString(), "25");
            commandDict.Add(CommandEnum.SetHostIPURLAndPortResponse.ToString(), "85");

            commandDict.Add(CommandEnum.ReadCardDataRequest.ToString(), "26");
            commandDict.Add(CommandEnum.ReadCardDataResponse.ToString(), "86");

            commandDict.Add(CommandEnum.ClearCardDataRequest.ToString(), "27");
            commandDict.Add(CommandEnum.ClearCardDataResponse.ToString(), "87");

            commandDict.Add(CommandEnum.PreAuthTransactionRequest.ToString(), "29");
            commandDict.Add(CommandEnum.PreAuthTransactionResponse.ToString(), "89");

            commandDict.Add(CommandEnum.PreAuthCompletionRequest.ToString(), "2A");
            commandDict.Add(CommandEnum.PreAuthCompletionResponse.ToString(), "8A");

            commandDict.Add(CommandEnum.CommandForSTXErrorORSomeInvalidLengthError.ToString(), "FF");

            return commandDict;
        }

        /*  Appendix B: Error code list - ERROR CODE: 1 byte. Min value is 0x00,max value is 0xff.ONLY for response. Please set to 0x00 for request. Please see Appendix B for detail. */
        private static Dictionary<string, string> GetErrorCodeDict()
        {
            Dictionary<string, string> errorCodeDict = new Dictionary<string, string>();
            errorCodeDict.Add("FF", "Operation completed successfully");
            errorCodeDict.Add("01", "Invalid length.Response package have head only.");
            errorCodeDict.Add("02", "The trans is cancelled by user");
            errorCodeDict.Add("03", "The trans communication error");
            errorCodeDict.Add("04", "Batch Full");
            errorCodeDict.Add("05", "Batch Empty");
            errorCodeDict.Add("06", "Invalid LRC.");
            errorCodeDict.Add("20", "Sale trans declined");
            errorCodeDict.Add("21", "Sale trans partial approve");
            errorCodeDict.Add("30", "Reversal trans declined");
            errorCodeDict.Add("31", "Init mode, don’t allow to do sale or reversal. Response package have head only.");
            errorCodeDict.Add("32", "Read card timeout. Response package have head only.");
            errorCodeDict.Add("33", "Read card failed. Response package have head only.");
            errorCodeDict.Add("34", "Command error.");
            errorCodeDict.Add("35", "Sale busy.");
            errorCodeDict.Add("36", "Cancel Sale Request rejected after read customer’s card info.");
            errorCodeDict.Add("37", "The first byte is not 02");
            errorCodeDict.Add("42", "Do host download failed");
            errorCodeDict.Add("43", "Sale failed");
            errorCodeDict.Add("44", "Set Device Information Failed");
            errorCodeDict.Add("45", "Set network modefailed");
            errorCodeDict.Add("46", "Invalid Parameter");
            errorCodeDict.Add("48", "There is None New Sale Transaction Processing.");
            errorCodeDict.Add("49", "New Sale Transaction is processing now.");
            errorCodeDict.Add("4A", "The last NewSale Transactiondidn’t get response, should get the last newtransaction response firstly.");
            errorCodeDict.Add("4B", "Card Not Support. Note: Only Support on Ultra VX and Ultra LX.");
            errorCodeDict.Add("4C", "Insert or Swipe not accepted.Note: Only support Tap card, not support insert card and swipe card");
            errorCodeDict.Add("4D", "unsupported payment method: For example, when a certain payment method of bank card, value card, mobile or coin is not enabled, the user who uses this payment method will reply this error code");
            errorCodeDict.Add("4E", "Now reading the card;This action cannot be performed |e.g. Sale /New Sale/Hostdownload/Reversal, etc.|");
            errorCodeDict.Add("4F", "Now the read is finished: This action cannot be performed |e.g.Sale / New Sale / Host Download / Reversal, etc.| Customer appneeds to clear the card number first.");
            errorCodeDict.Add("50", "Completion failed.");

            /* 0x51~0x60: Detailed code for "failed to read card" */
            errorCodeDict.Add("51", "Detailed code for |failed to read card| : FAULTURE");
            errorCodeDict.Add("52", "Detailed code for |failed to read card| : TIMEOUT");
            errorCodeDict.Add("53", "Detailed code for |failed to read card| : CARD_BLOCKED");
            errorCodeDict.Add("54", "Detailed code for |failed to read card| : APP_BLOCKED");
            errorCodeDict.Add("55", "Detailed code for |failed to read card| : APP_NOT_FOUND");
            errorCodeDict.Add("56", "Detailed code for |failed to read card| : NOT_ACCEPTED");

            return errorCodeDict;
        }

        /*  Appendix B: Error code list - ERROR CODE: 1 byte. Min value is 0x00,max value is 0xff.ONLY for response. Please set to 0x00 for request. Please see Appendix B for detail. */
        private static Dictionary<int, string> GetCardTypeDict()
        {
            Dictionary<int, string> cardTypeDict = new Dictionary<int, string>();
            cardTypeDict.Add(0, "Debit");
            cardTypeDict.Add(1, "Visa");
            cardTypeDict.Add(2, "Amex");
            cardTypeDict.Add(3, "MasterCard");
            cardTypeDict.Add(4, "Discover");
            cardTypeDict.Add(5, "DinersClub");
            cardTypeDict.Add(6, "JCB");
            cardTypeDict.Add(7, "Cart");
            cardTypeDict.Add(8, "EBT");
            cardTypeDict.Add(9, "Interac");

            return cardTypeDict;
        }

        /*  Appendix B: Error code list - ERROR CODE: 1 byte. Min value is 0x00,max value is 0xff.ONLY for response. Please set to 0x00 for request. Please see Appendix B for detail. */
        private static Dictionary<string, string> GetReadCardStatusDict()
        {
            Dictionary<string, string> readCardStatusDict = new Dictionary<string, string>();
            readCardStatusDict.Add("00", "No Card");
            readCardStatusDict.Add("01", "Reading (User tap card,VX reading)");
            readCardStatusDict.Add("02", "Read success");
            readCardStatusDict.Add("03", "Read Failed");

            return readCardStatusDict;
        }

        private static long GetTimeStampResponse()
        {
            string timeStampStr = DateTime.Now.ToString("yyyyMMddHHmmssfffff");
            long timeStampLong = Convert.ToInt64(timeStampStr);
            return timeStampLong;
        }

        public TransactionResponse SaleTransactionRequest(decimal salesTransactionAmount)
        {
            TransactionResponse transactionResponse = new TransactionResponse();
            try
            {
                ResetResponseValues();
                ResetResponseValues2();

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }

                _transactionCommandRequest = CommandEnum.SaleTransactionRequest.ToString();
                _commandCodeRequestDefinition = _transactionCommandRequest;
                _commandCodeRequest = GetCommandCodeHexValue(_transactionCommandRequest);

                _communicationPackageRequestHexStr = ConstructCommunicationPackageRequestHexStr(_transactionCommandRequest, 4, salesTransactionAmount);
                SendCommunicationPackageRequestHexStr(_communicationPackageRequestHexStr);
                #region [For Test]
                DisplayCancelTransactionRequestOptionMessage(); /* For Test */
                Console.WriteLine(String.Format("{0}: Waiting credit card reader response!", _transactionCommandRequest));
                //_autoResetEventWaiter.WaitOne(65000); //Thread.Sleep(61000);
                #endregion
                _transactionResponse = ParseSaleTransactionResponse();
                transactionResponse = _transactionResponse;
            }
            catch (Exception ex)
            {
                _applicationErrorMessage = _applicationErrorMessage + string.Format("| {0} - Error: {1}. |", _commandCodeRequestDefinition, ex.Message);
                transactionResponse = TransactionResponseObject();
            }
            finally
            {
                ResetResponseValues();
                ResetResponseValues2();
                _serialPort.Close();
                _autoResetEventWaiter.Set();
            }

            return transactionResponse;
        }

        private void DisplayCancelTransactionRequestOptionMessage()
        {
            Console.Clear();
            DisplayCancelTransactionRequestMessage();

            while (_isExitLooping == false)
            {
                if (_responseTupleList.Count == 0)
                {
                    Console.Write($"Pressed key: ");
                    string pressedKey = Console.ReadKey().Key.ToString();
                    Console.WriteLine();

                    if (pressedKey == "C")
                    {
                        Console.WriteLine("\nStart transaction!");
                        CancelTransactionRequest();
                        Console.WriteLine("End cancel transaction!");
                        _isExitLooping = true;
                    }
                    else
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"Pressed key: {pressedKey}, try again!\n");
                        Console.ResetColor();
                        DisplayCancelTransactionRequestMessage();
                    }
                }

                if (_isTimeout == true)
                {
                    _isExitLooping = true;
                    Console.ReadKey(true);
                    Console.WriteLine("Transaction timeout!");
                }
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(" Press any key to exit!");
            Console.ResetColor();
        }

        private void DisplayCancelTransactionRequestOptionMessage_()
        {
            DisplayCancelTransactionRequestMessage();

            while (_isExitLooping == false)
            {
                if (_responseTupleList.Count == 0)
                {   
                    bool isKeyAvailable = Console.KeyAvailable;

                    if (isKeyAvailable == true)
                    {
                        Console.WriteLine("Cancel transaction!");
                        CancelTransactionRequest();
                        Console.WriteLine("Transaction canceled!\n");
                        _isExitLooping = true;
                    }
                }

                if (_isTimeout == true)
                {
                    _isExitLooping = true;
                    Console.WriteLine("Transaction timeout!");
                }
            }
        }

        private void DisplayCancelTransactionRequestMessage()
        {
            Console.WriteLine("Options: ");
            Console.WriteLine("1 - Insert or swipe your credit card to complete transaction!");
            Console.WriteLine("2 - Press any key to cancel transaction!");
            Console.WriteLine();
        }

        private static string SplitCamelCaseString(string camelCaseString)
        {
            string str = Regex.Replace(camelCaseString, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
            return str;
        }

        public TransactionResponse CancelTransactionRequest()
        {
            try
            {
                ResetResponseValues2();

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }

                _transactionCommandRequest = CommandEnum.CancelTransactionRequest.ToString();
                _commandCodeRequestDefinition2 = _transactionCommandRequest;
                _commandCodeRequest2 = GetCommandCodeHexValue(_transactionCommandRequest);

                _communicationPackageRequestHexStr2 = ConstructCommunicationPackageRequestHexStr(_transactionCommandRequest, 0, 0);
                SendCommunicationPackageRequestHexStr(_communicationPackageRequestHexStr2);
                Console.WriteLine(String.Format("{0}: Waiting credit card reader response!", _transactionCommandRequest));
                _autoResetEventWaiter2.WaitOne(65000); 
                _transactionResponse = ParseSerialPortResponse2();
            }
            catch (Exception ex)
            {
                _applicationErrorMessage = _applicationErrorMessage + string.Format("| {0} - Error: {1}. |", _commandCodeRequestDefinition2, ex.Message);
                _transactionResponse = TransactionResponseObject();
            }
            finally
            {
                _serialPort.Close();
                _autoResetEventWaiter2.Set();
            }

            return _transactionResponse;
        }

        public TransactionResponse ReversalTransactionRequestCanada()
        {
            TransactionResponse transactionResponse = new TransactionResponse();
            try
            {
                ResetResponseValues();
                ResetResponseValues2();

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }

                _transactionCommandRequest = CommandEnum.ReversalTransactionRequest.ToString();
                _commandCodeRequestDefinition = _transactionCommandRequest;
                _commandCodeRequest = GetCommandCodeHexValue(_transactionCommandRequest);

                _communicationPackageRequestHexStr = ConstructCommunicationPackageRequestHexStr(_transactionCommandRequest, 4, 0);
                SendCommunicationPackageRequestHexStr(_communicationPackageRequestHexStr);
                Console.WriteLine(String.Format("{0}: Waiting credit card reader response!", _transactionCommandRequest));
                _autoResetEventWaiter.WaitOne(65000);
                _transactionResponse = ParseSerialPortResponse();
                transactionResponse = _transactionResponse;
            }
            catch (Exception ex)
            {
                _applicationErrorMessage = _applicationErrorMessage + string.Format("| {0} - Error: {1}. |", _commandCodeRequestDefinition, ex.Message);
                transactionResponse = TransactionResponseObject();
            }
            finally
            {
                ResetResponseValues();
                ResetResponseValues2();
                _serialPort.Close();
                _autoResetEventWaiter.Set();
            }

            return transactionResponse;
        }

        public TransactionResponse PreAuthTransactionRequestCanada(decimal salesTransactionAmount)
        {
            TransactionResponse transactionResponse = new TransactionResponse();
            try
            {
                ResetResponseValues();
                ResetResponseValues2();

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }

                _transactionCommandRequest = CommandEnum.PreAuthTransactionRequest.ToString();
                _commandCodeRequestDefinition = _transactionCommandRequest;
                _commandCodeRequest = GetCommandCodeHexValue(_transactionCommandRequest);

                _communicationPackageRequestHexStr = ConstructCommunicationPackageRequestHexStr(_transactionCommandRequest, 4, salesTransactionAmount);
                SendCommunicationPackageRequestHexStr(_communicationPackageRequestHexStr);
                #region [For Test]
                DisplayCancelTransactionRequestOptionMessage(); /* For Test */
                Console.WriteLine(String.Format("{0}: Waiting credit card reader response!", _transactionCommandRequest));
                //_autoResetEventWaiter.WaitOne(65000); //Thread.Sleep(61000);
                #endregion
                _transactionResponse = ParsePreAuthTransactionResponse();
                transactionResponse = _transactionResponse;
            }
            catch (Exception ex)
            {
                _applicationErrorMessage = _applicationErrorMessage + string.Format("| {0} - Error: {1}. |", _commandCodeRequestDefinition, ex.Message);
                transactionResponse = TransactionResponseObject();
            }
            finally
            {
                ResetResponseValues();
                ResetResponseValues2();
                _serialPort.Close();
                _autoResetEventWaiter.Set();
            }

            return transactionResponse;
        }

        private TransactionResponse PreAuthCompletionRequestCanada(decimal approvedSalesAmount, string recordNumber, string pan, string authorizationCode)
        {
            TransactionResponse transactionResponse = new TransactionResponse();
            try
            {
                ResetResponseValues();
                ResetResponseValues2();

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }

                _transactionCommandRequest = CommandEnum.PreAuthCompletionRequest.ToString();
                _commandCodeRequestDefinition = _transactionCommandRequest;
                _commandCodeRequest = GetCommandCodeHexValue(_transactionCommandRequest);

                _communicationPackageRequestHexStr = ConstructCommunicationPackageRequestHexStr(_transactionCommandRequest, 24, approvedSalesAmount, recordNumber, pan, authorizationCode);
                SendCommunicationPackageRequestHexStr(_communicationPackageRequestHexStr);
                Console.WriteLine(String.Format("{0}: Waiting credit card reader response!", _transactionCommandRequest));
                _autoResetEventWaiter.WaitOne(65000);
                _transactionResponse = ParseSerialPortResponse();;
                transactionResponse = _transactionResponse;
            }
            catch (Exception ex)
            {
                _applicationErrorMessage = _applicationErrorMessage + string.Format("| {0} - Error: {1}. |", _commandCodeRequestDefinition, ex.Message);
                transactionResponse = TransactionResponseObject();
            }
            finally
            {
                ResetResponseValues();
                ResetResponseValues2();
                _serialPort.Close();
                _autoResetEventWaiter.Set();
            }

            return transactionResponse;
        }

        private TransactionResponse ReadCardDataRequestCanada()
        {
            TransactionResponse transactionResponse = new TransactionResponse();
            try
            {
                ResetResponseValues();
                ResetResponseValues2();

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }

                _transactionCommandRequest = CommandEnum.ReadCardDataRequest.ToString();
                _commandCodeRequestDefinition = _transactionCommandRequest;
                _commandCodeRequest = GetCommandCodeHexValue(_transactionCommandRequest);
               
                /* Message body reserved fields: Fixed 8 bytes. This is a field for adding fields later. Fill the 0x00. */
                _communicationPackageRequestHexStr = ConstructCommunicationPackageRequestHexStr(_transactionCommandRequest, 8, 0);
                SendCommunicationPackageRequestHexStr(_communicationPackageRequestHexStr);
                Console.WriteLine(String.Format("{0}: Waiting credit card reader response!", _transactionCommandRequest));
                _autoResetEventWaiter.WaitOne(65000);
                _transactionResponse = ParseReadCardDataResponse();
                transactionResponse = _transactionResponse;
            }
            catch (Exception ex)
            {
                _applicationErrorMessage = _applicationErrorMessage + string.Format("| {0} - Error: {1}. |", _commandCodeRequestDefinition, ex.Message);
                transactionResponse = TransactionResponseObject();
            }
            finally
            {
                ResetResponseValues();
                ResetResponseValues2();
                _serialPort.Close();
                _autoResetEventWaiter.Set();
            }

            return transactionResponse;
        }

        private TransactionResponse ClearCardDataRequestCanada()
        {
            try
            {
                ResetResponseValues2();

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }

                _transactionCommandRequest = CommandEnum.ClearCardDataRequest.ToString();
                _commandCodeRequestDefinition2 = _transactionCommandRequest;
                _commandCodeRequest2 = GetCommandCodeHexValue(_transactionCommandRequest);

                /* Message body reserved fields: Fixed 8 bytes. This is a field for adding fields later. Fill the 0x00. */
                _communicationPackageRequestHexStr2 = ConstructCommunicationPackageRequestHexStr(_transactionCommandRequest, 8, 0);
                SendCommunicationPackageRequestHexStr(_communicationPackageRequestHexStr2);
                Console.WriteLine(String.Format("{0}: Waiting credit card reader response!", _transactionCommandRequest));
                _autoResetEventWaiter2.WaitOne(65000);
                _transactionResponse = ParseSerialPortResponse2();
            }
            catch (Exception ex)
            {
                _applicationErrorMessage = _applicationErrorMessage + string.Format("| {0} - Error: {1}. |", _commandCodeRequestDefinition2, ex.Message);
                _transactionResponse = TransactionResponseObject();
            }
            finally
            {
                _serialPort.Close();
                _autoResetEventWaiter2.Set();
            }

            return _transactionResponse;
        }

        public TransactionResponse SyncParametersRequest(string terminalIdRequest = "PS60G454")
        {
            TransactionResponse transactionResponse = new TransactionResponse();
            try
            {
                ResetResponseValues();
                ResetResponseValues2();

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }

                _transactionCommandRequest = CommandEnum.SyncParametersRequest.ToString();
                _commandCodeRequestDefinition = _transactionCommandRequest;
                _commandCodeRequest = GetCommandCodeHexValue(_transactionCommandRequest);

                /* This request is sent from Customer app to Payment App. Command value is 0x15.Message body is as follows: TID: 8 bytes, fixed length, use network byte order. */
                _TID_REQUEST = terminalIdRequest;
                _communicationPackageRequestHexStr = ConstructCommunicationPackageRequestHexStr(_transactionCommandRequest, 8, 0);
                SendCommunicationPackageRequestHexStr(_communicationPackageRequestHexStr);
                Console.WriteLine(String.Format("{0}: Waiting credit card reader response!", _transactionCommandRequest));
                _autoResetEventWaiter.WaitOne(65000);
                _transactionResponse = ParseSerialPortResponse();
                transactionResponse = _transactionResponse;
            }
            catch (Exception ex)
            {
                _applicationErrorMessage = _applicationErrorMessage + string.Format("| {0} - Error: {1}. |", _commandCodeRequestDefinition, ex.Message);
                transactionResponse = TransactionResponseObject();
            }
            finally
            {
                ResetResponseValues();
                ResetResponseValues2();
                _serialPort.Close();
                _autoResetEventWaiter.Set();
            }

            return transactionResponse;
        }

        public TransactionResponse HostDownloadRequest()
        {
            TransactionResponse transactionResponse = new TransactionResponse();
            try
            {
                ResetResponseValues();
                ResetResponseValues2();

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }

                _transactionCommandRequest = CommandEnum.HostDownloadRequest.ToString();
                _commandCodeRequestDefinition = _transactionCommandRequest;
                _commandCodeRequest = GetCommandCodeHexValue(_transactionCommandRequest);

                /* This request is sent from Customer app to Payment App. Command value is 0x14. This request has no message body. */
                _communicationPackageRequestHexStr = ConstructCommunicationPackageRequestHexStr(_transactionCommandRequest, 0, 0);
                SendCommunicationPackageRequestHexStr(_communicationPackageRequestHexStr);
                Console.WriteLine(String.Format("{0}: Waiting credit card reader response!", _transactionCommandRequest));
                _autoResetEventWaiter.WaitOne(65000);
                _transactionResponse = ParseSerialPortResponse();
                transactionResponse = _transactionResponse;
            }
            catch (Exception ex)
            {
                _applicationErrorMessage = _applicationErrorMessage + string.Format("| {0} - Error: {1}. |", _commandCodeRequestDefinition, ex.Message);
                transactionResponse = TransactionResponseObject();
            }
            finally
            {
                ResetResponseValues();
                ResetResponseValues2();
                _serialPort.Close();
                _autoResetEventWaiter.Set();
            }

            return transactionResponse;
        }

        private TransactionResponse TestCommunicationPackageRequestHexStr(CommandEnum transactionCommandRequest, string communicationPackageRequestHexStr)
        {
            TransactionResponse transactionResponse = new TransactionResponse();
            try
            {
                ResetResponseValues();
                ResetResponseValues2();

                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }

                _transactionCommandRequest = transactionCommandRequest.ToString();
                _commandCodeRequestDefinition = _transactionCommandRequest;
                _commandCodeRequest = GetCommandCodeHexValue(_transactionCommandRequest);

                _communicationPackageRequestHexStr = communicationPackageRequestHexStr;
                SendCommunicationPackageRequestHexStr(_communicationPackageRequestHexStr);
                Console.WriteLine(String.Format("{0}: Waiting credit card reader response!", _transactionCommandRequest));
                _autoResetEventWaiter.WaitOne(65000);
                _transactionResponse = ParseSerialPortResponse();
                transactionResponse = _transactionResponse;
            }
            catch (Exception ex)
            {
                _applicationErrorMessage = _applicationErrorMessage + string.Format("| {0} - Error: {1}. |", _commandCodeRequestDefinition, ex.Message);
                transactionResponse = TransactionResponseObject();
            }
            finally
            {
                ResetResponseValues();
                ResetResponseValues2();
                _serialPort.Close();
                _autoResetEventWaiter.Set();
            }

            return transactionResponse;
        }

        /* Package Format: STX|MESSAGE HEAD|MESSAGE BODY|LRC */
        private string ConstructCommunicationPackageRequestHexStr(string transactionCommandRequest, int MESSAGE_BODY_LENGTH, decimal salesAmountDecimal, string recordNumber = "", string pan = "", string authorizationCode = "")
        {
            _MESSAGE_BODY_LENGTH = MESSAGE_BODY_LENGTH;
            _salesAmountDecimal = Convert.ToInt32(salesAmountDecimal * 100);

            int salesAmount = Convert.ToInt32(_salesAmountDecimal);
            string amountHexStrValue = ConverterDecimalValueToHexStrValue(salesAmount);

            if (transactionCommandRequest == CommandEnum.PreAuthCompletionRequest.ToString())
            {
                string amountHexStrValue_MESSAGE_BODY = ConstructMessageBody(amountHexStrValue, 4 * 2);

                string recordNumber_MESSAGE_BODY = string.Empty;
                foreach (char chr in recordNumber.Replace(" ", "\0").ToCharArray())
                {
                    string recordNumberHexStrValue = ConverterDecimalValueToHexStrValue((int)chr);
                    recordNumber_MESSAGE_BODY = recordNumber_MESSAGE_BODY + " " + recordNumberHexStrValue;
                }

                string pan_MESSAGE_BODY = string.Empty;
                foreach (char chr in pan.Replace(" ", "\0").ToCharArray())
                {
                    string panHexStrValue = ConverterDecimalValueToHexStrValue((int)chr);
                    pan_MESSAGE_BODY = pan_MESSAGE_BODY + " " + panHexStrValue;
                }

                string authorizationCode_MESSAGE_BODY = string.Empty;
                foreach (char chr in authorizationCode.Replace(" ", "\0").ToCharArray())
                {
                    string authorizationCodeHexStrValue = ConverterDecimalValueToHexStrValue((int)chr);
                    authorizationCode_MESSAGE_BODY = authorizationCode_MESSAGE_BODY + " " + authorizationCodeHexStrValue;
                }

                _MESSAGE_BODY = string.Format("{0} {1} {2} {3}", amountHexStrValue_MESSAGE_BODY.Trim(), recordNumber_MESSAGE_BODY.Trim(), pan_MESSAGE_BODY.Trim(), authorizationCode_MESSAGE_BODY.Trim());
            }
            else if (transactionCommandRequest == CommandEnum.SyncParametersRequest.ToString())
            {
                byte[] byteArray = ConvertAsciiWindows1252strToByteArray(_TID_REQUEST);
                string terminalIdRequestSpaceSeparated = ConvertAsciiWindows1252byteArrayToHexStr(byteArray);

                _MESSAGE_BODY = string.Format("{0}", terminalIdRequestSpaceSeparated.Trim());
            }
            else
            {
                _MESSAGE_BODY = ConstructMessageBody(amountHexStrValue, MESSAGE_BODY_LENGTH * 2);
            }

            int msgLength = GetMsgLength(_MESSAGE_HEAD_LENGTH, MESSAGE_BODY_LENGTH);
            byte msgLengthByteValue = ConvertAsciiWindows1252charValueToByteValue((char)msgLength);
            string hexValue = ConvertAsciiWindows1252byteValueToHexValue(msgLengthByteValue);
            _MSG_LENGTH_HEX = string.Format("00 {0}", hexValue);
            _VERSION = GetVersionHexStr();
            _COMMAND = GetCommandCodeHexValue(transactionCommandRequest);
            _RROR_CODE = "00";
            /* Message Head Definition: MSG LENGTH|VERSION| COMMAND | ERROR CODE */
            _MESSAGE_HEAD = string.Format("{0} {1} {2} {3}", _MSG_LENGTH_HEX, _VERSION, _COMMAND, _RROR_CODE);
            string MESSAGE_HEAD_MESSAGE_BODY = string.Format("{0} {1}", _MESSAGE_HEAD, _MESSAGE_BODY.Trim()).Trim();
            byte[] lrcByteArray = ConvertHexStringToByteArray(MESSAGE_HEAD_MESSAGE_BODY);
            byte lrcByteValue = CalculateLRC(lrcByteArray);
            _LRC = ConverterDecimalValueToHexStrValue(lrcByteValue);

            string communicationPackageRequestHexStr = string.Format("{0} {1} {2}", _STX, MESSAGE_HEAD_MESSAGE_BODY, _LRC);
            return communicationPackageRequestHexStr;
        }

        private static string ConverterDecimalValueToHexStrValue(int dec)
        {
            string hexStr = dec.ToString("X2");
            return hexStr;
        }

        private static string GetSpaceSeparatedString(string inputStr)
        {
            string spaceSeparatedString = string.Empty;
            char[] charArray = inputStr.ToCharArray();

            foreach (char chr in charArray)
            {
                spaceSeparatedString = spaceSeparatedString + " " + chr;
            }

            return spaceSeparatedString.Trim();
        }

        private static string ConstructMessageBody(string amountHex, int length)
        {
            string str = "";
            string s = "";

            for (int i = 0; i < length - amountHex.Length; i++)
            {
                s = s + "0";
            }

            s = s + amountHex;

            char[] charArray = s.ToCharArray();

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    str = str + charArray[i++];
                }

                str = str + " ";
                i--;
            }

            return str.Trim();
        }

        /* MSG LENGTH:2 bytes, indicate message length, include |MESSAGE HEAD| length and |MESSAGE BODY| length, use network order byte. Max length is 65535. */
        private static int GetMsgLength(int messageHeadLength, int messageBodyLength)
        {
            int msgLength = messageHeadLength + messageBodyLength;
            if (msgLength > 65535 || msgLength < 0)
            {
                _applicationErrorMessage = string.Format("MSG LENGTH:2 bytes, indicate message length, include |MESSAGE HEAD| length and |MESSAGE BODY| length. Max length is 65535. The current |MSG LENGTH| value is: {0}.", msgLength);
            }

            return msgLength;
        }

        private static byte ConvertAsciiWindows1252charValueToByteValue(char asciiWindows1252charValue)
        {
            byte byteValue = Encoding.GetEncoding(_encodingWindows1252).GetBytes(asciiWindows1252charValue.ToString()).FirstOrDefault();
            return byteValue;
        }

        private static byte[] ConvertAsciiWindows1252strToByteArray(string asciiWindows1252str)
        {
            byte[] byteArray = Encoding.GetEncoding(_encodingWindows1252).GetBytes(asciiWindows1252str);
            return byteArray;
        }

        private static char[] GetHexCharacterArray()
        {
            char[] hexCharacterArray = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            return hexCharacterArray;
        }

        private static string ConvertAsciiWindows1252byteValueToHexValue(byte asciiWindows1252byteValue)
        {
            char[] hexCharacterArray = GetHexCharacterArray();

            int firstValue = (asciiWindows1252byteValue >> 4) & 0x0F;
            int secondValue = asciiWindows1252byteValue & 0x0F;

            char firstCharacter = hexCharacterArray[firstValue];
            char secondCharacter = hexCharacterArray[secondValue];

            string hexStrValue = string.Format($"{firstCharacter}{secondCharacter}");
            return hexStrValue;
        }

        private static List<Tuple<int, char, byte, string>> ConvertAsciiWindows1252strToTupleList(string asciiWindows1252str)
        {
            List<Tuple<int, char, byte, string>> responseTupleList = new List<Tuple<int, char, byte, string>>();
            char[] charArray = asciiWindows1252str.ToCharArray();

            for (int i = 0; i < charArray.Length; i++)
            {
                byte asciiWindows1252byteValue = ConvertAsciiWindows1252charValueToByteValue(charArray[i]);
                string hexStrValue = ConvertAsciiWindows1252byteValueToHexValue(asciiWindows1252byteValue);

                Tuple<int, char, byte, string> responseTuple = new Tuple<int, char, byte, string>(++_bytesCounter, charArray[i], asciiWindows1252byteValue, hexStrValue);
                responseTupleList.Add(responseTuple);
            }

            return responseTupleList;
        }

        private static string ConvertAsciiWindows1252byteArrayToHexStr(byte[] asciiWindows1252byteArray)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (var i = 0; i < asciiWindows1252byteArray.Length; i++)
            {
                string hexStrValue = ConvertAsciiWindows1252byteValueToHexValue(asciiWindows1252byteArray[i]);

                //stringBuilder.Append("0x");
                stringBuilder.Append(hexStrValue);
                stringBuilder.Append(' ');
            }

            return stringBuilder.ToString().Trim(' ');
        }

        /* version number (max value is 9), low 4 bits indicate revised version number (max value is 9). Min value is 0x10 (means version: 1.0), max value is 0x99 (means version: 9.9), and current value is 0x10. */
        private static string GetVersionHexStr()
        {
            int versionDecimal = (int)(_creditCardReaderSoftwareVersion * 10);
            if (versionDecimal > 99 || versionDecimal < 10)
            {
               _applicationErrorMessage = string.Format("Version number Min value is 0x10 (means version: 1.0), max value is 0x99 (means version: 9.9). The current passed value is: {0}.", _creditCardReaderSoftwareVersion);
            }

            return versionDecimal.ToString();
        }

        /* COMMAND: 1 byte. Min value is 0x01, max value is 0xff. Please see Appendix A for detail. */
        private string GetCommandCodeHexValue(string transactionCommandRequest)
        {
            string commandCodeHexValue = string.Empty;
            if (GetCommandDict().Keys.Contains(transactionCommandRequest))
            {
                commandCodeHexValue = GetCommandDict().FirstOrDefault(x => x.Key == transactionCommandRequest).Value;
            }
            else
            {
                _applicationErrorMessage = string.Format("There is no key value for the given transactionCommandRequest: {0}", transactionCommandRequest);
            }

            return commandCodeHexValue;
        }

        private string GetCommandCodeDefinition(string commandCodeHexValue)
        {
            string transactionCommandRequest = string.Empty;
            if (GetCommandDict().Values.Contains(commandCodeHexValue))
            {
                transactionCommandRequest = GetCommandDict().FirstOrDefault(x => x.Value == commandCodeHexValue).Key.ToString();
            }
            else
            {
                _applicationErrorMessage = string.Format("There is no transactionCommandRequest key for the given commandCodeHexValue: {0}", commandCodeHexValue);
            }

            return transactionCommandRequest;
        }

        private string GetErrorCodeResponseDefinition(string errorCodeResponse)
        {
            string errorCodeResponseDefinition = string.Empty;
            if (GetErrorCodeDict().Keys.Contains(errorCodeResponse))
            {
                errorCodeResponseDefinition = GetErrorCodeDict().FirstOrDefault(x => x.Key == errorCodeResponse).Value;
            }
            else
            {
                _applicationErrorMessage = string.Format("There is no errorCodeResponseDefinition value for the given errorCodeResponse key: {0}", errorCodeResponse);
            }

            return errorCodeResponseDefinition;
        }

        private static byte[] ConvertHexStringToByteArray(string hexString)
        {
            string[] strArray = hexString.Split(' ');
            byte[] byteArray = new byte[strArray.Length];

            for (int i = 0; i < strArray.Length; i++)
            {
                byte byteValue = ConvertHexStrValueToByteValue(strArray[i]);
                byteArray[i] = byteValue;
            }

            return byteArray;
        }

        private static byte ConvertHexStrValueToByteValue(string hexStrValue)
        {
            int intValue = Convert.ToInt32(hexStrValue, 16);
            byte byteValue = (byte)intValue;
            return byteValue;
        }

        private static int ConvertHexStrValueToDecimalValue(string hexStrValue)
        {
            int intValue = Convert.ToInt32(hexStrValue, 16);
            return intValue;
        }

        private static char[] ConvertByteArrayToCharArray(byte[] byteArray)
        {
            char[] charArray = Encoding.GetEncoding(_encodingWindows1252).GetChars(byteArray);
            return charArray;
        }

        /* LRC: Calculation of LRC from |MESSAGE HEAD| first byte to |MESSAGE BODY| last byte */
        private static byte CalculateLRC(byte[] bytes)
        {
            int LRC = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                LRC ^= bytes[i];
            }

            byte LRCbyte = new byte();
            LRCbyte = (byte)LRC;
            return LRCbyte;
        }

        private static void SendCommunicationPackageRequestHexStr(string communicationPackageRequestHexStr)
        {
            byte[] communicationPackageRequestByteArray = ConvertHexStringToByteArray(communicationPackageRequestHexStr);
            _serialPort.Write(communicationPackageRequestByteArray, 0, communicationPackageRequestByteArray.Length);
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = sender as SerialPort;
            string allIncomingAsciiWindows1252str = serialPort.ReadExisting();

            if (allIncomingAsciiWindows1252str != null && allIncomingAsciiWindows1252str.Length > 0)
            {
                List<Tuple<int, char, byte, string>> responseTupleList = ConvertAsciiWindows1252strToTupleList(allIncomingAsciiWindows1252str);

                _responseCollectedTupleList.AddRange(responseTupleList);

                string responsCharAsciiWindows1252strSpaceSeparated = string.Join(" ", _responseCollectedTupleList.Select(x => x.Item2));
                string responsByteStrSpaceSeparated = string.Join(" ", _responseCollectedTupleList.Select(x => x.Item3));
                string responseHexStrSpaceSeparated = string.Join(" ", _responseCollectedTupleList.Select(x => x.Item4));

                if (_responseCollectedTupleList.Count >= 7)
                {
                    string commandCodeResponse = string.Join(" ", _responseCollectedTupleList.Where(x => x.Item1 == 5).Select(x => x.Item4));

                    if (GetCommandDict().Values.Contains(commandCodeResponse))
                    {
                        string commandCodeResponseDefinition = GetCommandCodeDefinition(commandCodeResponse);

                        if (commandCodeResponseDefinition == CommandEnum.SaleTransactionResponse.ToString() && _responseCollectedTupleList.Count == 55 || 
                            commandCodeResponseDefinition == CommandEnum.SaleTransactionResponse.ToString() && _responseCollectedTupleList.Count == 62)
                        {
                            _timeStampResponse = GetTimeStampResponse();
                            _transactionCommandRequest = CommandEnum.SaleTransactionRequest.ToString();

                            if (_responseCollectedTupleList.Count == 55)
                            {
                                _responseTupleList = _responseCollectedTupleList;
                            }
                            else if (_responseCollectedTupleList.Count == 62)
                            {
                                _responseTupleList = _responseCollectedTupleList.Take(55).ToList();

                                List<Tuple<int, char, byte, string>> responseCollectedTupleList2 = _responseCollectedTupleList.GetRange(55, 7);
                                _bytesCounter = 0;
                                _responseTupleList2 = new List<Tuple<int, char, byte, string>>();

                                responseCollectedTupleList2.ForEach(x =>
                                {
                                    Tuple<int, char, byte, string> responseTuple = new Tuple<int, char, byte, string>(++_bytesCounter, x.Item2, x.Item3, x.Item4);
                                    _responseTupleList2.Add(responseTuple);
                                });
                            }

                            string errorCodeResponse = string.Join(" ", _responseCollectedTupleList.Where(x => x.Item1 == 6).Select(x => x.Item4));
                            if (errorCodeResponse == "32")
                            {
                                _isTimeout = true;
                            }

                            _autoResetEventWaiter.Set();
                            _autoResetEventWaiter2.Set();
                            _isExitLooping = true;
                            _bytesCounter = 0;
                            _responseCollectedTupleList = new List<Tuple<int, char, byte, string>>();

                            _transactionResponse = ParseSaleTransactionResponse();
                        }

                        if (commandCodeResponseDefinition == CommandEnum.CancelTransactionResponse.ToString() && _responseCollectedTupleList.Count == 7 || 
                            commandCodeResponseDefinition == CommandEnum.CancelTransactionResponse.ToString() && _responseCollectedTupleList.Count == 62)
                        {
                            _timeStampResponse2 = GetTimeStampResponse();
                            _transactionCommandRequest = CommandEnum.CancelTransactionRequest.ToString();

                            if (_responseCollectedTupleList.Count == 7)
                            {
                                _responseTupleList2 = _responseCollectedTupleList;
                            }
                            else if (_responseCollectedTupleList.Count == 62)
                            {
                                _responseTupleList2 = _responseCollectedTupleList.Take(7).ToList();

                                List<Tuple<int, char, byte, string>> responseCollectedTupleList = _responseCollectedTupleList.GetRange(7, 55);
                                _bytesCounter = 0;
                                _responseTupleList = new List<Tuple<int, char, byte, string>>();

                                responseCollectedTupleList.ForEach(x =>
                                {
                                    Tuple<int, char, byte, string> responseTuple = new Tuple<int, char, byte, string>(++_bytesCounter, x.Item2, x.Item3, x.Item4);
                                    _responseTupleList.Add(responseTuple);
                                });

                                _isExitLooping = true;
                            }

                            if (string.IsNullOrEmpty(_commandCodeRequestDefinition))
                            {
                                _autoResetEventWaiter2.Set();
                            }

                            //if (string.IsNullOrEmpty(_commandCodeRequestDefinition) || _responseTupleList.Count > 0)
                            //{
                            //    _autoResetEventWaiter2.Set();
                            //}

                            //if (_commandCodeRequestDefinition == CommandEnum.SaleTransactionRequest.ToString() && _responseTupleList.Count > 0)
                            //{
                            //    _transactionResponse = ParseSaleTransactionResponse();
                            //}

                            //if (_commandCodeRequestDefinition == CommandEnum.PreAuthTransactionRequest.ToString() && _responseTupleList.Count > 0)
                            //{
                            //    _transactionResponse = ParsePreAuthTransactionResponse();
                            //}

                            _bytesCounter = 0;
                            _responseCollectedTupleList = new List<Tuple<int, char, byte, string>>();

                            _transactionResponse = ParseSerialPortResponse2();
                        }


                        //69
                        //"\u0002 \0 \u0005 \u0010 ÿ 7 Ý \u0002 \0 \u0005 \u0010 x ÿ ’ \u0002 \0 5 \u0010 r \u0002 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 ÿ \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 \0 ª"
                        //"2 0 5 16 255 55 221 2 0 5 16 120 255 146 2 0 53 16 114 2 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 255 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 170"
                        //"02 00 05 10 FF 37 DD 02 00 05 10 78 FF 92 02 00 35 10 72 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 AA"

                        if (commandCodeResponseDefinition == CommandEnum.CommandForSTXErrorORSomeInvalidLengthError.ToString() && _responseCollectedTupleList.Count == 7 ||
                            commandCodeResponseDefinition == CommandEnum.CommandForSTXErrorORSomeInvalidLengthError.ToString() && _responseCollectedTupleList.Count == 69)
                        {
                            _timeStampResponse3 = GetTimeStampResponse();

                            if (_responseCollectedTupleList.Count == 7)
                            {
                                _responseTupleList3 = _responseCollectedTupleList;
                            }
                            else if (_responseCollectedTupleList.Count == 69)
                            {
                                _responseTupleList3 = _responseCollectedTupleList.Take(7).ToList();

                                List<Tuple<int, char, byte, string>> responseCollectedTupleList2 = _responseCollectedTupleList.GetRange(7, 7);
                                _bytesCounter = 0;
                                _responseTupleList2 = new List<Tuple<int, char, byte, string>>();

                                responseCollectedTupleList2.ForEach(x =>
                                {
                                    Tuple<int, char, byte, string> responseTuple = new Tuple<int, char, byte, string>(++_bytesCounter, x.Item2, x.Item3, x.Item4);
                                    _responseTupleList2.Add(responseTuple);
                                });

                                List<Tuple<int, char, byte, string>> responseCollectedTupleList3 = _responseCollectedTupleList.GetRange((7 + 7) , 69);
                                _bytesCounter = 0;
                                _responseTupleList3 = new List<Tuple<int, char, byte, string>>();

                                responseCollectedTupleList3.ForEach(x =>
                                {
                                    Tuple<int, char, byte, string> responseTuple = new Tuple<int, char, byte, string>(++_bytesCounter, x.Item2, x.Item3, x.Item4);
                                    _responseTupleList.Add(responseTuple);
                                });

                                _isExitLooping = true;
                            }

                            if (_commandCodeRequestDefinition != CommandEnum.SaleTransactionRequest.ToString() ||
                                _commandCodeRequestDefinition != CommandEnum.PreAuthCompletionRequest.ToString() ||
                                _responseTupleList.Count > 0)
                            {
                                _autoResetEventWaiter2.Set();
                            }

                            _bytesCounter = 0;
                            _responseCollectedTupleList = new List<Tuple<int, char, byte, string>>();

                            if (_commandCodeRequestDefinition == CommandEnum.SaleTransactionRequest.ToString() && _responseTupleList.Count > 0)
                            {
                                _transactionResponse = ParseSaleTransactionResponse();
                            }

                            if (_commandCodeRequestDefinition == CommandEnum.PreAuthTransactionRequest.ToString() && _responseTupleList.Count > 0)
                            {
                                _transactionResponse = ParsePreAuthTransactionResponse();
                            }

                            if (_commandCodeRequestDefinition == CommandEnum.CancelTransactionRequest.ToString() && _responseTupleList.Count > 0)
                            {
                                _transactionResponse = ParseSerialPortResponse2();
                            }

                            _transactionResponse = ParseSerialPortResponse3();
                        }

                        if (commandCodeResponseDefinition == CommandEnum.ReversalTransactionResponse.ToString() && _responseCollectedTupleList.Count == 7)
                        {
                            _timeStampResponse = GetTimeStampResponse();
                            _transactionCommandRequest = CommandEnum.ReversalTransactionRequest.ToString();
                            _responseTupleList = _responseCollectedTupleList;
                            _autoResetEventWaiter.Set();
                            _isExitLooping = true;

                            _transactionResponse = ParseSerialPortResponse();
                        }

                        if (commandCodeResponseDefinition == CommandEnum.PreAuthTransactionResponse.ToString() && _responseCollectedTupleList.Count == 59)
                        {
                            _timeStampResponse = GetTimeStampResponse();
                            _transactionCommandRequest = CommandEnum.PreAuthTransactionRequest.ToString();
                            _responseTupleList = _responseCollectedTupleList;
                            _autoResetEventWaiter.Set();
                            _isExitLooping = true;

                            _transactionResponse = ParsePreAuthTransactionResponse();
                        }

                        if (commandCodeResponseDefinition == CommandEnum.SyncParametersResponse.ToString() && _responseCollectedTupleList.Count == 7)
                        {
                            _timeStampResponse = GetTimeStampResponse();
                            _transactionCommandRequest = CommandEnum.SyncParametersRequest.ToString();
                            _responseTupleList = _responseCollectedTupleList;
                            _autoResetEventWaiter.Set();
                            _isExitLooping = true;

                            _transactionResponse = ParseSerialPortResponse();
                        }

                        if (commandCodeResponseDefinition == CommandEnum.HostDownloadResponse.ToString() && _responseCollectedTupleList.Count == 7)
                        {
                            _timeStampResponse = GetTimeStampResponse();
                            _transactionCommandRequest = CommandEnum.HostDownloadRequest.ToString();
                            _responseTupleList = _responseCollectedTupleList;
                            _autoResetEventWaiter.Set();
                            _isExitLooping = true;

                            _transactionResponse = ParseSerialPortResponse();
                        }

                        if (commandCodeResponseDefinition == CommandEnum.PreAuthCompletionResponse.ToString() && _responseCollectedTupleList.Count == 14)
                        {
                            _timeStampResponse2 = GetTimeStampResponse();
                            _transactionCommandRequest = CommandEnum.PreAuthCompletionRequest.ToString();
                            _autoResetEventWaiter.Set();
                            _isExitLooping = true;
                        }

                        if (commandCodeResponseDefinition == CommandEnum.ReadCardDataResponse.ToString() && _responseCollectedTupleList.Count == 76)
                        {
                            _timeStampResponse = GetTimeStampResponse();
                            TransactionResponse transactionResponse = ClearCardDataRequestCanada();

                            _transactionCommandRequest = CommandEnum.ReadCardDataRequest.ToString();
                            _autoResetEventWaiter.Set();
                            _isExitLooping = true;

                            _transactionResponse = ParseReadCardDataResponse();
                        }

                        if (commandCodeResponseDefinition == CommandEnum.ClearCardDataResponse.ToString() && _responseCollectedTupleList.Count == 15)
                        {
                            _timeStampResponse2 = GetTimeStampResponse();
                            _transactionCommandRequest = CommandEnum.ReadCardDataRequest.ToString();
                            _autoResetEventWaiter2.Set();
                            _isExitLooping = true;

                            _transactionResponse = ParseSerialPortResponse2();
                        }
                    }
                    else
                    {
                        _responsCharAsciiWindows1252strSpaceSeparated3 = string.Join(" ", _responseCollectedTupleList.Select(x => x.Item2));
                        _responseHexStrSpaceSeparated3 = string.Join(" ", _responseCollectedTupleList.Select(x => x.Item4));
                       
                        _isExitLooping = true;
                        _autoResetEventWaiter.Set();
                        _autoResetEventWaiter2.Set();

                        _transactionResponse = ParseSerialPortResponse();
                        _applicationErrorMessage = string.Format("There is no transactionCommand key definition for the given response commandCodeHexValue: {0}", commandCodeResponse);
                    }
                }

                if (_responseTupleList != null && _responseTupleList.Count > 0)
                {
                    _responsCharAsciiWindows1252strSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item2));
                    _responsByteStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item3));
                    _responseHexStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item4));
                }

                if (_responseTupleList2 != null && _responseTupleList2.Count > 0)
                {
                    _responsCharAsciiWindows1252strSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item2));
                    _responsByteStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item3));
                    _responseHexStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item4));
                }

                if (_responseTupleList3 != null && _responseTupleList3.Count > 0)
                {
                    _responsCharAsciiWindows1252strSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item2));
                    _responsByteStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item3));
                    _responseHexStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item4));
                }

            }

        }

        private TransactionResponse ParseSaleTransactionResponse()
        {
            if (_responseTupleList2 != null && _responseTupleList2.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item2));
                _responsByteStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item3));
                _responseHexStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item4));
            }

            if (_responseTupleList3 != null && _responseTupleList3.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item2));
                _responsByteStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item3));
                _responseHexStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item4));
            }

            if (_responseTupleList != null && _responseTupleList.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item2));
                _responsByteStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item3));
                _responseHexStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item4));

                int currentCount = 0;

                /* Command Value is 0x72: 7 bytes, fixed length */
                List<Tuple<int, char, byte, string>> responseCode6BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 6).ToList();
                currentCount = currentCount + 6;

                if (_transactionCommandRequest == CommandEnum.SaleTransactionRequest.ToString() && _responseTupleList.Count == 55)
                {
                    /* Approved amount: 4 bytes, fixed length */
                    List<Tuple<int, char, byte, string>> approvedAmount4BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 4).ToList();
                    currentCount = currentCount + 4;

                    /* TID: 16 bytes, fixed length. This is host’ sassigned 16 bytes Terminal ID, unique value per terminal, used by host. */
                    List<Tuple<int, char, byte, string>> terminalId16BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 16).ToList();
                    currentCount = currentCount + 16;

                    /* Card ID: 2 bytes, fixed length. This identifies the card type based on the PAN/AID */
                    List<Tuple<int, char, byte, string>> cardTypeId2BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 2).ToList();
                    currentCount = currentCount + 2;

                    /* PAN: 8 bytes, fixed length. This is masked PAN of the card number. First 4 and last 4 digits are included as part of the response */
                    List<Tuple<int, char, byte, string>> panCardNumber8BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 8).ToList();
                    currentCount = currentCount + 8;

                    /* Authorization Code: 8 bytes, fixed length. This is the authorization code returned by host */
                    List<Tuple<int, char, byte, string>> authCode8BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 8).ToList();
                    currentCount = currentCount + 8;

                    /* Sequence Number: 10 bytes, fixed length.This is the sequence number returned by host. */
                    List<Tuple<int, char, byte, string>> sequenceNumber10BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 10).ToList();
                    currentCount = currentCount + 10;

                    /* Last 1 byte */
                    List<Tuple<int, char, byte, string>> last1BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 1).ToList();
                    currentCount = currentCount + 1;

                    _approvedSalesAmountDecimal = ConvertHexStrValueToDecimalValue(string.Join("", approvedAmount4BytesTupleList.Select(x => x.Item4)));
                    _terminalId16Bytes = string.Join("", terminalId16BytesTupleList.Select(x => x.Item2));
                    _cardTypeId2Bytes = cardTypeId2BytesTupleList.Select(x => x.Item3).LastOrDefault();
                    _cardType = GetCardTypeDict().FirstOrDefault(x => x.Key == _cardTypeId2Bytes).Value;
                    _panCardNumber8Bytes = string.Join("", panCardNumber8BytesTupleList.Select(x => x.Item2));
                    _authCode8Bytes = string.Join("", authCode8BytesTupleList.Select(x => x.Item2));
                    _sequenceNumber10Bytes = string.Join("", sequenceNumber10BytesTupleList.Select(x => x.Item2));
                }

                _commandCodeResponse = string.Join(" ", responseCode6BytesTupleList.Where(x => x.Item1 == 5).Select(x => x.Item4));
                _commandCodeResponseDefinition = GetCommandCodeDefinition(_commandCodeResponse).ToString();
                _errorCodeResponse = string.Join(" ", responseCode6BytesTupleList.Where(x => x.Item1 == 6).Select(x => x.Item4));
                _errorCodeResponseDefinition = GetErrorCodeResponseDefinition(_errorCodeResponse);
                _isTransactionSucceeded = _errorCodeResponse == "FF" ? true : false;

                Console.WriteLine(_errorCodeResponseDefinition);
            }

            return _transactionResponse = TransactionResponseObject();
        }

        private TransactionResponse ParseSerialPortResponse()
        {
            if (_responseTupleList2 != null && _responseTupleList2.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item2));
                _responsByteStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item3));
                _responseHexStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item4));
            }

            if (_responseTupleList3 != null && _responseTupleList3.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item2));
                _responsByteStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item3));
                _responseHexStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item4));
            }

            if (_responseTupleList != null && _responseTupleList.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item2));
                _responsByteStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item3));
                _responseHexStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item4));

                int currentCount = 0;

                /* Command Value is 0x72: 7 bytes, fixed length */
                List<Tuple<int, char, byte, string>> responseCode6BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 6).ToList();
                currentCount = currentCount + 6;

                _commandCodeResponse = string.Join(" ", responseCode6BytesTupleList.Where(x => x.Item1 == 5).Select(x => x.Item4));
                _commandCodeResponseDefinition = GetCommandCodeDefinition(_commandCodeResponse).ToString();
                _errorCodeResponse = string.Join(" ", responseCode6BytesTupleList.Where(x => x.Item1 == 6).Select(x => x.Item4));
                _errorCodeResponseDefinition = GetErrorCodeResponseDefinition(_errorCodeResponse);
                _isTransactionSucceeded = _errorCodeResponse == "FF" ? true : false;

                Console.WriteLine(_errorCodeResponseDefinition);
            }

            return _transactionResponse = TransactionResponseObject();
        }

        private TransactionResponse ParseSerialPortResponse2()
        {
            if (_responseTupleList != null && _responseTupleList.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item2));
                _responsByteStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item3));
                _responseHexStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item4));
            }

            if (_responseTupleList3 != null && _responseTupleList3.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item2));
                _responsByteStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item3));
                _responseHexStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item4));
            }

            if (_responseTupleList2 != null && _responseTupleList2.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item2));
                _responsByteStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item3));
                _responseHexStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item4));

                int currentCount = 0;

                /* Command Value is 0x72: 7 bytes, fixed length */
                List<Tuple<int, char, byte, string>> responseCode6BytesTupleList = _responseTupleList2.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 6).ToList();
                currentCount = currentCount + 6;

                _commandCodeResponse2 = string.Join(" ", responseCode6BytesTupleList.Where(x => x.Item1 == 5).Select(x => x.Item4));
                _commandCodeResponseDefinition2 = GetCommandCodeDefinition(_commandCodeResponse2).ToString();
                _errorCodeResponse2 = string.Join(" ", responseCode6BytesTupleList.Where(x => x.Item1 == 6).Select(x => x.Item4));
                _errorCodeResponseDefinition2 = GetErrorCodeResponseDefinition(_errorCodeResponse2);
                _isTransactionSucceeded2 = _errorCodeResponse2 == "FF" ? true : false;

                Console.WriteLine(_errorCodeResponseDefinition2);
            }

            return _transactionResponse = TransactionResponseObject();
        }

        private TransactionResponse ParseSerialPortResponse3()
        {
            if (_responseTupleList != null && _responseTupleList.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item2));
                _responsByteStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item3));
                _responseHexStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item4));
            }

            if (_responseTupleList2 != null && _responseTupleList2.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item2));
                _responsByteStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item3));
                _responseHexStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item4));
            }

            if (_responseTupleList3 != null && _responseTupleList3.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item2));
                _responsByteStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item3));
                _responseHexStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item4));

                int currentCount = 0;

                /* Command Value is 0x72: 7 bytes, fixed length */
                List<Tuple<int, char, byte, string>> responseCode6BytesTupleList = _responseTupleList3.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 6).ToList();
                currentCount = currentCount + 6;

                _commandCodeResponse3 = string.Join(" ", responseCode6BytesTupleList.Where(x => x.Item1 == 5).Select(x => x.Item4));
                _commandCodeResponseDefinition3 = GetCommandCodeDefinition(_commandCodeResponse3).ToString();
                _errorCodeResponse3 = string.Join(" ", responseCode6BytesTupleList.Where(x => x.Item1 == 6).Select(x => x.Item4));
                _errorCodeResponseDefinition3 = GetErrorCodeResponseDefinition(_errorCodeResponse3);
                _isTransactionSucceeded3 = _errorCodeResponse3 == "FF" ? true : false;

                Console.WriteLine(_errorCodeResponseDefinition3);
            }

            return _transactionResponse = TransactionResponseObject();
        }

        private TransactionResponse ParsePreAuthTransactionResponse()
        {
            if (_responseTupleList2 != null && _responseTupleList2.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item2));
                _responsByteStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item3));
                _responseHexStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item4));
            }

            if (_responseTupleList3 != null && _responseTupleList3.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item2));
                _responsByteStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item3));
                _responseHexStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item4));
            }

            if (_responseTupleList != null && _responseTupleList.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item2));
                _responsByteStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item3));
                _responseHexStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item4));

                int currentCount = 0;

                /* Command Value is 0x72: 7 bytes, fixed length */
                List<Tuple<int, char, byte, string>> responseCode6BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 6).ToList();
                currentCount = currentCount + 6;

                if (_transactionCommandRequest == CommandEnum.PreAuthTransactionRequest.ToString() && _responseTupleList.Count == 59)
                {
                    /* Approved amount: 4 bytes, fixed length */
                    List<Tuple<int, char, byte, string>> approvedAmount4BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 4).ToList();
                    currentCount = currentCount + 4;

                    /* TID: 16 bytes, fixed length. This is host’ sassigned 16 bytes Terminal ID, unique value per terminal, used by host. */
                    List<Tuple<int, char, byte, string>> terminalId16BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 16).ToList();
                    currentCount = currentCount + 16;

                    /* Card ID: 2 bytes, fixed length. This identifies the card type based on the PAN/AID */
                    List<Tuple<int, char, byte, string>> cardTypeId2BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 2).ToList();
                    currentCount = currentCount + 2;

                    /* PAN: 8 bytes, fixed length. This is masked PAN of the card number. First 4 and last 4 digits are included as part of the response */
                    List<Tuple<int, char, byte, string>> panCardNumber8BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 8).ToList();
                    currentCount = currentCount + 8;

                    /* Authorization Code: 8 bytes, fixed length. This is the authorization code returned by host */
                    List<Tuple<int, char, byte, string>> authCode8BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 8).ToList();
                    currentCount = currentCount + 8;

                    /* Sequence Number: 10 bytes, fixed length.This is the sequence number returned by host. */
                    List<Tuple<int, char, byte, string>> sequenceNumber10BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 10).ToList();
                    currentCount = currentCount + 10;

                    /* Record Number: 4 bytes, fixed length. */
                    List<Tuple<int, char, byte, string>> recordNumber4BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 4).ToList();
                    currentCount = currentCount + 4;

                    /* Last 1 byte */
                    List<Tuple<int, char, byte, string>> last1BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 1).ToList();
                    currentCount = currentCount + 1;

                    _approvedSalesAmountDecimal = ConvertHexStrValueToDecimalValue(string.Join("", approvedAmount4BytesTupleList.Select(x => x.Item4)));
                    _terminalId16Bytes = string.Join("", terminalId16BytesTupleList.Select(x => x.Item2));
                    _cardTypeId2Bytes = cardTypeId2BytesTupleList.Select(x => x.Item3).LastOrDefault();
                    _cardType = GetCardTypeDict().FirstOrDefault(x => x.Key == _cardTypeId2Bytes).Value;
                    _panCardNumber8Bytes = string.Join("", panCardNumber8BytesTupleList.Select(x => x.Item2));
                    _authCode8Bytes = string.Join("", authCode8BytesTupleList.Select(x => x.Item2));
                    _sequenceNumber10Bytes = string.Join("", sequenceNumber10BytesTupleList.Select(x => x.Item2));
                    _recordNumber4Bytes = string.Join("", recordNumber4BytesTupleList.Select(x => x.Item2));
                }

                _commandCodeResponse = string.Join(" ", responseCode6BytesTupleList.Where(x => x.Item1 == 5).Select(x => x.Item4));
                _commandCodeResponseDefinition = GetCommandCodeDefinition(_commandCodeResponse).ToString();
                _errorCodeResponse = string.Join(" ", responseCode6BytesTupleList.Where(x => x.Item1 == 6).Select(x => x.Item4));
                _errorCodeResponseDefinition = GetErrorCodeResponseDefinition(_errorCodeResponse);
                _isTransactionSucceeded = _errorCodeResponse == "FF" ? true : false;

                Console.WriteLine(_errorCodeResponseDefinition);
            }

            return _transactionResponse = TransactionResponseObject();
        }

        private TransactionResponse ParseReadCardDataResponse()
        {
            if (_responseTupleList2 != null && _responseTupleList2.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item2));
                _responsByteStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item3));
                _responseHexStrSpaceSeparated2 = string.Join(" ", _responseTupleList2.Select(x => x.Item4));
            }

            if (_responseTupleList3 != null && _responseTupleList3.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item2));
                _responsByteStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item3));
                _responseHexStrSpaceSeparated3 = string.Join(" ", _responseTupleList3.Select(x => x.Item4));
            }

            if (_responseTupleList != null && _responseTupleList.Count > 0)
            {
                _responsCharAsciiWindows1252strSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item2));
                _responsByteStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item3));
                _responseHexStrSpaceSeparated = string.Join(" ", _responseTupleList.Select(x => x.Item4));

                int currentCount = 0;

                /* Command Value is 0x72: 7 bytes, fixed length */
                List<Tuple<int, char, byte, string>> responseCode6BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 6).ToList();
                currentCount = currentCount + 6;

                /* Read Card Status | Encrypted Package | Signed Package */
                if (_transactionCommandRequest == CommandEnum.ReadCardDataRequest.ToString() && _responseTupleList.Count == 76)
                {
                    /* Read Card Status: 1 bytes, fixed length */
                    List<Tuple<int, char, byte, string>> readCardStatus1BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 1).ToList();
                    currentCount = currentCount + 1;

                    /* Encrypted Package - TDES(KEY1,KEY2,Clear text): 48 bytes, fixed length | Use KEY to encrypt "Clear text of card data"; In this way, the Customer App can get 48 bytes of "Encryptedpackage". */
                    List<Tuple<int, char, byte, string>> encryptedPackage48BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 48).ToList();
                    //currentCount = currentCount + 48;

                    /* If Clear card data: Clear text of card data,48 bytes：Card Number | Card Expiry Date | The reserved - TDES(KEY1,KEY2,Clear text): */

                    /* Card Number: 21 bytes, fixed length | KEY1 */
                    List<Tuple<int, char, byte, string>> cardNumber21BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 21).ToList();
                    currentCount = currentCount + 21;

                    /* Expiry Date: 4 bytes, fixed length | KEY2 */
                    List<Tuple<int, char, byte, string>> expiryDate4BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 4).ToList();
                    currentCount = currentCount + 4;

                    /* The Reserved: 23 bytes, fixed length | Clear Text */
                    List<Tuple<int, char, byte, string>> theReserved23BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 23).ToList();
                    currentCount = currentCount + 23;

                    /* Signed Package - SHA1(Clear text): 20 bytes, fixed length | The SHA-1 value of "Clear Text of Card Data". */
                    List<Tuple<int, char, byte, string>> signedPackage20BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 20).ToList();
                    currentCount = currentCount + 20;

                    /* Last 1 byte */
                    List<Tuple<int, char, byte, string>> last1BytesTupleList = _responseTupleList.Where(x => x.Item1 > currentCount && x.Item1 <= currentCount + 1).ToList();
                    currentCount = currentCount + 1;

                    _cardNumber21Bytes = string.Empty;
                    _expiryDate4Bytes = string.Empty;
                    _theReserved23Bytes = string.Empty;
                    _signedPackage20Bytes = string.Empty;
                }

                _commandCodeResponse = string.Join(" ", responseCode6BytesTupleList.Where(x => x.Item1 == 5).Select(x => x.Item4));
                _commandCodeResponseDefinition = GetCommandCodeDefinition(_commandCodeResponse).ToString();
                _errorCodeResponse = string.Join(" ", responseCode6BytesTupleList.Where(x => x.Item1 == 6).Select(x => x.Item4));
                _errorCodeResponseDefinition = GetErrorCodeResponseDefinition(_errorCodeResponse);
                _isTransactionSucceeded = _errorCodeResponse == "FF" ? true : false;

                Console.WriteLine(_errorCodeResponseDefinition);
            }

            return _transactionResponse = TransactionResponseObject();
        }

        private static TransactionResponse TransactionResponseObject()
        {
            TransactionResponse transactionResponse = new TransactionResponse();

            /* Request */
            transactionResponse.CommunicationPackageRequestHexStr = _communicationPackageRequestHexStr;
            transactionResponse.CommunicationPackageRequestHexStr2 = _communicationPackageRequestHexStr2;
            transactionResponse.CommandCodeRequest = _commandCodeRequest;
            transactionResponse.CommandCodeRequest2 = _commandCodeRequest2;
            transactionResponse.CommandCodeRequestDefinition = _commandCodeRequestDefinition;
            transactionResponse.CommandCodeRequestDefinition2 = _commandCodeRequestDefinition2;

            /* Response */
            transactionResponse.AsciiWindows1252stringResponse = _responsCharAsciiWindows1252strSpaceSeparated;
            transactionResponse.AsciiWindows1252stringResponse2 = _responsCharAsciiWindows1252strSpaceSeparated2;
            transactionResponse.AsciiWindows1252stringResponse3 = _responsCharAsciiWindows1252strSpaceSeparated3;
            transactionResponse.HexStringResponse = _responseHexStrSpaceSeparated;
            transactionResponse.HexStringResponse2 = _responseHexStrSpaceSeparated2;
            transactionResponse.HexStringResponse3 = _responseHexStrSpaceSeparated3;
            transactionResponse.CommandCodeResponse = _commandCodeResponse;
            transactionResponse.CommandCodeResponse2 = _commandCodeResponse2;
            transactionResponse.CommandCodeResponse3 = _commandCodeResponse3;
            transactionResponse.CommandCodeResponseDefinition = _commandCodeResponseDefinition;
            transactionResponse.CommandCodeResponseDefinition2 = _commandCodeResponseDefinition2;
            transactionResponse.CommandCodeResponseDefinition3 = _commandCodeResponseDefinition3;
            transactionResponse.ErrorCodeResponse = _errorCodeResponse;
            transactionResponse.ErrorCodeResponse2 = _errorCodeResponse2;
            transactionResponse.ErrorCodeResponse3 = _errorCodeResponse3;
            transactionResponse.ErrorCodeResponseDefinition = _errorCodeResponseDefinition;
            transactionResponse.ErrorCodeResponseDefinition2 = _errorCodeResponseDefinition2;
            transactionResponse.ErrorCodeResponseDefinition3 = _errorCodeResponseDefinition3;
            transactionResponse.TimeStampResponse = _timeStampResponse;
            transactionResponse.TimeStampResponse2 = _timeStampResponse2;
            transactionResponse.TimeStampResponse3 = _timeStampResponse3;
            transactionResponse.IsTransactionSucceeded = _isTransactionSucceeded;
            transactionResponse.IsTransactionSucceeded2 = _isTransactionSucceeded2;
            transactionResponse.IsTransactionSucceeded3 = _isTransactionSucceeded3;

            transactionResponse.ApplicationErrorMessage = _applicationErrorMessage;

            transactionResponse.ApprovedSalesAmount = _approvedSalesAmountDecimal / 100;
            transactionResponse.TID = _terminalId16Bytes.Replace("\0", " ");
            transactionResponse.CardTypeId = _cardTypeId2Bytes;
            transactionResponse.CardType = _cardType;
            transactionResponse.PAN = _panCardNumber8Bytes.Replace("\0", " ");
            transactionResponse.AuthorizationCode = _authCode8Bytes.Replace("\0", " ");
            transactionResponse.SequenceNumber = _sequenceNumber10Bytes.Replace("\0", " ");

            transactionResponse.RecordNumber = _recordNumber4Bytes.Replace("\0", " ");

            transactionResponse.CardNumber = "";
            transactionResponse.ExpiryDate = "";
            transactionResponse.TheReserved = "";
            transactionResponse.SignedPackag = "";

            transactionResponse.CardHoldersName = "";
            transactionResponse.CardExpiration = "";
            transactionResponse.PayerIdentifier = "";

            transactionResponse.JsonStringResponse = string.Empty;
            transactionResponse.JsonStringResponse = JsonSerializer.Serialize(transactionResponse);
            return transactionResponse;
        }

    }
}

