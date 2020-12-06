using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using BlynkLibrary;

namespace RPi_LED_Display
{
    class BlynkConnection
    {
        int port = 8442;
        string server = "blynk-cloud.com";
        string AUTH = "z6UQHaPf3XdtTfOhgB5-FEj2oK_0b2x7";
        Blynk blynk;
        IrSender irSender;

        public bool IsConnected { get { return blynk.Connected; } }
        //Faire un dictionnaire pour linker les pin number a une fonction ou un object
        public BlynkConnection()
        {
            irSender = new IrSender();
            blynk = new Blynk(AUTH, server, port);
            blynk.DigitalPinReceived += Blynk_DigitalPinReceived;
            blynk.VirtualPinReceived += Blynk_VirtualPinReceived;
            try
            {
                blynk.Connect();
                Console.WriteLine("Connected to blynk server!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Blynk_VirtualPinReceived(Blynk b, VirtualPinEventArgs e)
        {
            VirtualPin vp;
            try
            {
                if(e.Data.Value[0].GetType() == typeof(string))
                {
                    float value = 0;
                    string strVal = (string)e.Data.Value[0];
                    strVal = strVal.Replace('.', ',');
                    if (float.TryParse(strVal, out value))
                        switch (e.Data.Pin)
                        {
                            case 2:
                                vp = e.Data;
                                vp.Value[0] = "0.0";
                                blynk.SendVirtualPin(vp);
                                if (value == 1)
                                {
                                    irSender.SendCommand(IrSender.Keys.VolumeUp, IrSender.Remotes.Jumbo, 1);
                                }
                                else if (value == -1)
                                    irSender.SendCommand(IrSender.Keys.VolumeDown, IrSender.Remotes.Jumbo, 1);
                                Thread.Sleep(250);
                                break;
                            default:
                               break;
                      }
                }
                else if ((int)e.Data.Value[0] == 1)
                {
                    switch ((int)e.Data.Pin)
                    {
                        case 0:
                            irSender.SendCommand(IrSender.Keys.Power, IrSender.Remotes.Jumbo, 10);
                            break;
                        case 1:
                            irSender.SendCommand(IrSender.Keys.Input, IrSender.Remotes.Jumbo, 5);
                            break;
                        case 2:

                            break;
                        default:
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }
        }

        private void Blynk_DigitalPinReceived(Blynk b, DigitalPinEventArgs e)
        {
            
        }
    }
}
