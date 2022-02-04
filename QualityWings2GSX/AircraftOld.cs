using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using FSUIPC;
using System.Threading;

namespace QualityWings2GSX
{
    public class AircraftOld
    {
        //5570 gal (orig)
        //  -> FS: 21084,74 lt / 16927,61 kg // 37318,991939007898 lbs ===> 6,7 conv (gal -> lbs) // 6,7003
        //  -> QW: 17433 kg // 38433,1861658394 lbs ===> 6,9 wing conv // 6,94005 center conv

        public static double paxScalar = 190; //in lbs ~ 86 kg per Pax
        public static double poundScalar = 2.2046226218; //2,2046226218 // 0,45359237
        public static double bizPax = 0.10526315789473684210526315789474;
        public static double ecoPax = 0.89473684210526315789473684210526;
        public static double tankWingMax = 17433;
        public static double tankCenterMax = 70735;
        public static double tankWingStep = 0.0023339317773788150807899461400359;
        public static double tankCenterStep = 0.0005785491766800178015131286159324;
        public static double fuelScalar = 8388608; //% to Offset Value
        protected static double unitScalar = 1.0;

        public double OFPfuel { get; set; } = 5280;
        public int OFPpax { get; set; } = 10;
        public string OFPunit { get; set; }

        protected double fuelWingTarget;
        protected double fuelCenterTarget;
        protected double fuelWingCurrent;
        protected double fuelCenterCurrent;
        protected Offset<int> fuelLeftOffset = new Offset<int>(Program.groupName, 0x0B7C);
        protected Offset<int> fuelRightOffset = new Offset<int>(Program.groupName, 0x0B94);
        protected Offset<int> fuelCenterOffset = new Offset<int>(Program.groupName, 0x0B74);

        protected Offset<double> paxBizOffset = new Offset<double>(Program.groupName, 0x1400);
        protected Offset<double> paxEcoOffset = new Offset<double>(Program.groupName, 0x1430);

        protected int infoTicksFuel = 0;
        protected int infoTicksPax = 0;

        public AircraftOld()
        {
            
        }

        public void SetPayload(double fuel, int pax, string unit)
        {
            Log.Logger.Information("Setting Payload ...");

            OFPfuel = fuel;
            OFPpax = pax;
            OFPunit = unit;
            if (OFPunit != "kgs")
                unitScalar = poundScalar;

            fuelCenterTarget = OFPfuel - (tankWingMax * unitScalar * 2);
            if (fuelCenterTarget > 0)
                fuelWingTarget = (OFPfuel - fuelCenterTarget) / 2.0;
            else
            {
                fuelWingTarget = OFPfuel / 2.0;
                fuelCenterTarget = 0.0;
            }
            Log.Logger.Information($"Wing Target: {fuelWingTarget:F0}{OFPunit} | Center Target: {fuelCenterTarget:F0}{OFPunit} | Total: {((fuelWingTarget * 2) + fuelCenterTarget):F0}{OFPunit}");

            fuelWingTarget /= (tankWingMax * unitScalar);
            if (fuelWingTarget > 1.0)
                fuelWingTarget = 1.0;
            fuelCenterTarget /= (tankCenterMax *unitScalar);
            if (fuelCenterTarget > 1.0)
                fuelCenterTarget = 1.0;
            Log.Logger.Information($"Wing Target: {(fuelWingTarget * 100):F2}% | Center Target: {(fuelCenterTarget * 100):F2}%");

            fuelWingCurrent = 0;
            fuelCenterCurrent = 0;

            Log.Logger.Information($"Total Passengers: {OFPpax} (Business: {(OFPpax * bizPax):F0} | Economy: {(OFPpax * ecoPax):F0})");
            if (OFPunit != "kgs")
                Log.Logger.Information($"Payload: {(OFPpax * paxScalar):F0}lbs");
            else
                Log.Logger.Information($"Payload: {((OFPpax * paxScalar) / poundScalar):F0}kgs");
        }

        public bool RefuelFinished { get { return fuelWingCurrent == fuelWingTarget && fuelCenterCurrent == fuelCenterTarget; } }

        public void StartRefuel()
        {
            fuelLeftOffset.Reconnect();
            fuelRightOffset.Reconnect();
            fuelCenterOffset.Reconnect();
            FSUIPCConnection.Process(Program.groupName);
            if (fuelCenterTarget == 0.0 && fuelCenterOffset.Value != 0)
                fuelCenterOffset.Value = 0;
            else if (fuelCenterTarget > 0.0)
                fuelCenterCurrent = fuelCenterOffset.Value / fuelScalar;

            fuelWingCurrent = fuelRightOffset.Value / fuelScalar;
            FSUIPCConnection.Process(Program.groupName);
            infoTicksFuel = 0;

            Log.Logger.Information("Fuel Hose connected, refueling ...");
        }

