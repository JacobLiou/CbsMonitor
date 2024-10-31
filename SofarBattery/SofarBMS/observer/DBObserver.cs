using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SofarBMS.Model;
using SofarBMS.Helper;

namespace SofarBMS.Observer
{
    public class DBObserver : RealtimeDataObserver
    {
        PackageMysqlHelper mysqlHelper = new PackageMysqlHelper();

        public override void SaveData(RealtimeData_BTS5K data)
        {
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            try
            {
                /*string sql = $@"update realtimedata set CreateDate='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}',BatteryStatus='{data.BatteryStatus}',ChargeCurrentLimitation={data.ChargeCurrentLimitation},DischargeCurrentLimitation={data.DischargeCurrentLimitation},ChargeMosEnable={data.ChargeMosEnable},DischargeMosEnable={data.DischargeMosEnable},PrechgMosEnable={data.PrechgMosEnable},StopChgEnable={data.StopChgEnable},HeatEnable={data.HeatEnable},BatteryVolt={data.BatteryVolt},LoadVolt={data.LoadVolt},BatteryCurrent={data.BatteryCurrent},SOC={data.SOC}
                                ,BatMaxCellVolt={data.BatMaxCellVolt},BatMaxCellVoltNum={data.BatMaxCellVoltNum},BatMinCellVolt={data.BatMinCellVolt},BatMinCellVoltNum={data.BatMinCellVoltNum},BatDiffCellVolt={data.BatDiffCellVolt},BatMaxCellTemp={data.BatMaxCellTemp}
                                ,BatMaxCellTempNum={data.BatMaxCellTempNum},BatMinCellTemp={data.BatMinCellTemp},BatMinCellTempNum={data.BatMinCellTempNum},TotalChgCap={data.TotalChgCap},TotalDsgCap={data.TotalDsgCap}
                                ,CellVoltage1={data.Cell_voltage1},CellVoltage2={data.Cell_voltage2},CellVoltage3={data.Cell_voltage3},CellVoltage4={data.Cell_voltage4},CellVoltage5={data.Cell_voltage5},CellVoltage6={data.Cell_voltage6},CellVoltage7={data.Cell_voltage7},CellVoltage8={data.Cell_voltage8},CellVoltage9={data.Cell_voltage9},CellVoltage10={data.Cell_voltage10},CellVoltage11={data.Cell_voltage11},CellVoltage12={data.Cell_voltage12},CellVoltage13={data.Cell_voltage13},CellVoltage14={data.Cell_voltage14},CellVoltage15={data.Cell_voltage15},CellVoltage16={data.Cell_voltage16}
                                ,CellTemperature1={data.CellTemperature1},CellTemperature2={data.CellTemperature2},CellTemperature3={data.CellTemperature3},CellTemperature4={data.CellTemperature4},MOSTemperature={data.MosTemperature},EnvTemperature={data.EnvTemperature},SOH={data.SOH}
                                ,Fault='{data.Fault}',Warning='{data.Warning}',Protection='{data.Protection}',RemainingCapacity='{data.RemainingCapacity}',FullCapacity='{data.FullCapacity}'";*/
                mysqlHelper.ExecuteNonQuery("");
            }
            catch (Exception ex)
            {
            }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }
    }
}
