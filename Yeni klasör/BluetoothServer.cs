using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;
using System.IO;
using System.Threading;
using gzsProjesi;

namespace GZSProjesi
{
    public class BluetoothServer
    {
        public List<BluetoothClient> connessioniServer { get; set; }
       
        Login lg;

        public BluetoothClient conn { set; get; }
        BluetoothClient sonMesajGonderen { get; set; }



        
       
        
        public BluetoothServer(Login lg)
        {
            this.lg =lg;
            
            connessioniServer = new List<BluetoothClient>();

          
        }
        public void addList(BluetoothClient bc)
        {
            if(connessioniServer.IndexOf(bc) <0)
            {
                
                connessioniServer.Add(bc);
                
            }
           
        }
       
       

        public void ConnectAsServer()
        {
            

            // thread handshake
            Thread bluetoothConnectionControlThread = new Thread(new ThreadStart(ServerControlThread));
            bluetoothConnectionControlThread.IsBackground = true;
            bluetoothConnectionControlThread.Start();

            // thread connessione
            Thread bluetoothServerThread = new Thread(new ThreadStart(ServerConnectThread));
            bluetoothServerThread.IsBackground = true;
            bluetoothServerThread.Start();
        }

        private void ServerControlThread()
        {
            while (true)
            {
                
                updateConnList();
              
                Thread.Sleep(0);
            }
        }

        Guid mUUID = new Guid("8ce255c0-200a-11e0-ac64-0800200c9a66");
        
        private void ServerConnectThread()
        {
            updateUI("server started");
            BluetoothListener blueListener = new BluetoothListener(mUUID);
            blueListener.Start();
            while (true)
            {
                conn = blueListener.AcceptBluetoothClient();
                addList(conn);
                Thread appoggio = new Thread(new ParameterizedThreadStart(ThreadAscoltoClient));
                appoggio.IsBackground = true;
                appoggio.Start(conn);
                updateUI(conn.RemoteMachineName + " has connected");
                

            }
        }
      
        private void ThreadAscoltoClient(object obj)
        {
            BluetoothClient clientServer = (BluetoothClient)obj;
            sonMesajGonderen = clientServer;
            Stream streamServer = clientServer.GetStream();
            streamServer.ReadTimeout = 1000;
            while (clientServer.Connected)
            {
                try
                {
                    int bytesDaLeggere = clientServer.Available;
                    if (bytesDaLeggere > 0)
                    {
                        byte[] bytesLetti = new byte[bytesDaLeggere];
                        int byteLetti = 0;
                        while (bytesDaLeggere > 0)
                        {
                            int bytesDavveroLetti = streamServer.Read(bytesLetti, byteLetti, bytesDaLeggere);
                            bytesDaLeggere -= bytesDavveroLetti;
                            byteLetti += bytesDavveroLetti;
                        }
                        updateUI( System.Text.Encoding.UTF8.GetString(bytesLetti));
                        
                        
                    }
                }
                catch { }
                Thread.Sleep(0);
            }
            updateUI(clientServer.RemoteMachineName + " has gone");
        }



        public void updateUI(string message)
        {
            Func<int> del = delegate ()
            {
                Console.WriteLine(message);
                return 0;
            };
            lg.Invoke(del);
        }
        

        private void updateConnList()
        {
            Func<int> del = delegate ()
            {
                //fm2.setClearList();
                
              
                return 0;
            };
            try
            {
                lg.Invoke(del);
            }
            catch { }
        }
    }
}
