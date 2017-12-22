package com.example.fince.gzsmobil;

import android.Manifest;
import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Build;
import android.support.annotation.RequiresApi;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

import java.util.ArrayList;

/**
 * Created by fince on 24.11.2017.
 */

public class BluetoothController {


    private BluetoothAdapter mBluetoothAdapter;
    public BluetoothAdapter getmBluetoothAdapter() {
        return mBluetoothAdapter;
    }
    private Activity a;
    private boolean btIsActive=false;
    private ArrayList<BluetoothDevice> mBTDevices=new ArrayList<>();
    private DeviceListAdapter mDeviceListAdapter;
    private MainActivity ma;

    //4 bluetooth server
    private BluetoothDevice mBTDevice;
    private BluetoothConnectionService mBluetoothConnection;
    private StringBuilder tmp;

    //register receivers
    private boolean reg1,reg2,reg3,reg4;




    BluetoothController(Activity a,MainActivity ma)
    {
        mBluetoothAdapter=BluetoothAdapter.getDefaultAdapter();
        this.a=a;
        this.ma=ma;
        tmp=new StringBuilder();

    }
    // Create a BroadcastReceiver for ACTION_FOUND.
    private final BroadcastReceiver mBroadcastReceiver1 = new BroadcastReceiver() {
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            if (action.equals(mBluetoothAdapter.ACTION_STATE_CHANGED)) {
                final int state=intent.getIntExtra(BluetoothAdapter.EXTRA_STATE,mBluetoothAdapter.ERROR);
                switch(state){
                    case BluetoothAdapter.STATE_OFF:
                        Log.d("BluetoothController","onReceive : STATE_OFF");

                        break;
                    case BluetoothAdapter.STATE_TURNING_OFF:
                        Log.d("mBroadCastReceiver1","onReceive : STATE_TURNING_OFF");
                        btIsActive=false;
                        break;
                    case BluetoothAdapter.STATE_ON:
                        Log.d("mBroadCastReceiver1","onReceive : STATE_ON");

                        break;
                    case BluetoothAdapter.STATE_TURNING_ON:
                        Log.d("mBroadCastReceiver1","onReceive : STATE_ON");
                        btIsActive=true;
                        break;

                }

            }
        }
    };
    public void enableOrDisableBluetooth()
    {
        if(mBluetoothAdapter == null)
        {
            Log.d("BluetoothController","enableOrDisableBluetooth fonksiyonunda sıkıntı çıktı.");
        }
        if(!mBluetoothAdapter.isEnabled())
        {
            Log.d("BluetoothController","bluetooth aktif edildi");
            btIsActive=true;
            Intent enableBTTIntent=new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
            a.startActivity(enableBTTIntent);
            IntentFilter BTTIntent=new IntentFilter(BluetoothAdapter.ACTION_STATE_CHANGED);
            a.registerReceiver(mBroadcastReceiver1,BTTIntent);
            reg1=true;

        }
        if(mBluetoothAdapter.isEnabled())
        {
            Log.d("BluetoothController","bluetooth pasif edildi");
            btIsActive=false;
            mBluetoothAdapter.disable();
            IntentFilter BTTIntent=new IntentFilter(BluetoothAdapter.ACTION_STATE_CHANGED);
            a.registerReceiver(mBroadcastReceiver1,BTTIntent);
            reg1=true;
        }
    }
    public void unRegReceiver()
    {
        if(reg1)
            a.unregisterReceiver(mBroadcastReceiver1);
        if(reg2)
            a.unregisterReceiver(mBroadcastReceiver2);
        if(reg3)
            a.unregisterReceiver(mBroadcastReceiver3);
        if(reg4)
            a.unregisterReceiver(mBroadcastReceiver4);

    }
    private final BroadcastReceiver mBroadcastReceiver2 = new BroadcastReceiver() {
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            if (action.equals(mBluetoothAdapter.ACTION_SCAN_MODE_CHANGED)) {
                final int state=intent.getIntExtra(BluetoothAdapter.EXTRA_SCAN_MODE,mBluetoothAdapter.ERROR);
                switch(state){
                    case BluetoothAdapter.SCAN_MODE_CONNECTABLE_DISCOVERABLE:
                        Log.d("mBroadCastReceiver2","Discoverability Enabled.");
                        break;
                    case BluetoothAdapter.SCAN_MODE_CONNECTABLE:
                        Log.d("mBroadCastReceiver1","Discoverability Enabled.Able to receive connections.");
                        break;
                    case BluetoothAdapter.SCAN_MODE_NONE:
                        Log.d("mBroadCastReceiver1","oDiscoverability Disabled.Not Able to receive connections.");
                        break;
                    case BluetoothAdapter.STATE_CONNECTING:
                        Log.d("mBroadCastReceiver1","Connecting...");
                        break;
                    case BluetoothAdapter.STATE_CONNECTED:
                    Log.d("mBroadCastReceiver1","Connected.");
                    break;

                }

            }
        }
    };
    public void beDiscovered4Devices()
    {
        Intent discoverableIntent=new Intent(BluetoothAdapter.ACTION_REQUEST_DISCOVERABLE);
        discoverableIntent.putExtra(BluetoothAdapter.EXTRA_DISCOVERABLE_DURATION,300);
        a.startActivity(discoverableIntent);
        IntentFilter intentFilter=new IntentFilter(mBluetoothAdapter.ACTION_SCAN_MODE_CHANGED);
        a.registerReceiver(mBroadcastReceiver2,intentFilter);
        reg2=true;

    }
    public boolean isBtIsActive() {
        return btIsActive;
    }
    public DeviceListAdapter getmDeviceListAdapter() {
        return mDeviceListAdapter;
    }
    private final BroadcastReceiver mBroadcastReceiver3 = new BroadcastReceiver() {
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            if (action.equals(BluetoothDevice.ACTION_FOUND)) {
                BluetoothDevice device=intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
                if(mBTDevices.indexOf(device)<0)
                    mBTDevices.add(device);
                Log.d(" ACTION_FOUND ","onReceive ->"+device.getName()+" : "+device.getAddress());
                mDeviceListAdapter=new DeviceListAdapter(a.getBaseContext(),R.layout.device_adapter_view,mBTDevices);
                ma.drawView();




            }
        }
    };
    @RequiresApi(api = Build.VERSION_CODES.M)
    public void discoverDevices()
    {
        mBTDevices.clear();
        if(mBluetoothAdapter.isDiscovering())
        {
            mBluetoothAdapter.cancelDiscovery();
            Log.d("discoverDEvices","Canceling discovery");
            checkBTPermissions();
            mBluetoothAdapter.startDiscovery();
            IntentFilter discoverDevicesIntent=new IntentFilter(BluetoothDevice.ACTION_FOUND);
            a.registerReceiver(mBroadcastReceiver3,discoverDevicesIntent);
            reg3=true;


        }
        if(!mBluetoothAdapter.isDiscovering()){
            checkBTPermissions();
            mBluetoothAdapter.startDiscovery();
            IntentFilter discoverDevicesIntent=new IntentFilter(BluetoothDevice.ACTION_FOUND);
            a.registerReceiver(mBroadcastReceiver3,discoverDevicesIntent);
            reg3=true;

        }
    }

    @RequiresApi(api = Build.VERSION_CODES.M)
    public void checkBTPermissions()
    {
        if(Build.VERSION.SDK_INT > Build.VERSION_CODES.LOLLIPOP){
            int permissionCheck=a.checkSelfPermission("Manifest.permission.ACCES_FINE_LOCATION");
            permissionCheck+=a.checkSelfPermission("Manifest.permission.ACCESS_COARSE_LOCATION");
            if(permissionCheck != 0){
                a.requestPermissions(new String[]{Manifest.permission.ACCESS_FINE_LOCATION,Manifest.permission.ACCESS_COARSE_LOCATION},1001);
            }
            else{
                Log.d("checkBTPermissions","No beed to check permissions.");
            }
        }
    }
    private final BroadcastReceiver mBroadcastReceiver4 = new BroadcastReceiver() {
        public void onReceive(Context context, Intent intent) {
           final String action=intent.getAction();
           if(action.equals(BluetoothDevice.ACTION_BOND_STATE_CHANGED)){
               BluetoothDevice mDevice=intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
               //zaten cihaz bağlı ise
               if(mDevice.getBondState() == BluetoothDevice.BOND_BONDED){
                    Log.d("BroadcastReceiver4","BOND_BONDED");
                    mBTDevice=mDevice;

               }
               //bağlanma aşamasında ise
               if(mDevice.getBondState() == BluetoothDevice.BOND_BONDING)
               {
                   Log.d("BroadcastReceiver4","BOND_BONDING");
               }
               //herhangi bir bağlanma yok ise
               if(mDevice.getBondState()==BluetoothDevice.BOND_NONE){
                   Log.d("BroadcastReceiver4","BOND_NONE");


               }

           }
        }
    };
    public void prepare4pairingDevices()
    {
        IntentFilter discoverDevicesIntent=new IntentFilter(BluetoothDevice.ACTION_FOUND);
        a.registerReceiver(mBroadcastReceiver4,discoverDevicesIntent);
        reg4=true;
    }
    public BluetoothConnectionService cancelDiscovery(int i,Context cont,MainActivity ma)
    {
        mBluetoothAdapter.cancelDiscovery();
        Log.d("cancelDiscovery()","onItemClick: You clicked on a device");
        String deviceName=mBTDevices.get(i).getName();
        String deviceAddress=mBTDevices.get(i).getAddress();
        Log.d("cancelDiscovery()","onItemClick: deviceName: "+deviceName);
        Log.d("cancelDiscovery()","onItemClick: deviceAddress: "+deviceAddress);
        if(Build.VERSION.SDK_INT > Build.VERSION_CODES.JELLY_BEAN_MR2){
            Log.d("cancelDiscovery()","Trying to pair with "+deviceName);
            mBTDevices.get(i).createBond();
            mBTDevice=mBTDevices.get(i);
            mBluetoothConnection=new BluetoothConnectionService(cont,ma);



        }
        return mBluetoothConnection;


    }
    public BluetoothDevice getmBTDevice() {
        return mBTDevice;
    }
    BroadcastReceiver mReceiver5=new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String text=intent.getStringExtra("theMessage");
            tmp.append(text);
            ma.setMessages(tmp);
            //GELEN MESAJ

        }
    };
    public void mReceiver(Activity act)
    {
        LocalBroadcastManager.getInstance(act).registerReceiver(mReceiver5,new IntentFilter("incomingMessage"));

    }







}
