using Microsoft.Office.Interop.Excel;
using MySqlX.XDevAPI.Common;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Forms;
using static NPOI.POIFS.Crypt.CryptoFunctions;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace SofarBMS.Model
{
    internal class RealtimeData_CBS5000S_BCU
    {
        public String PackID { get; set; }
        public String CreateDate { get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); } }

        //0x0B6:BCU遥信数据上报1
        public Double Di_Status_Get { get; set; }                //BCU的di状态
        public Double Balance_Chg_Status { get; set; }           //电池包充均衡状态
        public Double Balance_Dchg_Status { get; set; }          //电池包放均衡状态
        public Double Relay_Status { get; set; }                 //继电器状态
        public Double Other_Dev_Staus { get; set; }              //器件的状态

        //0x0B7:BCU遥信数据上报2
        public Double Power_Terminal_Temperature1 { get; set; }//母线正极温度
        public Double Power_Terminal_Temperature2 { get; set; }//负载正极温度
        public Double Power_Terminal_Temperature3 { get; set; }//负载负极温度
        public Double Power_Terminal_Temperature4 { get; set; }//母线负极温度

        //0x0B8:BCU遥信数据上报3
        public Double Insulation_Resistance { get; set; }//高压绝缘阻抗
        public Double Auxiliary_Power_Supply_Voltage { get; set; }//辅助电源电压
        public Double Fuse_Voltage { get; set; }//保险丝后电压
        public Double Power_Voltage { get; set; }//Power_voltage

        //0x0B9:BCU遥信数据上报4
        public Double Load_Voltage { get; set; }//负载侧电压

        //0x0BA:BCU遥信数据上报5
        public UInt16 Bat_Max_Cell_Volt { get; set; }//簇最高单体电压
        public ushort Bat_Max_Cell_VoltPack { get; set; }//簇最高单体电压所在pack
        public ushort Bat_Max_Cell_VoltNum { get; set; }//簇最高单体电压所在pack的第几个位置
        public UInt16 Bat_Min_Cell_Volt { get; set; } //簇最低单体电压
        public ushort Bat_Min_Cell_Volt_Pack { get; set; }//簇最低单体电压所在pack
        public ushort Bat_Min_Cell_Volt_Num { get; set; }//簇最低单体电压所在pack的第几个位置     
 
        //0x0BB:BCU遥信数据上报6
        public ushort Bat_Max_Cell_Temp { get; set; }//簇最高单体温度
        public ushort Bat_Max_Cell_Temp_Pack { get; set; }//簇最高单体温度所在pack
        public ushort Bat_Max_Cell_Temp_Num { get; set; }//簇最高单体温度所在pack的第几个位置
        public ushort Bat_Min_Cell_Temp { get; set; }//簇最低单体温度
        public ushort Bat_Min_Cell_Temp_Pack { get; set; }//簇最低单体温度所在pack
        public ushort Bat_Min_Cell_Temp_Num { get; set; }//簇最低单体温度所在pack的第几个位置

        //0x0BC:BCU遥测数据1
        public Double Battery_Charge_Voltage { get; set; }//电池簇的充电电压
        public Double Charge_Current_Limitation { get; set; }//电池簇充电电流上限
        public Double Discharge_Current_Limitation { get; set; }//电池簇放电电流上限
        public Double Battery_Discharge_Voltage { get; set; }//电池簇的放电截止电压

        //0x0BD:BCU遥测数据2
        public Double Cluster_Voltage { get; set; }//电池簇电压
        public Double Cluster_Current { get; set; }//电池簇电流
        public Double Max_Power_Terminal_Temperature { get; set; }//最高功率端子温度
        public Double Cycles { get; set; }//电池充放电循环次数

        //0x0BE:BCU遥测数据3
        public Double Remaining_Total_Capacity { get; set; }//实时剩余总容量
        public Double Bat_Temp { get; set; }//电池平均温度
        public Double Cluster_Rate_Power { get; set; }//额定功率
        public Double Bat_Bus_Volt { get; set; }//电池母线电压

        //0x0BF:BCU遥测数据4
        public string Bms_State { get; set; }//bcu上送的bms状态
        public ushort Cluster_SOC { get; set; }//电池簇SOC
        public ushort Cluster_SOH { get; set; }//电池簇SOH
        public ushort Pack_Num { get; set; }//簇内电池包数量
        public ushort HW_Version { get; set; }//硬件版本号

        //0x0C0:BCU系统时间
        public String Year { get; set; }//年
        public String Month { get; set; }//月
        public String Day { get; set; }//日
        public String Hour { get; set; }//时
        public String Minute { get; set; }//分
        public String Second { get; set; }//秒

        //0x0C1:模拟量与测试结果1(一般用于ate测试)
        public Double Max_Ring_Charge_Zero_Volt { get; set; }//充电大电流偏压
        public Double Min_Ring_Charge_Zero_Volt { get; set; }//充电小电流偏压
        public Double Max_Ring_Discharge_Zero_Volt { get; set; }//放电大电流偏压
        public Double Min_Ring_Discharge_Zero_Volt { get; set; }//放电小电流偏压

        //0x0C2:模拟量与测试结果2(一般用于ate测试)
        public Double RT1_Tempture { get; set; }//RT1环境温度
        public string Eeprom_Test_Result { get; set; }//eeprom测试结果
        public string Test_Result_485 { get; set; }//485测试结果
        public string CAN1_Test_Result { get; set; }//CAN1测试结果
        public string CAN2_Test_Result { get; set; }//CAN2测试结果
        public string CAN3_Test_Result { get; set; }//CAN3测试结果
        public string Reset_Mode { get; set; }//复位方式
        public Double Dry2_In_Status { get; set; }//干接点2输入
        public string Wake_Source { get; set; }//唤醒原因
       
        public String Fault { get; set; }                           //故障状态
        public String Warning { get; set; }                         //告警状态
        public String Protection { get; set; }                      //保护状态
        public String Fault2 { get; set; }                           //故障状态
        public String Warning2 { get; set; }                         //告警状态
        public String Protection2 { get; set; }                      //保护状态
        public String BCUSaftwareVersion { get; set; }              //BCU软件版本
        public String BCUHardwareVersion { get; set; }              //BCU硬件版本


        public String GetHeader()
        {
            return @"记录时间,电池ID,故障信息1,告警信息1,保护信息1,故障信息2,告警信息2,保护信息2,BCU的di状态,电池包充均衡状态,继电器状态,器件的状态,母线正极温度,负载正极温度,负载负极温度,母线负极温度,高压绝缘阻抗,辅助电源电压,保险丝后电压,Power_voltage,负载侧电压,簇最高单体电压,簇最高单体电压所在pack,簇最高单体电压所在pack的第几个位置,簇最低单体电压,簇最低单体电压所在pack,簇最低单体电压所在pack的第几位置,簇最高单体温度,簇最高单体温度所在pack,簇最高单体温度所在pack的第几个位置,簇最低单体温度,簇最低单体温度所在pack,簇最低单体温度所在pack的第几个位置,电池簇的充电电压,电池簇充电电流上限,电池簇放电电流上限,电池簇的放电截止电压,电池簇电压,电池簇电流,最高功率端子温度,电池充放电循环次数,实时剩余总容量,电池平均温度,额定功率,电池母线电压,bcu上送的bms状态,电池簇SOC,电池簇SOH,簇内电池包数量,硬件版本号,充电大电流偏压,充电小电流偏压,放电大电流偏压,放电小电流偏压,RT1环境温度,eeprom测试结果,485测试结果,CAN1测试结果,CAN2测试结果,CAN3测试结果,复位方式,干接点2输入,唤醒原因,BCU软件版本,BCU硬件版本";
        }
    
        public string GetValue()
        {

            return $"{this.CreateDate},{this.PackID},{this.Fault},{this.Warning},{this.Protection},{this.Fault2},{this.Warning2},{this.Protection2}," +
                   $"{this.Di_Status_Get},{this.Balance_Chg_Status},{this.Balance_Dchg_Status},{this.Relay_Status},{this.Other_Dev_Staus},{this.Power_Terminal_Temperature1}," +
                   $"{this.Power_Terminal_Temperature2},{this.Power_Terminal_Temperature3},{this.Power_Terminal_Temperature4},{this.Insulation_Resistance},{this.Auxiliary_Power_Supply_Voltage},{this.Fuse_Voltage},{this.Power_Voltage}," +
                   $"{this.Load_Voltage},{this.Bat_Max_Cell_Volt},{this.Bat_Max_Cell_VoltPack}, {this.Bat_Max_Cell_VoltNum},{this.Bat_Min_Cell_Volt},{this.Bat_Min_Cell_Volt_Pack},{this.Bat_Min_Cell_Volt_Num}," +
                   $"{this.Bat_Max_Cell_Temp},{this.Bat_Max_Cell_Temp_Pack},{this.Bat_Max_Cell_Temp_Num},{this.Bat_Min_Cell_Temp},{this.Bat_Min_Cell_Temp_Pack}," +
                   $"{this.Bat_Min_Cell_Temp_Num},{this.Battery_Charge_Voltage},{this.Charge_Current_Limitation},{this.Discharge_Current_Limitation},{this.Battery_Discharge_Voltage},{this.Cluster_Voltage}," +
                   $"{this.Cluster_Current},{this.Max_Power_Terminal_Temperature},{this.Cycles},{this.Remaining_Total_Capacity},{this.Bat_Temp},{this.Cluster_Rate_Power},{this.Bat_Bus_Volt}," +
                   $"{this.Bms_State},{this.Cluster_SOC},{this.Cluster_SOH},{this.Pack_Num},{this.HW_Version},{this.Max_Ring_Charge_Zero_Volt},{this.Min_Ring_Charge_Zero_Volt}," +
                   $"{this.Max_Ring_Discharge_Zero_Volt},{this.Min_Ring_Discharge_Zero_Volt},{this.RT1_Tempture},{this.Eeprom_Test_Result},{this.Test_Result_485},{this.CAN1_Test_Result}," +
                   $"{this.CAN2_Test_Result},{this.CAN3_Test_Result},{this.Reset_Mode},{this.Dry2_In_Status},{this.Wake_Source},{this.BCUSaftwareVersion},{this.BCUHardwareVersion}";

        }
    }
}