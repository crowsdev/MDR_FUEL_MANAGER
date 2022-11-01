using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        #region DeviceLists
        List<IMyAirVent> _airVentList;
        List<IMyAirVent> airVentList
        {
            get
            {
                if (_airVentList != null) return _airVentList;
                _airVentList = new List<IMyAirVent>();
                GridTerminalSystem.GetBlocksOfType<IMyAirVent>(_airVentList, null);
                return _airVentList;
            }
        }

        List<IMyGasTank> _gasTankList;
        List<IMyGasTank> gasTankList
        {
            get
            {
                if (_gasTankList != null) return _gasTankList;
                _gasTankList = new List<IMyGasTank>();
                GridTerminalSystem.GetBlocksOfType<IMyGasTank>(_gasTankList, null);
                return _gasTankList;
            }
        }

        List<IMyGasGenerator> _gasGeneratorList;
        List<IMyGasGenerator> gasGeneratorList
        {
            get
            {
                if (_gasGeneratorList != null) return _gasGeneratorList;
                _gasGeneratorList = new List<IMyGasGenerator>();
                GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(_gasGeneratorList, null);
                return _gasGeneratorList;
            }
        }
        #endregion

        #region Settings
        float LowOxygenAirVent = 0.75f;
        float FullOxygenAirVent = 0.9f;
        double LowGasTanks = (double) 0.45;
        double FullGasTanks = (double) 0.9;
        #endregion

        #region States
        bool HasVentsOrTanks => (airVentList.Count > 0 && gasTankList.Count > 0);
        bool LowStateDetected => HasVentsOrTanks ? (CheckAnyAirVentLow(airVentList) || CheckAnyGasTankLow(gasTankList)) : false;
        bool GeneratorsRunning = false;
        bool AllVentsAndTanksFull => HasVentsOrTanks ? (CheckAllAirVentsFull(airVentList) && CheckAllGasTanksFull(gasTankList)) : true;
        #endregion

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (!(GeneratorsRunning || !LowStateDetected))
            {
                foreach (var gg in gasGeneratorList)
                {
                    gg.Enabled = true;
                }
                GeneratorsRunning = true;
                return;
            }


            if (!(!GeneratorsRunning || !AllVentsAndTanksFull))
            {
                foreach (var gg in gasGeneratorList)
                {
                    gg.Enabled = false;
                }
                GeneratorsRunning = false;
            }
        }

        bool CheckAnyAirVentLow(List<IMyAirVent> _myAirVents)
        {
            bool LowVentDetected = false;

            if (_myAirVents.Count == 0) return LowVentDetected;

            foreach (var av in _myAirVents)
            {
                if (av.GetOxygenLevel() <= LowOxygenAirVent)
                {
                    string toLog = av.GetOxygenLevel().ToString();
                    Echo("NEW-OBJECT-DETECTED-------------------------");
                    Echo("Detected Low AirVent OxygenLevel::" + toLog);
                    toLog = "CustomName::" + av.CustomName;
                    Echo(toLog);
                    toLog = "EntityId::" + av.CubeGrid.EntityId.ToString();
                    Echo(toLog);
                    toLog = av.DetailedInfo;
                    Echo("DetailedInfo::" + toLog);
                    Echo("--------------------------------------------");
                    LowVentDetected = true;
                }
            }

            return LowVentDetected;
        }

        bool CheckAnyGasTankLow(List<IMyGasTank> _myGasTanks)
        {
            bool LowTanksDetected = false;

            if (_myGasTanks.Count == 0) return LowTanksDetected;

            foreach (var gt in _myGasTanks)
            {
                if (gt.FilledRatio <= LowGasTanks)
                {
                    string toLog = gt.FilledRatio.ToString();
                    Echo("NEW-OBJECT-DETECTED-------------------------");
                    Echo("Detected Low GasTank FilledRatio::" + toLog);
                    toLog = "CustomName::" + gt.CustomName;
                    Echo(toLog);
                    toLog = "EntityId::" + gt.CubeGrid.EntityId.ToString();
                    Echo(toLog);
                    toLog = gt.DetailedInfo;
                    Echo("DetailedInfo::" + toLog);
                    Echo("--------------------------------------------");
                    LowTanksDetected = true;
                }
            }

            return LowTanksDetected;
        }

        bool CheckAllAirVentsFull(List<IMyAirVent> _myAirVents)
        {
            bool bAllVentsFull = true;

            if (_myAirVents.Count == 0) return bAllVentsFull;

            foreach (var av in _myAirVents)
            {
                if (av.GetOxygenLevel() < FullOxygenAirVent)
                {
                    bAllVentsFull = false;
                    return bAllVentsFull;
                }
            }

            return bAllVentsFull;
        }

        bool CheckAllGasTanksFull(List<IMyGasTank> _myGasTanks)
        {
            bool bAllTanksFull = true;

            if (_myGasTanks.Count == 0) return bAllTanksFull;

            foreach (var gt in _myGasTanks)
            {
                if (gt.FilledRatio < FullGasTanks)
                {
                    bAllTanksFull = false;
                    return bAllTanksFull;
                }
            }

            return bAllTanksFull;
        }
    }
}
