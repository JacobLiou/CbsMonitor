using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Model
{
    public class RealtimeData_BTS5K
    {
        public String PackID { get; set; }
        public String CreateDate { get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"); } }
        //状态信息区
        public Double BatteryVolt { get; set; }                     //电池电压
        public Double LoadVolt { get; set; }                        //负载电压
        public Double BatteryCurrent { get; set; }                  //电池电流
        public Double SOC { get; set; }                             //电池剩余容量
        public Double SOH { get; set; }                             //电池健康程度
        public String RemainingCapacity { get; set; }               //剩余容量
        public String FullCapacity { get; set; }                    //满充容量
        public String BatteryStatus { get; set; }                   //电池状态
        public Double ChargeCurrentLimitation { get; set; }         //充电电流上限
        public Double DischargeCurrentLimitation { get; set; }      //放电电流上限
        public String CumulativeDischargeCapacity { get; set; }     //累计放电量
        public Double TotalChgCap { get; set; }                     //累计充电容量
        public Double TotalDsgCap { get; set; }                     //累计放电容量
        public String CycleTIme { get; set; }                       //循环次数
        //PCU模块数据
        public String WorkState { get; set; }                       //工作状态
        public String BatteryState { get; set; }                    //电池状态
        public String CaseTemperature { get; set; }                 //机箱温度
        public String RadiatorTemperature { get; set; }             //散热器温度
        public String HighvoltageSideVoltage { get; set; }          //高压侧电压
        public String LowvoltageSideVoltage { get; set; }           //低压侧电压
        public String LowvoltageSideCurrent { get; set; }           //低压侧电流
        public String LowvoltageSidePower { get; set; }             //低压侧功率
        public String HighvoltageChargingCurrent { get; set; }      //高压充电电流
        public String HighvoltageDischargeCurrent { get; set; }     //高压放电电流
        public String PcuEvents { get; set; }                       //事件列表
        //BMS模块数据
        public UInt16 BatMaxCellVolt { get; set; }                  //最高单体电压
        public ushort BatMaxCellVoltNum { get; set; }               //最高单体电压编号
        public UInt16 BatMinCellVolt { get; set; }                  //最低单体电压
        public ushort BatMinCellVoltNum { get; set; }               //最低单体电压编号
        public UInt16 BatDiffCellVolt { get; set; }                 //单体电压压差
        public UInt32 CellVoltage1 { get; set; }                    //电压1
        public UInt32 CellVoltage2 { get; set; }                    //电压2
        public UInt32 CellVoltage3 { get; set; }                    //电压3
        public UInt32 CellVoltage4 { get; set; }                    //电压4
        public UInt32 CellVoltage5 { get; set; }                    //电压5
        public UInt32 CellVoltage6 { get; set; }                    //电压6
        public UInt32 CellVoltage7 { get; set; }                    //电压7
        public UInt32 CellVoltage8 { get; set; }                    //电压8
        public UInt32 CellVoltage9 { get; set; }                    //电压9
        public UInt32 CellVoltage10 { get; set; }                   //电压10
        public UInt32 CellVoltage11 { get; set; }                   //电压11
        public UInt32 CellVoltage12 { get; set; }                   //电压12
        public UInt32 CellVoltage13 { get; set; }                   //电压13
        public UInt32 CellVoltage14 { get; set; }                   //电压14
        public UInt32 CellVoltage15 { get; set; }                   //电压15
        public UInt32 CellVoltage16 { get; set; }                   //电压16
        public Double EnvTemperature { get; set; }                  //环境温度
        public Double MosTemperature { get; set; }                  //Mos温度
        public Double BatMaxCellTemp { get; set; }                  //最高单体温度
        public ushort BatMaxCellTempNum { get; set; }               //最高单体温度编号
        public Double BatMinCellTemp { get; set; }                  //最低单体温度
        public ushort BatMinCellTempNum { get; set; }               //最低单体温度编号
        public Double CellTemperature1 { get; set; }                //温度1
        public Double CellTemperature2 { get; set; }                //温度2
        public Double CellTemperature3 { get; set; }                //温度3
        public Double CellTemperature4 { get; set; }                //温度4
        public Double CellTemperature5 { get; set; }                //温度5
        public Double CellTemperature6 { get; set; }                //温度6
        public Double CellTemperature7 { get; set; }                //温度7
        public Double CellTemperature8 { get; set; }                //温度8
        public String Fault { get; set; }                           //故障状态
        public String Warning { get; set; }                         //告警状态
        public String Protection { get; set; }                      //保护状态
        public String Fault2 { get; set; }                           //故障状态
        public String Warning2 { get; set; }                         //告警状态
        public String Protection2 { get; set; }                      //保护状态
        public ushort ChargeMosEnable { get; set; }                 //充电MOS 0断开，1闭合
        public ushort DischargeMosEnable { get; set; }              //放电MOS
        public ushort PrechgMosEnable { get; set; }                 //预充MOS
        public ushort StopChgEnable { get; set; }                   //充电急停
        public ushort HeatEnable { get; set; }                      //加热MOS
        public Double BalanceTemperature1 { get; set; }             //均衡温度1
        public Double BalanceTemperature2 { get; set; }             //均衡温度2
        public String HfilmState { get; set; }                      //加入状态
        public String HfimForbiddenCmd { get; set; }                //加热膜状态

        public String GetHeader()
        {
            return "记录时间,电池ID,故障信息,告警信息,保护信息,电池状态,充电MOS,放电MOS,预充MOS,充电急停,加热MOS,电池电压,负载电压,电池电流,电池剩余容量(SOC),电池健康程度(SOH),剩余容量,满充容量,充电电流上限,放电电流上限,累计放电量,累计充电容量,累计放电容量,最高单体电压编号,最高单体电压,最低单体电压编号,最低单体电压,单体电压差,电压1,电压2,电压3,电压4,电压5,电压6,电压7,电压8,电压9,电压10,电压11,电压12,电压13,电压14,电压15,电压16,环境温度,Mos温度,均衡温度1,均衡温度2,最高单体温度编号,最高单体温度,最低单体温度编号,最低单体温度,温度1,温度2,温度3,温度4,温度5,温度6,温度7,温度8,工作状态,电池状态 ,机箱温度,散热器温度,高压侧电压,低压侧电压,低压侧电流,低压侧功率,高压充电电流,高压放电电流,PCU事件,加热状态,加热膜状态";
        }

        public string GetValue()
        {
            return $"{this.CreateDate},{this.PackID},{this.Fault},{this.Warning},{this.Protection},{this.BatteryStatus},{this.ChargeMosEnable} , {this.DischargeMosEnable}, {this.PrechgMosEnable}, {this.StopChgEnable}, {this.HeatEnable},{this.BatteryVolt},{this.LoadVolt},{this.BatteryCurrent},{this.SOC}, {this.SOH},{this.RemainingCapacity},{this.FullCapacity},{this.ChargeCurrentLimitation},{this.DischargeCurrentLimitation},{this.CumulativeDischargeCapacity},{this.TotalChgCap},{this.TotalDsgCap},{this.BatMaxCellVoltNum},{this.BatMaxCellVolt},{this.BatMinCellVoltNum},{this.BatMinCellVolt},{this.BatDiffCellVolt},{this.CellVoltage1},{this.CellVoltage2},{this.CellVoltage3},{this.CellVoltage4},{this.CellVoltage5},{this.CellVoltage6},{this.CellVoltage7},{this.CellVoltage8},{this.CellVoltage9},{this.CellVoltage10},{this.CellVoltage11},{this.CellVoltage12},{this.CellVoltage13},{this.CellVoltage14},{this.CellVoltage15},{this.CellVoltage16},{this.EnvTemperature},{this.MosTemperature},{this.BalanceTemperature1},{this.BalanceTemperature2},{this.BatMaxCellTempNum},{this.BatMaxCellTemp},{this.BatMinCellTempNum},{this.BatMinCellTemp},{this.CellTemperature1}, {this.CellTemperature2}, {this.CellTemperature3}, {this.CellTemperature4},{this.CellTemperature5}, {this.CellTemperature6}, {this.CellTemperature7}, {this.CellTemperature8},{this.WorkState},{this.BatteryState},{this.CaseTemperature},{this.RadiatorTemperature},{this.HighvoltageSideVoltage},{this.LowvoltageSideVoltage},{this.LowvoltageSideCurrent},{this.LowvoltageSidePower},{this.HighvoltageChargingCurrent},{this.HighvoltageDischargeCurrent},{this.PcuEvents},{this.HfilmState},{this.HfimForbiddenCmd}";
        }
    }
}