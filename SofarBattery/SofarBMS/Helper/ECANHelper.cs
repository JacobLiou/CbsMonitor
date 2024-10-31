using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Helper
{
    [Flags]
    public enum ECANStatus : uint
    {
        /// <summary>
        ///  error
        /// </summary>
        STATUS_ERR = 0x00000,
        /// <summary>
        /// No error
        /// </summary>
        STATUS_OK = 0x00001,
    }

    /// <summary>
    /// 包含 ECAN 系列接口卡的设备信息
    /// </summary>
    public struct BOARD_INFO
    {
        /* 注解
        hw_Version 
        硬件版本号，用16进制表示。比如0x0100表示V1.00。 
        fw_Version 
        固件版本号，用16进制表示。 
        dr_Version 
        驱动程序版本号，用16进制表示。 
        in_Version 
        接口库版本号，用16进制表示。 
        irq_Num 
        板卡所使用的中断号。 
        can_Num 
        表示有几路CAN通道。 
        str_Serial_Num 
        此板卡的序列号。 
        str_hw_Type 
        硬件类型，比如“USBCAN V1.00”（注意：包括字符串结束符’\0’）。 
        Reserved 
        系统保留。*/
        public ushort hw_Version;
        public ushort fw_Version;
        public ushort dr_Version;
        public ushort in_Version;
        public ushort irq_Num;
        public byte can_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] str_Serial_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] str_hw_Type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] Reserved;
    }

    /// <summary>
    /// 在Transmit和Receive函数中被用来传送CAN信息帧
    /// </summary>
    public struct CAN_OBJ
    {
        /* 注解
         ID 
        报文ID。 
        TimeStamp 
        接收到信息帧时的时间标识，从CAN控制器初始化开始计时。 
        TimeFlag 
        是否使用时间标识，为1时TimeStamp有效，TimeFlag和TimeStamp只在此帧为接
        收帧时有意义。 
        SendType 
        发送帧类型，=0时为正常发送，=1时为单次发送，=2时为自发自收，=3时为
        单次自发自收，只在此帧为发送帧时有意义。 
        RemoteFlag 
        是否是远程帧。 
        ExternFlag 
        是否是扩展帧。 
        DataLen 
        数据长度(<=8)，即Data的长度。 
        Data 
        报文的数据。 
        Reserved
        系统保留。*/
        public uint ID;
        public uint TimeStamp;
        public byte TimeFlag;
        public byte SendType;
        public byte RemoteFlag;
        public byte ExternFlag;
        public byte DataLen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Reserved;
    }

    /// <summary>
    /// 包含CAN控制器状态信息。结构体将在ReadCanStatus函数中被填充
    /// </summary>
    public struct CAN_STATUS
    {
        /* 注解
         ErrInterrupt 
        中断记录，读操作会清除。 
        regMode 
        CAN控制器模式寄存器。 
        regStatus 
        CAN控制器状态寄存器。 
        regALCapture 
        CAN控制器仲裁丢失寄存器。 
        regECCapture 
        CAN控制器错误寄存器。 
        regEWLimit 
        CAN控制器错误警告限制寄存器。 
        regRECounter 
        CAN控制器接收错误寄存器。 
        regTECounter 
        CAN控制器发送错误寄存器。*/
        public byte ErrInterrupt;
        public byte regMode;
        public byte regStatus;
        public byte regALCapture;
        public byte regECCapture;
        public byte regEWLimit;
        public byte regRECounter;
        public byte regTECounter;
    }

    /// <summary>
    /// 用于装载VCI 库运行时产生的错误信息。结构体将在ReadErrInfo函数中被填充
    /// </summary>
    public struct ERR_INFO
    {
        /* 注解
         ErrCode 
        错误码。 
        Passive_ErrData 
        当产生的错误中有消极错误时表示为消极错误的错误标识数据。 
        ArLost_ErrData 
        当产生的错误中有仲裁丢失错误时表示为仲裁丢失错误的错误标识数据。*/
        public uint ErrCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Passive_ErrData;
        public byte ArLost_ErrData;
    }

    public struct CAN_ERR_INFO
    {
        public uint ErrCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Passive_ErrData;
        public byte ArLost_ErrData;

    }

    /// <summary>
    /// 定义了初始化CAN的配置。结构体将在InitCan函数中被填充
    /// </summary>
    public struct INIT_CONFIG
    {
        /* 注解
         AccCode
        验收码。SJA1000的帧过滤验收码。
        AccMask
        屏蔽码。SJA1000的帧过滤屏蔽码。屏蔽码推荐设置为0xFFFF FFFF，即
        全部接收。
        Reserved
        保留。
        Filter
        滤波使能。0=不使能，1=使能。使能时，请参照SJA1000验收滤波器设置
        验收码和屏蔽码。
        Timing0
        波特率定时器0（BTR0）。设置值见下表。
        Timing1
        波特率定时器1（BTR1）。设置值见下表。
        Mode
        模式。=0为正常模式，=1为只听模式，=2为自发自收模式。*/
        public uint AccCode;
        public uint AccMask;
        public uint Reserved;
        public byte Filter;
        public byte Timing0;
        public byte Timing1;
        public byte Mode;
    }

    public static class ECANHelper
    {
        [DllImport("ECANVCI.dll", EntryPoint = "OpenDevice")]
        public static extern ECANStatus OpenDevice(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 Reserved);

        [DllImport("ECANVCI.dll", EntryPoint = "CloseDevice")]
        public static extern ECANStatus CloseDevice(
            UInt32 DeviceType,
            UInt32 DeviceInd);


        [DllImport("ECANVCI.dll", EntryPoint = "InitCAN")]
        public static extern ECANStatus InitCAN(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd,
            ref INIT_CONFIG InitConfig);


        [DllImport("ECANVCI.dll", EntryPoint = "StartCAN")]
        public static extern ECANStatus StartCAN(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd);


        [DllImport("ECANVCI.dll", EntryPoint = "ResetCAN")]
        public static extern ECANStatus ResetCAN(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd);


        [DllImport("ECANVCI.dll", EntryPoint = "Transmit")]
        public static extern ECANStatus Transmit(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd,
            CAN_OBJ[] Send,
            UInt16 length);


        [DllImport("ECANVCI.dll", EntryPoint = "Receive")]
        public static extern ECANStatus Receive(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd,
            out CAN_OBJ Receive,
            UInt32 length,
            UInt32 WaitTime);

        [DllImport("ECANVCI.dll", EntryPoint = "ReadErrInfo")]
        public static extern ECANStatus ReadErrInfo(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd,
            out CAN_ERR_INFO ReadErrInfo);



        [DllImport("ECANVCI.dll", EntryPoint = "ReadBoardInfo")]
        public static extern ECANStatus ReadBoardInfo(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            out BOARD_INFO ReadErrInfo);
    }
}