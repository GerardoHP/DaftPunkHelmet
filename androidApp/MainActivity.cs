using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;
using Java.Util;
using Android.Bluetooth;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace androidApp
{
    [Activity(Label = "androidApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        Switch switchButton;
        //Creamos las variables necesarios para trabajar
        //Widgets
        ToggleButton tgConnect;
        TextView Result;
        //String a enviar
        private Java.Lang.String dataToSend;
        //Variables para el manejo del bluetooth Adaptador y Socket
        private BluetoothAdapter mBluetoothAdapter = null;
        private BluetoothSocket btSocket = null;
        //Streams de lectura I/O
        private Stream outStream = null;
        private Stream inStream = null;
        //MAC Address del dispositivo Bluetooth
        private static string address = "20:15:10:08:16:25";
        //Id Unico de comunicacion
        private static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            switchButton = FindViewById<Switch>(Resource.Id.switchButton);
            Result = FindViewById<TextView>(Resource.Id.textView1);

            // var color = FindViewById<ColorPicker.ColorPanelView>(Resource.Id.color_panel_new);
            switchButton.Click += SwitchButton_Click;

            Connect();

            var firstcolorPickerIntentButton = FindViewById<Button>(Resource.Id.firstcolorPickerIntentButton);
            firstcolorPickerIntentButton.Click += delegate
            {
                var intent = new Intent(this, typeof(ColorPickerActivity));
                StartActivityForResult(intent, Resource.Id.firstColor);
            };

            var secondcolorPickerIntentButton = FindViewById<Button>(Resource.Id.secondcolorPickerIntentButton);
            secondcolorPickerIntentButton.Click += delegate
            {
                var intent = new Intent(this, typeof(ColorPickerActivity));
                StartActivityForResult(intent, Resource.Id.secondColor);
            };

            var thirdcolorPickerIntentButton = FindViewById<Button>(Resource.Id.thirdcolorPickerIntentButton);
            thirdcolorPickerIntentButton.Click += delegate
            {
                var intent = new Intent(this, typeof(ColorPickerActivity));
                StartActivityForResult(intent, Resource.Id.thirdColor);
            };

            var fourthcolorPickerIntentButton = FindViewById<Button>(Resource.Id.fourthcolorPickerIntentButton);
            fourthcolorPickerIntentButton.Click += delegate
            {
                var intent = new Intent(this, typeof(ColorPickerActivity));
                StartActivityForResult(intent, Resource.Id.fourthColor);
            };

            var fifthcolorPickerIntentButton = FindViewById<Button>(Resource.Id.fifthcolorPickerIntentButton);
            fifthcolorPickerIntentButton.Click += delegate
            {
                var intent = new Intent(this, typeof(ColorPickerActivity));
                StartActivityForResult(intent, Resource.Id.fifthColor);
            };

            var sixthcolorPickerIntentButton = FindViewById<Button>(Resource.Id.sixthcolorPickerIntentButton);
            sixthcolorPickerIntentButton.Click += delegate
            {
                var intent = new Intent(this, typeof(ColorPickerActivity));
                StartActivityForResult(intent, Resource.Id.sixthColor);
            };

            var sendbutton = FindViewById<Button>(Resource.Id.sendButton);
            sendbutton.Click += delegate
            {
                var stringList = new List<string>();
                stringList.Add(FindViewById<TextView>(Resource.Id.firstColor).Text);
                stringList.Add(FindViewById<TextView>(Resource.Id.secondColor).Text);
                stringList.Add(FindViewById<TextView>(Resource.Id.thirdColor).Text);
                stringList.Add(FindViewById<TextView>(Resource.Id.fourthColor).Text);
                stringList.Add(FindViewById<TextView>(Resource.Id.fifthColor).Text);
                stringList.Add(FindViewById<TextView>(Resource.Id.sixthColor).Text);

                var jsonString = JsonConvert.SerializeObject(new { colors = stringList });
                writeData(new Java.Lang.String(jsonString));
            };
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Android.App.Result.Ok)
            {
                var colorLabel = FindViewById<TextView>(requestCode);
                var selectedColorStr = data.GetStringExtra("colorSelected");
                colorLabel.Text = selectedColorStr;
                colorLabel.SetBackgroundColor(Android.Graphics.Color.ParseColor(selectedColorStr));
            }
        }

        private void SwitchButton_Click(object sender, EventArgs e)
        {
            var on = new Java.Lang.String("1");
            var off = new Java.Lang.String("0");
            var message = switchButton.Checked ? on : off;
            writeData(message);
        }

        //Evento de conexion al Bluetooth
        public void Connect()
        {
            //asignamos el sensor bluetooth con el que vamos a trabajar
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            //Verificamos que este habilitado
            if (!mBluetoothAdapter.Enable())
            {
                Toast.MakeText(this, "Bluetooth Desactivado",
                    ToastLength.Short).Show();
            }
            //verificamos que no sea nulo el sensor
            if (mBluetoothAdapter == null)
            {
                Toast.MakeText(this,
                    "Bluetooth No Existe o esta Ocupado", ToastLength.Short)
                    .Show();
            }

            //Iniciamos la conexion con el arduino
            BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(address);
            System.Console.WriteLine("Conexion en curso" + device);

            //Indicamos al adaptador que ya no sea visible
            mBluetoothAdapter.CancelDiscovery();
            try
            {
                //Inicamos el socket de comunicacion con el arduino
                btSocket = device.CreateRfcommSocketToServiceRecord(MY_UUID);
                //Conectamos el socket
                btSocket.Connect();
                System.Console.WriteLine("Conexion Correcta");
            }
            catch (System.Exception e)
            {
                //en caso de generarnos error cerramos el socket
                Console.WriteLine(e.Message);
                try
                {
                    btSocket.Close();
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("Imposible Conectar");
                }
                System.Console.WriteLine("Socket Creado");
            }

            //Una vez conectados al bluetooth mandamos llamar el metodo que generara el hilo
            //que recibira los datos del arduino
            beginListenForData();
            //NOTA envio la letra e ya que el sketch esta configurado para funcionar cuando
            //recibe esta letra.

            //dataToSend = new Java.Lang.String("Hola");
            //writeData(dataToSend);
        }

        //Evento para inicializar el hilo que escuchara las peticiones del bluetooth
        public void beginListenForData()
        {
            //Extraemos el stream de entrada
            try
            {
                inStream = btSocket.InputStream;
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            //Creamos un hilo que estara corriendo en background el cual verificara si hay algun dato
            //por parte del arduino
            Task.Factory.StartNew(() =>
            {
                //declaramos el buffer donde guardaremos la lectura
                byte[] buffer = new byte[1024];
                //declaramos el numero de bytes recibidos
                int bytes;
                while (true)
                {
                    try
                    {
                        //leemos el buffer de entrada y asignamos la cantidad de bytes entrantes
                        bytes = inStream.Read(buffer, 0, buffer.Length);
                        //Verificamos que los bytes contengan informacion
                        if (bytes > 0)
                        {
                            //Corremos en la interfaz principal
                            RunOnUiThread(() =>
                            {
                                //Convertimos el valor de la informacion llegada a string
                                string valor = System.Text.Encoding.ASCII.GetString(buffer);
                                //Agregamos a nuestro label la informacion llegada
                                Result.Text = Result.Text + "\n" + valor;
                            });
                        }
                    }
                    catch (Java.IO.IOException)
                    {
                        //En caso de error limpiamos nuestra label y cortamos el hilo de comunicacion
                        RunOnUiThread(() =>
                        {
                            Result.Text = string.Empty;
                        });
                        break;
                    }
                }
            });
        }

        //Metodo de envio de datos la bluetooth
        private void writeData(Java.Lang.String data)
        {
            //Extraemos el stream de salida
            try
            {
                outStream = btSocket.OutputStream;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error al enviar" + e.Message);
            }

            //creamos el string que enviaremos
            Java.Lang.String message = new Java.Lang.String(data.Concat("\n"));

            //lo convertimos en bytes
            byte[] msgBuffer = message.GetBytes();

            try
            {
                //Escribimos en el buffer el arreglo que acabamos de generar
                outStream.Write(msgBuffer, 0, msgBuffer.Length);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error al enviar" + e.Message);
            }
        }

    }
}