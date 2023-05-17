
# Modbus TCP

#### It's a cross-platforms library write on C#. Library in charge off all action with device (write and read). 
#### You can read data from device with help any type endians:
#### 1.Big Endians/HighLow like 0123
#### 2.Little Endians/LowHigh like 3210, 
#### 3.Half LowHigh/Cross Endians like 2301
### Example
#### using ModbusTCP.Enums;
#### using ModbusTCP;
#### using System;
#### using System.Threading;

#### internal class Program
#### {
####   private static void Main(string[] args)
####    {
####        Modbus device_1 = new ModbusTCP("192.168.0.1"); //test device
####        Modbus device_2 = new ModbusTCP("192.168.0.2"); //test device
####        ushort register_1 = 204;//test address
####        ushort register_2 = 205; //test address
####        for (;;)
####        {
####            try
####            {
####                Thread.Sleep(200);
####                responseDevice_1 = device_1.ReadHoldingFloat(register_1, 1, Endians.Endians_3210, 4); 
#### //if you know that device exist  next  address register
####                responseDevice_2 = device_2.ReadHoldingFloat(register_2, 1,  Endians.Endians_2301, 5);
####                Console.WriteLine($"{Environment.NewLine}Device_1{Environment.NewLine}");
####                foreach (var item in responseDevice_1)
####                {
####                    Console.WriteLine($"Value: {item} ");
####                }
####                Console.WriteLine($"{Environment.NewLine}Device_2{Environment.NewLine}");
####                foreach (var item in responseDevice_2)
####                {
####                    Console.WriteLine($"Value: {item} ");
####                }
####            }
####            catch (Exception ex)
####            {
####                device_1 = new ModbusTCP("192.168.8.35");
####                device_2 = new ModbusTCP("192.168.8.38");
####                Console.WriteLine(ex.Message);
####            }
####        }
####    }
#### }