        public bool RefuelAircraft()
        {
            if (fuelRightOffset.Value / fuelScalar < fuelWingTarget - tankWingStep)
            {
                fuelWingCurrent += tankWingStep;
                fuelLeftOffset.Value = (int)(fuelWingCurrent * fuelScalar);
                fuelRightOffset.Value = (int)(fuelWingCurrent * fuelScalar);
            }
            else if (fuelWingCurrent != fuelWingTarget)
            {
                fuelWingCurrent = fuelWingTarget;
                fuelLeftOffset.Value = (int)(fuelWingTarget * fuelScalar);
                fuelRightOffset.Value = (int)(fuelWingTarget * fuelScalar);
                Log.Logger.Information($"Wings finished: {(fuelRightOffset.Value / fuelScalar):F2}%");
            }

            if (fuelCenterTarget > 0.0 && fuelCenterCurrent < fuelCenterTarget - tankCenterStep)
            {
                fuelCenterCurrent += tankCenterStep;
                fuelCenterOffset.Value = (int)(fuelCenterCurrent * fuelScalar);
            }
            else if (fuelCenterTarget > 0.0 && fuelCenterCurrent != fuelCenterTarget)
            {
                fuelCenterCurrent = fuelCenterTarget;
                fuelCenterOffset.Value = (int)(fuelCenterTarget * fuelScalar);
                Log.Logger.Information($"Center finished: {(fuelCenterOffset.Value / fuelScalar):F2}%");
            }

            FSUIPCConnection.Process(Program.groupName);
            infoTicksFuel++;
            if (infoTicksFuel >= 10 && !RefuelFinished)
            {
                Log.Logger.Information($"Wings Loaded: {(fuelWingCurrent * 100):F2}% | Center Loaded: {(fuelCenterCurrent * 100):F2}%");
                infoTicksFuel = 0;
            }

            return RefuelFinished;
        }

        public void StopRefuel()
        {
            double left = (fuelLeftOffset.Value / fuelScalar) * (tankWingMax * unitScalar);
            double right = (fuelRightOffset.Value / fuelScalar) * (tankWingMax * unitScalar);
            double center = (fuelCenterOffset.Value / fuelScalar) * (tankWingMax * unitScalar);
            double sum = left + right + center;

            fuelLeftOffset.Disconnect();
            fuelRightOffset.Disconnect();
            fuelCenterOffset.Disconnect();
            
            Log.Logger.Information($"Refuel finished! FOB: {sum:F0}{OFPunit} (Wings: {(left + right):F0}{OFPunit} | Center: {center:F0}{OFPunit})");
        }

        public void StartBoarding()
        {
            paxBizOffset.Reconnect();
            paxEcoOffset.Reconnect();
            paxBizOffset.Value = 0;
            paxEcoOffset.Value = 0;
            FSUIPCConnection.Process(Program.groupName);
            infoTicksPax = 0;
            Log.Logger.Information($"Started Boarding (PAX: {OFPpax}) ...");
        }

        public bool BoardAircraft()
        {
            int brdPax = (int)FSUIPCConnection.ReadLVar("FSDT_GSX_NUMPASSENGERS_BOARDING_TOTAL");       

            paxBizOffset.Value = brdPax * bizPax * paxScalar;
            paxEcoOffset.Value = brdPax * ecoPax * paxScalar;

            FSUIPCConnection.Process(Program.groupName);

            infoTicksPax++;
            if (infoTicksPax >= 10 && brdPax != OFPpax)
            {
                Log.Logger.Information($"Boarding ... {brdPax}/{OFPpax} (Business: {(brdPax * bizPax):F0} | Economy: {(brdPax * ecoPax):F0})");
                infoTicksPax = 0;
            }

            return brdPax == OFPpax;
        }
    
        public void StopBoarding()
        {
            double payload = paxBizOffset.Value + paxEcoOffset.Value;

            paxBizOffset.Disconnect();
            paxEcoOffset.Disconnect();

            Log.Logger.Information($"Boarding finished! SOB: {(payload / paxScalar):F0} (Payload {(payload / poundScalar):F2}kg)");
        }

        public void StartDeboarding()
        {
            paxBizOffset.Reconnect();
            paxEcoOffset.Reconnect();
            FSUIPCConnection.Process(Program.groupName);
            FSUIPCConnection.WriteLVar("FSDT_GSX_NUMPASSENGERS", OFPpax);
            infoTicksPax = 0;
            Log.Logger.Information($"Started Deboarding (PAX: {OFPpax}) ...");
        }

        public bool DeboardAircraft()
        {
            int debrdPax = OFPpax - (int)FSUIPCConnection.ReadLVar("FSDT_GSX_NUMPASSENGERS_DEBOARDING_TOTAL");

            paxBizOffset.Value = debrdPax * bizPax * paxScalar;
            paxEcoOffset.Value = debrdPax * ecoPax * paxScalar;

            FSUIPCConnection.Process(Program.groupName);

            infoTicksPax++;
            if (infoTicksPax >= 10 && debrdPax != 0)
            {
                Log.Logger.Information($"Deboarding ... {debrdPax}/{OFPpax}");
                infoTicksPax = 0;
            }

            return debrdPax == 0;
        }

        public void StopDeboarding()
        {
            paxBizOffset.Value = 0;
            paxEcoOffset.Value = 0;
            FSUIPCConnection.Process(Program.groupName);

            paxBizOffset.Disconnect();
            paxEcoOffset.Disconnect();

            Log.Logger.Information($"Deboarding finished!");
        }
    }
}
