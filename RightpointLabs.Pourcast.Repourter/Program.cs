﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Toolbox.NETMF.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace RightpointLabs.Pourcast.Repourter
{
    public class Program
    {
        // count flow every half a second
        private const int MILLISECONDS_BETWEEN_COUNT = 500;

        private static readonly TimeSpan ResetWatchdogEvery = new TimeSpan(0, 0, 45);

        private const int NUMBER_OF_TAPS = 2;

        /// <summary>
        /// Main method runs on startup
        /// </summary>
        public static void Main()
        {
            //var sender = new WifiMessageSender("Rightpoint", "Cha", WiFlyGSX.AuthMode.MixedWPA1_WPA2);
            var sender = new EthernetMessageSender();
            //var sender = new NullMessageSender();
            sender.Initalize();
            var writer = new HttpMessageWriter(sender);

            var sensors = new FlowSensor[NUMBER_OF_TAPS];
            // Flow sensor plugged into pin 13, no resistor necessary, fire on the rising edge of the pulse
            sensors[0] = new FlowSensor(new InterruptPort(Pins.GPIO_PIN_D13, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeHigh), writer, "535c61a951aa0405287989ec");
            // Flow sensor plugged into pin 12, no resistor necessary, fire on the rising edge of the pulse
            sensors[1] = new FlowSensor(new InterruptPort(Pins.GPIO_PIN_D12, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeHigh), writer, "537d28db51aa04289027cde5");
            Debug.Print("Starting");

            DateTime lastWatchdogTrigger = DateTime.MinValue;
            while(true)
            {
                if (DateTime.UtcNow.Subtract(lastWatchdogTrigger) >= ResetWatchdogEvery)
                {
                    writer.SendHeartbeatAsync();
                    lastWatchdogTrigger = DateTime.UtcNow;
                }
                foreach (var flowSensor in sensors)
                {
                    flowSensor.CheckPulses();
                }
                Thread.Sleep(MILLISECONDS_BETWEEN_COUNT);
            }

            writer.Stop();
            Debug.Print("Exiting");
        }
    }
}
