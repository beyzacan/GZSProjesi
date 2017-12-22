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
    public class BluetoothClientController
    {
        private Login lg;
        private Boolean ready { get; set; }
    
        Guid mUUID = new Guid("8ce255c0-200a-11e0-ac64-0800200c9a66");
        public BluetoothClientController(Login lg)
        {
            this.lg= lg;
        }
        public void sendMessageToClient(String message ,BluetoothClient bc)
        {
           
            Byte[] byteMessage = Encoding.UTF8.GetBytes(message);
            ready = true;
            Stream stream = bc.GetStream();
            stream.ReadTimeout = 1000;
            stream.Write(byteMessage, 0, byteMessage.Length);
           
           

        }
        public void sendMessageToAllClients(String message,List<BluetoothClient> list,BluetoothClient gonderen)
        {
            foreach(BluetoothClient bc in list)
            {
                if (bc != gonderen)
                    sendMessageToClient(message, bc);
            }
        }
       

    }
}
