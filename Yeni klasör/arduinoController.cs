using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gzsProjesi
{
    class arduinoController
    {
        private Login login;
        private string nabiz, sicaklik;
        public bool isNabizDinle { get; set; }
        public bool isSicaklikDinle { get; set;}
        public List<int> list= new List<int>() ;
        public int sayac { get; set; }

        public arduinoController(Login login) {
            this.login = login;
            isNabizDinle = false;
            sayac = 0;

        }
        public void scan()
        {
            List<String> gecici = new List<string>();
            foreach (string portlar in System.IO.Ports.SerialPort.GetPortNames())
            {
                gecici.Add(portlar);
            }
            login.updateList(gecici);
        }
        public void baglan(String name)
        {
            try
            {
                login.serialPort1.PortName = name;
                login.serialPort1.BaudRate = 9600;
                login.serialPort1.Open();
                Console.WriteLine("Giriş Başarılı");
                Thread seriDinle = new Thread(seriPortDinle);
                seriDinle.IsBackground = true;
                seriDinle.Start();
               

            }
            catch(Exception e)
            {
                Console.WriteLine("Hata: " + e.Message);
            }
        }
        public void seriPortDinle()
        {
            Console.WriteLine("Thread oluştu");
            while(true){
                String message = login.serialPort1.ReadLine();
                String[] gecici = message.Split('-');
               
               
                if (gecici[0].Equals("N"))
                {
                    nabiz = gecici[1];
                    if(isNabizDinle)
                    {
                        sayac+=1;
                    }
                  
                }
                else
                {
                    if (isSicaklikDinle)

                    {
                        
                        if(gecici[1].Contains('.'))
                        {
                            String[] parcalar = gecici[1].Split('.');
                            int sicaklik = Convert.ToInt32(parcalar[0]);
                            
                            list.Add(sicaklik);
                        }
                        else
                        {
                            int sicaklik = Convert.ToInt32(gecici[1]);
                            
                            list.Add(sicaklik);


                        }
                            
                  
                        
                       
                       
                    }
                   
                }

            }
        }
        private void updateUI(String message)
        {
            Func<int> del = delegate ()
            {

                return 0;

            };
            login.Invoke(del);
        }
        public void gonder(String veri)
        {
            login.serialPort1.Write(veri);
        }

    
    }
}
