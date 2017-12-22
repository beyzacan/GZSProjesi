package com.example.fince.gzsmobil;

import android.bluetooth.BluetoothDevice;
import android.os.Build;
import android.support.annotation.RequiresApi;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import java.util.UUID;

public class MainActivity extends AppCompatActivity implements AdapterView.OnItemClickListener {
    private ListView lvNewDevices;
    private BluetoothController bc;
    BluetoothConnectionService mBluetoothConnection;
    BluetoothDevice mBTDevice;
    TextView adSoyadT,yasT,cinsiyetT,nabizT,sicaklikT,saglik_durumuT;
    private static final UUID MY_UUID_INSECURE=
            UUID.fromString("8ce255c0-200a-11e0-ac64-0800200c9a66");
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        lvNewDevices=(ListView)findViewById(R.id.listView);
        lvNewDevices.setOnItemClickListener(MainActivity.this);
        adSoyadT=(TextView)findViewById(R.id.textView4);
        yasT=(TextView)findViewById(R.id.textView3);
        cinsiyetT=(TextView)findViewById(R.id.textView);
        nabizT=(TextView)findViewById(R.id.textView5);
        sicaklikT=(TextView)findViewById(R.id.textView6);
        saglik_durumuT=(TextView)findViewById(R.id.textView9);
    }

    public void incomingMessage(String message)
    {
       setApple(message);
    }
    public MainActivity()
    {
        bc=new BluetoothController(this,this);

    }
    public void onItemClick(AdapterView<?> adapterView, View view, int i, long l) {
        mBluetoothConnection=bc.cancelDiscovery(i,MainActivity.this,this);


    }
    public void drawView()
    {
        lvNewDevices.setAdapter(bc.getmDeviceListAdapter());
    }
    StringBuilder messages;
    public void setMessages(StringBuilder messages) {
        this.messages = messages;
        Log.i("TAG: sendMessages","Gelen Mesaj : "+messages);
    }
    public void bluetooth(View view)
    {
        bc.enableOrDisableBluetooth();
        if(!bc.isBtIsActive())
        {
            lvNewDevices.setAdapter(null);
        }

    }
    @RequiresApi(api = Build.VERSION_CODES.M)
    public void bilgisayar(View view)
    {
        if(bc.isBtIsActive())
        {
            bc.beDiscovered4Devices();
            bc.discoverDevices();



        }
        else
            Toast.makeText(getBaseContext(),"Bluetooth'u AÇ!",Toast.LENGTH_SHORT).show();
    }
    public void baglan(View view)
    {
        mBTDevice=bc.getmBTDevice();
        if(mBTDevice != null)
        {
            if(mBluetoothConnection.isConnect() == false)
            {
                mBluetoothConnection.startBTConnection(mBTDevice,MY_UUID_INSECURE);
                while(!mBluetoothConnection.isConnect()){Log.d("","bekleniyor");};
                Log.i("","bağlandı");


            }

        }
        else
        {
            Toast.makeText(getBaseContext(),"PC'yi SEÇ!.",Toast.LENGTH_SHORT).show();
        }
    }
    public void setApple(final String message)
    {
        new Thread()
        {
            public void run()
            {
                MainActivity.this.runOnUiThread(new Runnable()
                {
                    public void run()
                    {
                        //ad-soyad-yaş-cinsiyet-nabız-sicaklık-sağlıkdurumu
                        String[] parcalar=message.split("-");
                        String ad=parcalar[0];
                        String soyad=parcalar[1];
                        String yas=parcalar[2];
                        String cinsiyet=parcalar[3];
                        String nabiz=parcalar[4];
                        String sicaklik=parcalar[5];
                        String saglik_durumu=parcalar[6];

                        adSoyadT.setText(ad+" "+soyad);
                        yasT.setText(yas);
                        cinsiyetT.setText(cinsiyet);
                        nabizT.setText(nabiz);
                        sicaklikT.setText(sicaklik);
                        saglik_durumuT.setText(saglik_durumu);


                    }
                });
            }
        }.start();
    }
}
