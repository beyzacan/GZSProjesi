
//sıcaklık sensoru onayarları
#include <OneWire.h>
#include <LiquidCrystal.h> 

// Sıcaklık sensörünü bağladığımız dijital pini 2 olarak belirliyoruz.
int DS18S20_Pin = 52; 

// Sıcaklık Sensörü Giriş-Çıkışı
OneWire ds(DS18S20_Pin);  // 2. Dijital pinde.

//nabız sensoru ayarları
int PulseSensorPurplePin = 8;        // Pulse Sensor PURPLE WIRE connected to ANALOG PIN 0
int LED13 = 53;   //  The on-board Arduion LED


int Signal;                // holds the incoming raw data. Signal value can range from 0-1024
int Threshold = 550;            // Determine which Signal to "count as a beat", and which to ingore.

String gelen;
int isSicaklik=0;
int isNabiz=0;

int sayac=0;
int nabiz=0;
float sicaklik;
//lcd ayarları
LiquidCrystal lcd(8, 9, 4, 5, 6, 7); 
void setup() {
   
   Serial.begin(9600);         // Set's up Serial Communication at certain speed.
   pinMode(LED13,OUTPUT);  

   //lcd 
   lcd.begin(16, 2); /* Kullandigimiz LCDnin sutun ve satir sayisini belirtmeliyiz */
   lcd.setCursor(0, 0);
   lcd.print("MERHABA"); /* Ekrana yazi yazalim */
   delay(1000);


}

void loop() {

   gelen=Serial.readString();
  
  
  
  if(gelen[0]== '1')
  {
    isSicaklik=1;  
  }
  else if (gelen[0] == '2')
  {
    isNabiz=1;
   
  }
  else
  {
    //istek yok
    Serial.println("i-dinleniyor");
  }
  while(isSicaklik==1)
     {

       gelen[0]=Serial.read();
       if(gelen[0] == '1')
       {
          isSicaklik=0;
          break;
       }
         
      
         //sıcaklık sensoru olayları
     // temperature değişkenini sıcaklık değerini alma fonksiyonuna bağlıyoruz.
      sicaklik = getTemp();
      // Sensörden gelen sıcaklık değerini Serial monitörde yazdırıyoruz.
      String strSicaklik=String(sicaklik);
      String message="S-"+strSicaklik;
      Serial.println(message);
      lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print("SICAKLIK:");
      lcd.setCursor(0,1);
      lcd.print(sicaklik);
      
  
      delay(10);
  
     }
   while(isNabiz == 1)
   {
      gelen[0]=Serial.read();
      if(gelen[0] == '2')
      {
        isNabiz=0;
        nabiz=0;
        break;
      }
      // nabız sensoru olayları
      Signal = analogRead(PulseSensorPurplePin);  // Read the PulseSensor's value.
      if(Signal > Threshold)
      {                                          // If the signal is above "550", then "turn-on" Arduino's on-Board LED.
          digitalWrite(LED13,HIGH);
          sayac=0;
            
         
      } 
      else 
      {
          digitalWrite(LED13,LOW); 
          sayac+=1;
          if(sayac == 10)
          {
            lcd.clear();
            lcd.setCursor(0, 0);
            nabiz+=1;
            lcd.print("NABIZ:");
            lcd.setCursor(0, 1);
            lcd.print(nabiz);
            Serial.println("N-geldi");
          }
           
          /*String strNabiz=String(Signal);
          String message="N-"+strNabiz;
          Serial.println(message);*/
          //  Else, the sigal must be below "550", so "turn-off" this LED.
      }                                       // Assign this value to the "Signal" variable.
      
       delay(10);
   }
     lcd.clear();
     lcd.setCursor(0, 0);
     lcd.print("ISLEM");
     lcd.setCursor(0, 1);
     lcd.print("BEKLENIYOR");
   
  

 
  

  
  

  

  

}

float getTemp(){
  //returns the temperature from one DS18S20 in DEG Celsius

  byte data[12];
  byte addr[8];

  if ( !ds.search(addr)) {
      //no more sensors on chain, reset search
      ds.reset_search();
      return -1000;
  }

  if ( OneWire::crc8( addr, 7) != addr[7]) {
      Serial.println("CRC is not valid!");
      return -1000;
  }

  if ( addr[0] != 0x10 && addr[0] != 0x28) {
      Serial.print("Device is not recognized");
      return -1000;
  }

  ds.reset();
  ds.select(addr);
  ds.write(0x44,1); // start conversion, with parasite power on at the end

  byte present = ds.reset();
  ds.select(addr);    
  ds.write(0xBE); // Read Scratchpad

  for (int i = 0; i < 9; i++) { // we need 9 bytes
    data[i] = ds.read();
  }

  ds.reset_search();

  byte MSB = data[1];
  byte LSB = data[0];

  float tempRead = ((MSB << 8) | LSB); //using two's compliment
  float TemperatureSum = tempRead / 16;

  return TemperatureSum;

}
