/*
 * Created by SharpDevelop.
 * User: Benny
 * Date: 2012-10-8
 * Time: 10:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers; 
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace GOSTS.Common
{
    /// <summary>
    /// Description of MessageChannel.
    /// </summary>
    public class MessageChannel : IDisposable
    {
        #region Delegates and Events
        public delegate void OnError(object sender, Exception ex);
        public event OnError Error;

        public delegate void OnConnecting(object sender);
        public event OnConnecting Connecting;

        public delegate void OnConnected(object sender);
        public event OnConnected Connected;

        public delegate void OnDisconnected(object sender);
        public event OnDisconnected Disconnected;

        public delegate void OnSendMessageSuccess(object sender, string str);
        public event OnSendMessageSuccess SendMessageSuccess;

        //public delegate void OnReceiveMessageSuccess(object sender, string str);
        //public event OnReceiveMessageSuccess ReceiveMessageSuccess;

        //kenlo 130103
        public delegate void OnReceiveMessageSuccessUsingStream(object sender, MemoryStream stream);
        public event OnReceiveMessageSuccessUsingStream ReceiveMessageSuccessUsingStream;

        public delegate void OnSendHeartBeat(object sender, string type);
        public event OnSendHeartBeat SendHeartBeat;

        #endregion

        #region Instance Variables
        private bool connected;
        private bool firstConnect;
        private bool disposing;
        private bool closing;
        private TcpClient msgClient;
        // private NetworkStream msgStream;
        private SslStream msgSslStream;
        private byte[] buffer = new byte[11];
        private StringBuilder receiveData = null;
        private int readCount = 0;
        private System.Timers.Timer sendTimerOMT;
        private System.Timers.Timer sendTimerOMP;

        #endregion

        #region Constructors
        public MessageChannel(string ip, int port)
        {
            Host = ip;
            Port = port;
            connected = false;
            firstConnect = true;
            disposing = false;
            closing = false;
            ReconnectDelay = AppFlag.ReconnectDelay;
            ConnectRetries = AppFlag.ConnectRetries;
            receiveData = new StringBuilder();
            // EnsureConnection();
            // this.AsynConnect();
            sendTimerOMT = new System.Timers.Timer(AppFlag.HeartBeatDelay);
            sendTimerOMT.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            sendTimerOMT.Enabled = false;
            sendTimerOMP = new System.Timers.Timer(AppFlag.HeartBeatDelay);
            sendTimerOMP.Elapsed += new ElapsedEventHandler(OnTimedEventOMP);
            sendTimerOMP.Enabled = false;

        }
        #endregion

        #region Properties

        /// <summary>
        /// ReconnectDelay
        /// </summary>
        public int ReconnectDelay
        {
            get;
            set;
        }

        /// <summary>
        /// ConnectRetries
        /// </summary>
        public int ConnectRetries
        {
            get;
            set;
        }
        /// <summary>
        /// Ip
        /// </summary>
        public string Host
        {
            get;
            private set;
        }
        /// <summary>
        /// Port
        /// </summary>
        public int Port
        {
            get;
            private set;
        }

        #endregion

        //Send Heartbeat
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            sendTimerOMT.Enabled = false;
            var OnSendHeartBeat = SendHeartBeat;
            if (OnSendHeartBeat != null)
                OnSendHeartBeat(this, this.Host + ":" + AppFlag.PortOMT);
        }

        private void OnTimedEventOMP(object source, ElapsedEventArgs e)
        {
            sendTimerOMP.Enabled = false;
            var OnSendHeartBeat = SendHeartBeat;
            if (OnSendHeartBeat != null)
                OnSendHeartBeat(this, this.Host + ":" + AppFlag.PortOMP);
        }
        //private TextWriterTraceListener tr1, tr2;
        //private TextWriterTraceListener  tr2;
        private void DebugLog(String remarkStr, String logString)
        {
            ////if (tr1 == null)
            ////{
            ////    tr1 = new TextWriterTraceListener(System.Console.Out);
            ////    Trace.Listeners.Add(tr1);
            ////}

            //if (tr2 == null)
            //{
            //    //tr2 = new TextWriterTraceListener(System.IO.File.CreateText("ComDebug.txt"));
            //    tr2 = new TextWriterTraceListener(System.IO.File.CreateText(AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder + "ChannelDebug-" + this.Host + "-" + this.Port.ToString() + "-" + DateTime.Now.ToString("yyMMdd-HHmmss") + ".txt"));
            //    Trace.Listeners.Add(tr2);
            //}

            //tr2.WriteLine(DateTime.Now.ToString("yyMMdd-HHmmss ") + remarkStr + "  " + logString);
            //tr2.Flush();

            // TradeStationLog.WriteChannelDebug(DateTime.Now.ToString("yyMMdd-HHmmss ") + remarkStr + "  " + logString);
        }

        private void DebugLog(String remarkStr, Stream logStream)
        {
            long orgPos = logStream.Position;

            DebugLog(remarkStr, (new StreamReader(logStream)).ReadToEnd());
            logStream.Position = orgPos;
        }

        #region Public Methods

        /// <summary>
        /// Closes the Connection when logout and sessionhash error.  This will cause EnsureConnection() to always return false after this method is called.
        /// </summary>
        public void Close()
        {
            closing = true;
        }

        public void EnsureConnection()
        {
            // TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " EnsureConnection");
            while (!connected)
            {
                Reconnect();
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " EnsureConnection Done");
            }
        }

        /// <summary>
        /// Synchronization
        /// </summary>
        /// <returns></returns>
        private bool Connect()
        {
            // ConnectRetries
            int connectionAttempts = 0;
            while (connectionAttempts < this.ConnectRetries && (msgClient == null || !msgClient.Connected))
            {
                if (connectionAttempts > 0)
                    Thread.Sleep(this.ReconnectDelay);

                connectionAttempts++;

                try
                {
                    if (this.Connecting != null)
                        this.Connecting(this);

                    TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " " + this.Host + ":" + this.Port + " 2. Connecting.....");

                    msgClient = new TcpClient();
                    msgClient.Connect(this.Host, this.Port);
                    TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " " + this.Host + ":" + this.Port + " 3. Connected");

                }
                catch (SocketException ex)
                {
                    if (this.Error != null)
                        this.Error(this, ex);
                    return false;
                }
            }
            if (connectionAttempts >= 3)
            {
                if (this.Error != null)
                    this.Error(this, new MessageException(3, "Too many connection attempts"));

                return false;
            }

            return true;
        }

        /// <summary>
        /// Force a reconnection,  if an Connection exception is received or Timeout, in which case it closes the existing channel
        /// </summary>
        private void ForceReconnect()
        {
            this.connected = false;
        }

        /// <summary>
        /// Disable ForceReconnect
        /// </summary>
        private void DisForceReconnect()
        {
            this.connected = true;
        }

        public void EnsureDisconnected()
        {
            this.DisForceReconnect();

            //if (msgStream != null)
            if (msgSslStream != null)
                CloseStream();
            if (msgClient != null)
                Disconnect();

            //TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " EnsureDisconnected Done.");
        }

        private void Disconnect()
        {
            try
            {
                msgClient.Close();
            }
            catch (Exception ex)
            {
                if (this.Error != null)
                    this.Error(this, ex);
            }
        }

        public void Send(string msg)
        {
            DebugLog("SEND", msg);

            byte[] msgBytes = ToByte(msg);
            Send(msgBytes);
        }

        /// <summary>
        /// Synchronization Send a message to a connected channel immediately,must call EnsureConnection() before starting to send.
        /// </summary>
        /// <param name="message">The Messages to send.</param>
        public void Send(byte[] message)
        {
            if (!disposing && connected)
            {
                try
                {
                    this.EnsureConnection();

                    if (this.msgClient == null || !this.msgClient.Connected)
                    {
                        throw new MessageException(2, "Send Connection is unavailable!");
                    }
                    else
                    {
                        msgSslStream.Write(message, 0, message.Length);
                        //msgSslStream.Write(message, 0, 11);
                        //msgSslStream.Flush();
                        //msgSslStream.Write(message, 11, message.Length - 11);
                        msgSslStream.Flush();
                        if (msgClient.Client.RemoteEndPoint.ToString().Equals(this.Host + ":" + AppFlag.PortOMT))
                        {
                            sendTimerOMT.Enabled = false;
                            sendTimerOMT.Start();
                        }
                        else
                        {
                            sendTimerOMP.Enabled = false;
                            sendTimerOMP.Start();
                        }

                        if (this.SendMessageSuccess != null)
                            this.SendMessageSuccess(this, Encoding.ASCII.GetString(message, 0, message.Length));
                    }
                }

                catch (ArgumentOutOfRangeException argumentOutOfRangeException)
                {
                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 11 Read::  " + "ArgumentOutOfRangeException.Message");
                    if (this.Error != null)
                        this.Error(this, argumentOutOfRangeException);
                }
                catch (ArgumentNullException argumentNullException)
                {
                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 22 Read::  " + "argumentNullException.Message");
                    if (this.Error != null)
                        this.Error(this, argumentNullException);
                }
                catch (ObjectDisposedException objectDisposedException)
                {
                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 33 Read::  " + "objectDisposedException.Message");
                    if (this.Error != null)
                        this.Error(this, objectDisposedException);
                }
                catch (IOException iOException)
                {
                    SocketException socketException = iOException.InnerException as SocketException;
                    if (socketException != null)
                    {
                        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 44 Read::  " + "socketException.Message");
                        if (this.Error != null)
                        {
                            this.Error(this, iOException.InnerException);
                        }
                        this.DisForceReconnect();
                        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 55. ForceReconnect.");
                    }
                    else
                    {
                        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "66 Read::  " + "iOException.Message");
                        if (this.Error != null)
                        {
                            this.Error(this, iOException);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageException messageException = ex as MessageException;
                    if (messageException != null)
                    {
                        this.DisForceReconnect();
                        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 88. Send::   + ex.Message");
                    }

                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 77. Send::  " + ex.Message);
                    if (this.Error != null)
                    {
                        this.Error(this, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Synchronization Read
        /// </summary>  
        public void Read()
        {
            if (!disposing && connected)
            {
                try
                {
                    this.EnsureConnection();

                    if (this.msgClient == null || !this.msgClient.Connected)
                    {
                        throw new MessageException(1, "Read Connection is unavailable!");
                    }
                    else
                    {
                        MemoryStream responseStream = new MemoryStream();
                        int offset = 0;
                        int recv;
                        const int headerLen = 11;
                        byte[] header = new byte[headerLen];

                        recv = msgSslStream.Read(header, 0, headerLen);
                        responseStream.Write(header, (int)responseStream.Length, headerLen);

                        if (recv == headerLen)
                        {
                            int dataleft = MsgLength(header);
                            byte[] data = new Byte[dataleft];
                            offset = headerLen;
                            //int i = 1;
                            while (dataleft > 0)
                            {
                                recv = msgSslStream.Read(data, 0, dataleft);
                                if (recv == 0)
                                {
                                    break;
                                }
                                responseStream.Write(data, 0, recv);
                                offset += recv;
                                dataleft -= recv;

                                //TradeStationLog.WriteLogRead(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") +(i++).ToString ()+ "  HEAD [" + responseStream.Length + "]------." + System.Text.Encoding.Default.GetString(responseStream.ToArray()) + "\r\n");
                            }

                            responseStream.Position = 0;
                            if (ReceiveMessageSuccessUsingStream != null)
                            {
                                var OnReceiveMessageSuccess = this.ReceiveMessageSuccessUsingStream;
                                //TradeStationLog.WriteLogRead(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "  HEAD ["+ responseStream.Length+ "]------." + System.Text.Encoding.Default.GetString(responseStream.ToArray()) + "\r\n");

                                if (OnReceiveMessageSuccess != null)
                                    OnReceiveMessageSuccess(this, responseStream);
                            }
                        }

                        //MemoryStream responseStream = new MemoryStream();      //kenlo 130103
                        //int len = 0;
                        //const int headerLen = 11;
                        //byte[] data = new Byte[headerLen];
                        //string responseData = ""; 

                        //while (true)
                        //{
                        //    if (responseStream.Length < headerLen)
                        //    {
                        //        //Int32 readLen = msgStream.Read(data, (int)responseStream.Length, (int)(headerLen - responseStream.Length));
                        //        Int32 readLen = msgSslStream.Read(data, (int)responseStream.Length, (int)(headerLen - responseStream.Length));
                        //        responseStream.Write(data, (int)responseStream.Length, readLen);
                        //        if (readLen == 0)
                        //        {
                        //            break;
                        //        }
                        //        responseData += System.Text.Encoding.ASCII.GetString(data, 0, readLen);
                        //        TradeStationLog.WriteLogRead(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 2 Read........." + responseData + "aaaaaaa\r\n");
                        //        if (responseStream.Length == headerLen)
                        //        {
                        //            len = MsgLength(data);
                        //            if (len == 100591)
                        //            {
                        //                string dtr = "";
                        //            }
                        //            data = new Byte[len];

                        //        }

                        //        //  int bytes = -1;
                        //        //  byte[] buffer = new byte[11];
                        //        //  bytes = msgSslStream.Read(buffer, 0, buffer.Length);
                        //        //  responseData += System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);

                        //        // // Int32 readLen = msgStream.Read(data, (int)responseStream.Length, (int)(headerLen - responseStream.Length));
                        //        //  Int32 readLen = msgSslStream.Read(data, (int)responseStream.Length, (int)(headerLen - responseStream.Length));
                        //        //  responseStream.Write(data, (int)responseStream.Length, readLen);
                        //        ////  responseData += System.Text.Encoding.ASCII.GetString(data, 0, readLen);
                        //        //  if (responseStream.Length == headerLen)
                        //        //  {
                        //        //      len = MsgLength(data);
                        //        //      data = new Byte[len];
                        //        //  }
                        //    }
                        //    if (len > 0 && responseStream.Length >= headerLen)
                        //    {
                        //        //Int32 readLen = msgStream.Read(data, (int)(responseStream.Length - headerLen), (int)(len - (responseStream.Length - headerLen)));
                        //        Int32 readLen = msgSslStream.Read(data, (int)(responseStream.Length - headerLen), (int)(len - (responseStream.Length - headerLen)));
                        //        if (readLen == 0)
                        //        {
                        //            break;
                        //        }
                        //        responseStream.Write(data, (int)(responseStream.Length - headerLen), readLen);
                        //        responseData += System.Text.Encoding.ASCII.GetString(data, 0, readLen);
                        //        TradeStationLog.WriteLogRead(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 3 Read........." + responseData + "bbbbbbbb\r\n");
                        //        //int id=responseData.Length;
                        //    }
                        //    if (responseStream.Length - headerLen == len)
                        //    {
                        //        responseStream.Position = 0;
                        //        if (ReceiveMessageSuccessUsingStream != null)
                        //        {
                        //            var OnReceiveMessageSuccess = this.ReceiveMessageSuccessUsingStream;
                        //            TradeStationLog.WriteLogRead(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 4 Read........." + responseData + "cccccccc\r\n");
                        //            if (OnReceiveMessageSuccess != null)
                        //                OnReceiveMessageSuccess(this, responseStream);
                        //        }
                        //        break;
                        //    }
                        //} 
                    }
                }
                catch (ArgumentOutOfRangeException argumentOutOfRangeException)
                {
                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 1 Read::  " + "ArgumentOutOfRangeException.Message");
                    if (this.Error != null)
                        this.Error(this, argumentOutOfRangeException);
                }
                catch (ArgumentNullException argumentNullException)
                {
                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 2 Read::  " + "argumentNullException.Message");
                    if (this.Error != null)
                        this.Error(this, argumentNullException);
                }
                catch (ObjectDisposedException objectDisposedException)
                {
                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 3 Read::  " + "objectDisposedException.Message");
                    if (this.Error != null)
                        this.Error(this, objectDisposedException);
                }
                catch (IOException iOException)
                {
                    SocketException socketException = iOException.InnerException as SocketException;
                    if (socketException != null)
                    {
                        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 4 Read::  " + "socketException.Message");
                        if (this.Error != null)
                        {
                            this.Error(this, iOException.InnerException);
                        }
                        this.DisForceReconnect();
                        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 5. ForceReconnect.");
                    }
                    else
                    {
                        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "6 Read::  " + "iOException.Message");
                        if (this.Error != null)
                        {
                            this.Error(this, iOException);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageException messageException = ex as MessageException;
                    if (messageException != null)
                    {
                        this.DisForceReconnect();
                        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 8. Read::    + ex.Message");
                    }

                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " 7. Read::  " + ex.Message);
                    if (this.Error != null)
                    {
                        this.Error(this, ex);
                    }
                }
            }
        }

        public void Dispose()
        {
            disposing = true;
            connected = false;

            //try { msgStream.Close(); }
            try { msgSslStream.Close(); }
            catch { }

            // try { msgStream.Dispose(); }
            try { msgSslStream.Dispose(); }
            catch { }

            try { msgClient.Client.Shutdown(SocketShutdown.Both); }
            catch { }

            try { msgClient.Client.Close(); }
            catch { }

            try { msgClient.Close(); }
            catch { }
        }

        #endregion

        #region Asyn
        /// <summary>
        /// AsynConnect
        /// </summary>
        /// <returns></returns>
        public bool AsynConnect()
        {
            try
            {
                if (this.Connecting != null)
                    this.Connecting(this);

                msgClient = new TcpClient();
                //msgClient.BeginConnect(this.Host, this.Port, new AsyncCallback(AsynOpenStream), msgClient);

                var result = msgClient.BeginConnect(this.Host, this.Port, new AsyncCallback(AsynOpenStream), msgClient);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(AppFlag.TimeOutDelay/1000));
                if (!success)
                { 
                    throw new MessageException(5, "Failed to connect!");
                }
                msgClient.EndConnect(result);
            }
            catch (SocketException ex)
            {
                if (this.Error != null)
                    this.Error(this, ex);

                return false;
            }
            // return true;
            return this.connected;
        }

        ///<summary>
        ///AsynOpenStream
        ///</summary>
        ///<returns></returns>
        private void AsynOpenStream(IAsyncResult ar)
        {
            try
            {
                msgClient = (TcpClient)ar.AsyncState;
                //msgStream = msgClient.GetStream();  
                msgSslStream = new SslStream(msgClient.GetStream(), true, new RemoteCertificateValidationCallback(validateServerCertificate), null);
                msgSslStream.AuthenticateAsClient(this.Host, null, System.Security.Authentication.SslProtocols.Ssl3, false);

            }
            catch (SocketException e)
            {
                if (this.Error != null)
                    this.Error(this, e);

                return;
            }
            catch (Exception e)
            {
                if (this.Error != null)
                    this.Error(this, e);

                return;
            }
            //if (!msgStream.CanWrite)
            if (!msgSslStream.CanWrite)
            {
                if (this.Error != null)
                    this.Error(this, new MessageException(4, "NetWorkStream is not Writable"));

                return;
            }

            connected = true;

            if (this.Connected != null)
            {
                this.Connected(this);
            }
            else
            {
                string strStatus = "this.Connected != null";
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + strStatus);
            }

            return;
        }

        public void AsynSend(string msg)
        {
            byte[] msgBytes = ToByte(msg);
            AsynSend(msgBytes);
        }

        /// <summary>
        /// Asynchronism Send a message to a connected channel immediately,must call EnsureConnection() before starting to send.
        /// </summary>
        /// <param name="message">The Messages to send.</param>
        public void AsynSend(byte[] message)
        {
            try
            {
                if (this.msgClient == null || !this.msgClient.Connected)
                {
                    throw new Exception("Cannot connect to server!");
                }
                else
                {
                    AsyncCallback callback = new AsyncCallback(SendCallback);
                    //msgStream.BeginWrite(message, 0, message.Length, callback, msgStream);
                    msgSslStream.BeginWrite(message, 0, message.Length, callback, msgSslStream);
                }

                if (SendMessageSuccess != null)
                {
                    var OnSendMessageSuccess = SendMessageSuccess;
                    if (OnSendMessageSuccess != null)
                        OnSendMessageSuccess(this, Encoding.ASCII.GetString(message, 0, message.Length));
                }


            }
            catch (Exception ex)
            {
                if (this.Error != null)
                    this.Error(this, ex);

            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // NetworkStream msgStream = (NetworkStream)ar.AsyncState;
                //msgStream = (NetworkStream)ar.AsyncState;
                //msgStream.EndWrite(ar);
                msgSslStream = (SslStream)ar.AsyncState;
                msgSslStream.EndWrite(ar);
                //if(SendMessageSuccess != null)
                //        {   
                //var OnSendMessageSuccess = SendMessageSuccess;
                //if (OnSendMessageSuccess != null)
                //    OnSendMessageSuccess(this, Encoding.ASCII.GetString(buffer, 0, nBytesRec));
                //   }
            }
            catch (Exception ex)
            {
                //var OnMessageFailed = MessageFailed;
                //if (OnMessageFailed != null)
                //    OnMessageFailed(this, messsage);

                //var onError = Error;
                //if (onError != null)
                //    onError(this, ex);

                if (this.Error != null)
                    this.Error(this, ex);

            }
        }

        /// <summary>
        /// Asynchronism Read
        /// </summary> 
        public void AsynRead()
        {
            //byte[] buffer=new byte [11];  
            //byte lenght = 11;
            //if (buffer[7]!= 0)
            //{
            //    lenght = buffer[7];
            //    buffer = new byte[lenght];
            //}
            //else if (buffer[7] != 0)
            //{
            //    return;
            //}
            byte lenght = 11;
            if (readCount == 0)
            {
                buffer = new byte[11];
            }
            else if (readCount == 1)
            {
                lenght = buffer[7];
                buffer = new byte[lenght];
            }
            else
            {
                buffer = new byte[0];
                lenght = 0;
            }
            try
            {
                if (this.msgClient == null || !this.msgClient.Connected)
                {
                    throw new MessageException(-1, "");
                }
                else
                {
                    AsyncCallback callback = new AsyncCallback(ReceiveDataback);
                    //msgStream.BeginRead(buffer, 0, lenght, callback, msgStream);
                    msgSslStream.BeginRead(buffer, 0, lenght, callback, msgSslStream);
                }
            }
            catch (Exception ex)
            {
                if (this.Error != null)
                    this.Error(this, ex);
            }
        }

        public string ReceivedData
        {
            get
            {
                lock (this)
                {
                    return this.receiveData.ToString();
                }
            }
        }

        private void ReceiveDataback(IAsyncResult ar)
        {
            try
            {
                //msgStream = (NetworkStream)ar.AsyncState;
                //int nBytesRec = msgStream.EndRead(ar);
                msgSslStream = (SslStream)ar.AsyncState;
                int nBytesRec = msgSslStream.EndRead(ar);
                if (nBytesRec > 0)
                {
                    string sRecieved = Encoding.ASCII.GetString(buffer, 0, nBytesRec);
                    receiveData.Append(sRecieved);
                    readCount++;
                    AsynRead();

                    if (receiveData.Length != 0)
                    {
                        if (readCount > 1)
                        {
                            //var OnReceiveMessageSuccess = ReceiveMessageSuccess;  //removed by ben 130104 ,will add
                            //if (OnReceiveMessageSuccess != null)
                            //    OnReceiveMessageSuccess(this, receiveData.ToString());

                            //var MyDelegate = MyEvent;
                            //if (MyDelegate != null)
                            //    MyDelegate(this, null);
                        }
                    }
                }
                else
                {
                    if (receiveData.Length != 0)
                    {
                        //if (ReceiveMessageSuccess != null)
                        //{
                        //    var OnReceiveMessageSuccess = ReceiveMessageSuccess;
                        //    if (OnReceiveMessageSuccess != null)
                        //        OnReceiveMessageSuccess(this, receiveData.ToString());
                        //}
                    }
                    else
                    {
                        if (this.Error != null)
                            this.Error(this, new Exception("Data is null!"));
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Error != null)
                    this.Error(this, ex);
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Synchronization
        /// </summary>
        /// <returns></returns>
        private bool OpenStream()
        {
            try
            {
                // msgStream = msgClient.GetStream();
                //msgSslStream = new SslStream(msgClient.GetStream(), false, new RemoteCertificateValidationCallback(validateServerCertificate), null);
                //msgSslStream.AuthenticateAsClient(this.Host,null, System.Security.Authentication.SslProtocols.Ssl2, false);
                msgSslStream = new SslStream(msgClient.GetStream(), false, new RemoteCertificateValidationCallback(validateServerCertificate), null);
                msgSslStream.AuthenticateAsClient(this.Host, null, System.Security.Authentication.SslProtocols.Ssl3, false);
            }
            catch (SocketException e)
            {
                if (this.Error != null)
                    this.Error(this, e);

                return false;
            }

            //if (!msgStream.CanWrite)
            if (!msgSslStream.CanWrite)
            {
                if (this.Error != null)
                    this.Error(this, new MessageException(4, "NetWorkStream is not Writable"));

                return false;
            }

            //if (this.Connected != null)
            //    this.Connected(this);

            return true;
        }

        private bool validateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            return true; // Dont care about server's cert
        }

        //private bool validateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        //{
        //    if (sslPolicyErrors == SslPolicyErrors.None)
        //        return true;
        //    return true; // Dont care about server's cert
        //}

        private bool Reconnect()
        {
            if (!firstConnect)
            {
                for (int i = 0; i < this.ReconnectDelay; i += 100)
                    System.Threading.Thread.Sleep(100);
            }
            else
            {
                firstConnect = false;
            }

            //if (msgStream != null && msgStream.CanWrite)
            if (msgSslStream != null && msgSslStream.CanWrite)
            {
                try { Disconnect(); }
                catch { }
            }

            if (msgClient != null && msgClient.Client != null && msgClient.Connected)
            {
                try { CloseStream(); }
                catch { }
            }

            if (Connect())
            {
                this.connected = OpenStream();

                if (this.Connected != null)
                    this.Connected(this);

                return this.connected;
            }

            this.connected = false;

            return this.connected;
        }

        private void CloseStream()
        {
            try
            {
                //msgStream.Close();
                //msgStream.Dispose();
                //msgStream = null;
                msgSslStream.Close();
                msgSslStream.Dispose();
                msgSslStream = null;
            }
            catch (Exception ex)
            {
                if (this.Error != null)
                    this.Error(this, ex);
            }

            if (this.Disconnected != null)
                this.Disconnected(this);
        }

        #endregion

        #region TOOLS
        private static byte[] ToByte(string str)
        {
            string str1 = str.Substring(0, str.LastIndexOf("\\x") + 4);
            string str2 = str.Substring(str.LastIndexOf("\\x") + 4, str.Length - str1.Length);
            string s = str1.Replace("\\x", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            Byte[] buffer2 = System.Text.Encoding.ASCII.GetBytes(str2);
            //ssHead + ssContent 
            byte[] buffer3 = new byte[buffer.Length + buffer2.Length];
            buffer.CopyTo(buffer3, 0);
            buffer2.CopyTo(buffer3, buffer.Length);

            return buffer3;
        }

        //private int MsgLength(byte[] headByte)
        //{
        //    int len = 0;
        //    if ((int)headByte[10] > 0)
        //    {
        //        len += (int)headByte[10] * 256 << (3 - 1);
        //    }
        //    if ((int)headByte[9] > 0)
        //    {
        //        len += (int)headByte[9] * 256 << (2 - 1);
        //    }
        //    if ((int)headByte[8] > 0)
        //    {
        //        len += (int)headByte[8] * 256 << (1 - 1); ;
        //    }
        //    if ((int)headByte[7] > 0)
        //    {
        //        len += (int)headByte[7];
        //    }
        //    int n = len;
        //    return len;
        //}
        private int MsgLength(byte[] headByte)
        {
            int len = 0;
            if ((int)headByte[10] > 0)
            {
                len += (int)headByte[10] << 24;
            }
            if ((int)headByte[9] > 0)
            {
                len += (int)headByte[9] << 16;
            }
            if ((int)headByte[8] > 0)
            {
                len += (int)headByte[8] << 8;
            }
            if ((int)headByte[7] > 0)
            {
                len += (int)headByte[7];
            }
            return len;
        }

        #endregion
    }
}
 