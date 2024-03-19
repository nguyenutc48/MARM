
using MARM.Enums;
using System.IO.Ports;
using System;

namespace MARM.Services
{
    public class ComDataService : IComDataService
    {
        public event Action<byte[]>? DataReceived;

        private SerialPort? serialPort;
        private bool isOpened = false;
        
        private byte[] buffer = new byte[255];


        public void Open(string comPort, int baudrate)
        {
            try
            {
                serialPort = new SerialPort(comPort, baudrate);
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                isOpened = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                isOpened = false;
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            int byteReceivedLength = sp.Read(buffer,0,buffer.Length);
            if(byteReceivedLength > 0)
                OnDataReceived(buffer);

        }

        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(data);
        }

        public void Close()
        {
            try
            {
                serialPort.DataReceived -= SerialPort_DataReceived;
                serialPort.Close();
                isOpened = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public async Task SendData(string data)
        {
            await Task.Delay(100);
            serialPort.WriteLine(data);
        }

        public async Task<byte[]> ReadData()
        {
            await Task.Yield();
            byte[] data = new byte[255];
            if(isOpened)
            {
                int a = serialPort.Read(data, 0, data.Length);  
                if (a > 0)
                {
                    data = new byte[a];
                    return data;
                }
            }
            return Array.Empty<byte>();
        }

        public bool IsConnected()
        {
            return serialPort.IsOpen;
        }

        public async Task SendByte(byte[] data)
        {
            await Task.Yield();
            serialPort.Write(data, 0, data.Length);
        }
    }
}
