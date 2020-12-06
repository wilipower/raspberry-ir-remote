using System;
using System.Threading;
using System.Device.I2c;
using Iot.Device.CharacterLcd;


namespace RPi_LED_Display
{
    class Program
    {
        static Lcd1602 lcdScreen;
        static BlynkConnection blynk;
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Press a key to start!");
            Console.ReadKey();
            int I2CAddress = 0x3f;
            IrReceiver irReceiver = new IrReceiver();
            //IrSender irSender = new IrSender();
            irReceiver.IrKeyPress += Ir_IrKeyPress;
            Console.WriteLine("Hello World!");
            I2cConnectionSettings settings = new I2cConnectionSettings(1, I2CAddress);
            try
            {
                Console.WriteLine(string.Format("I2C Address is {0}", I2CAddress.ToString()));
                I2cDevice I2c = I2cDevice.Create(settings);
                lcdScreen = new Lcd1602(I2c, false);
                irReceiver.Start();
                blynk = new BlynkConnection();
                //irSender.SendCommand(IrSender.Keys.Power, IrSender.Remotes.Jumbo, 10);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private static void Ir_IrKeyPress(LircEventArgs e)
        {
            lcdScreen.Clear();
            lcdScreen.SetCursorPosition(0, 0);
            lcdScreen.Write(e.KeyCode);
            lcdScreen.SetCursorPosition(0, 1);
            lcdScreen.Write(e.RemoteName);
        }

    }
}
