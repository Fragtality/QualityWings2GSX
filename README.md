# QualityWings2GSX: SimBrief and GSX Integration for the QualityWings 787
If this Toolset is of interest to you, can be answered easily by yourself:
- Wished that you could just get the planned Fuel and Weights from your SimBrief OFP automatically by calling the GSX Services? Without any Payload Dialog or Dispatch Page / Dispatcher involved?
- Wished that the Plane was actually & progessively refuelled and (De-)Boarded when GSX is active? (Not refueling and boarding an already loaded Plane - really seeing the GW rise and fall)
- Wished that the Ground- and Cabin-Crew would just take care of the Doors, instead you "leaving" the Cockpit to open/close all that Doors?
- Wished the Jetway/GPU would just automatically be called / removed, the Chocks being set / removed and External Power connected / disconnected?<br/>

So if the Answer is 'yes' to any of these Questions: Here is a tool you might want to try out :wink:<br/>But before getting to excited, here the caveat: you need a registered Version of FSUIPC to use the whole Toolset!<br/><br/>

Since you're still interested, here the Features of the individual Tools/Scripts contained in this Toolset:
- An external Program (*QualityWings2GSX*) which will read your current SimBrief OFP and uses the Fuel Weights, Passenger and Bag Count (and their configured Weight in SimBrief) to progressively Refuel, Board and Deboard the Plane. It will set these Values in / use the Values with GSX so that the Refuel, Board and Deboard Animation is "in synch" with your OFP Data.<br/>It does not matter if your OFP is in kgs or lbs, if your APU is running or not and it is "turn-around-safe": After being deboarded the new OFP will be loaded and used for the next Refuel, Board and Deboard Cycle. It uses FSUIPC (C# Client) to communicate with the Sim/GSX/Plane.
- A small "GSX Lua Library" (*GSX_AUTO*) which can also be used for other Planes - it does the GSX Menu Handling and some Automation. The Functions can be called with "LuaToggles" via FSUIPC. Or from your StreamDeck - when used together with PilotsDeck you can display the current (De-)Boarding and Cargo (Un-)Loading Progress and what is the current State / Service that can be called.
- A Lua Script to automate the Ground-Service Handling (*QW787_SYNC*) for the QualityWings 787. It opens / closes the respective Doors as requested from GSX, automatically after Boarding is completed or when the Cargo (Un-)Loading is finished. It will set / remove the Chocks & External Power and will move the Jetway/GPU automatically for you.<br/><br/>

# Installation
## QualityWings2GSX
You can basically place it anywhere you want - preferrably with your other Addons / Sim Files. Your AV-Scanner might being triggered, so place it in a Location where it is excluded.<br/>
The Tool is intended to being started when the Sim is running and the QualityWings 787 is loaded - otherwise it will just exit. You can, of course, start it manually or let it start automatically via FSUIPC. Add this to your FSUIPC6.ini for an automatic Start:
```
[Programs]
RunIf1=READY,KILL,X:\Path\to\the\Installation-Folder\QualityWings2GSX.exe
```
It will then be automatically started (and stopped) by FSUIPC when FSUIPC/the Sim is ready for Requests/Processing. With that Mechanic it will be loaded for every Plane, but don't worry: it closes immediately when the loaded Plane isn't a QW787.
## GSX_AUTO + QW787_SYNC
- Place the Files from the FSUIPC6 Folder in your FSUIPC6 Addon-Folder (the Folder where your FSUIPC6.ini is located)
- Either start the Scripts (Auto + Sync) manually ...
- ... or add them as Auto-Scripts to your FSUIPC6.ini. Start P3D/FSUIPC6 once so the Files are added (if you're not familiar with adding them manually). Then add the following to your FSUIPC6.ini:<br/>
```
[Auto.QW787]
1=Lua GSX_AUTO
2=Lua QW787_SYNC
```
Assuming your FSUIPC Profile is named "QW787"! Replace that with the correct Name. If already using Auto-Scripts, change the Numbers accordingly (these Scripts don't need to be run first - but these two in this Order!).<br/>
If you don't have a FSUIPC Profile for the QW787, start them as "Global"/"Default" Auto-Scripts:
```
[Auto]
1=Lua GSX_AUTO
2=Lua QW787_SYNC
```
If you plan to use the GSX_AUTO Script with other Planes, it has to be started as "Global"/"Default" Auto-Script!<br/>
If you're using the PilotsDeck Profile I have published: add the QW787_AUTO as second Script (GSX_AUTO -> QW787_AUTO -> QW787_SYNC)<br/><br/>

# Configuration
## QualityWings2GSX / SimBrief / GSX
### QualityWings2GSX
The most important Settings you have to set are your SimBrief's **Pilot ID** (look it up [here](https://www.simbrief.com/system/profile.php#settings)) and the Maximums for the Fuel-Tanks.<br/>
If you're not aware: there is a Discrepancy with the QW787 showing different Maximums for the Fuel-Quantity (or specifically Quantity-to-Weight-Ratios). The EFB and Dispatcher will report a higher Capacity than actually configured in the aircraft.cfg. This is not an Issue when using the EFB - 100% there will give you 100%. But when other Programs (GSX, my Tool) set the Tanks to 100% the EFB will report ~97% and EICAS a lower than expected Fuel-Weight. So you have two Options:
- (Recommended) Modify the aircraft.cfg (all three Variants) to use the Maximum Values configured in the Tool (so it everything is "aligned" to the same Maximums). It is *5737* gallons for the Wings and *23278* for the Center. The Maximum fuel Capacity to be used in SimBrief is *232809* lbs then.
- If you're not willing or don't want to modify the aircraft.cfg, modify the .config File of the Tool to use whatever you have in your aircraft.cfg. By Default it is *5570* gallons for the Wings and *22470* for the Center.<br/><br/>

The Tool is configured through the QualityWings2GSX.config File. Make sure you keep the XML-Syntax intact or else the Tool won't start!
- **pilotID**: Set this to your SimBrief Pilots ID!
- **constMaxWing**: Set this to the Value from your aircraft.cfg!
- **constMaxCenter**: Set this to the Value from your aircraft.cfg!
- *simbriefURL*: SimBrief's URL to fetch the current OFP in XML-Format. Is already set, needs only to be changed if SimBrief should change the URL.
- *fmsPath*: If you want the RTE File for that OFP directly downloaded to your FMS, set the Full Path here (typically <Path-to-P3D>\QualityWings\QW787\FlightPlans). If you don't want/need the RTE File or use SimBrief Downloader for that - just leave it empty!
- *useActualValue*: The SimBrief OFP has planned and actual Passenger/Bag Counts. The Tool is set to use the actual Value, so there is sometimes Variation. If you always want the planned Number, set it to false.
- *noCrewBoarding*: The Tool will disable GSX' Simulation to (De)Board the Pilots and Crew by default. If you want that to happen, set it to false.
- *constGPS*: It the Gallons-Per-Second the Plugin to simulate the Refueling. By default it is 16.5 gal/s which is 990 gal/min (the fastest Truck/Pump from GSX is 1000 gal/min, the slowest 800 gal/min). Or in "metric-speak": it is ~50kg/s which gives you 0.1t every 2 Seconds :wink:<br/>
Note that this is the *total* flow, it will be split across the Tanks (depending on how many need to be filled)! The Wings will be primarly filled, everything above their Capacity goes to Center. So if you do a "Short-Haul" only 2 Tanks will be filled in parallel (Wings), for a "Long-Haul" all 3 Tanks will be filled in parallel and when the Wings are full, the Center will be filled with the full flow.<br/>
Also note that this Value scales with (P3D's) Time Acceleration, so when using 2x Acceleration the Refueling will be twice as fast. The GSX Time Acceleration has no Effect on the Tools' Refuel Process.
- *constPaxDashX*: The Number Seats in Business;Economy for the given Variant. Already set to QW's Defaults.
- all other const Variables don't need and should not be touched - they are really constant Constants :laugh:
  
### SimBrief
Configure the Plane in SimBrief with **lbs** - even when you use only OFP's in kgs! The Passenger-Weight to be used is 190lbs and Bag-Weight is 55lbs. Whichever option you choose for the Fuel-Capacity: use the Total Capacity in gallons from your aircraft.cfg, multiply it with *6.69921875* and use the Result as the Max Fuel Capacity in SimBrief.
### GSX
Make sure you're Installation is updated and uses the Version released from January 31st 2022! FSDT has added Support for the QW787's Chocks, you can now release the Parking Brake and GSX will continue with its Service.<br/>
You have to change the GSX-Configuration for the Plane, it has now a "Custom Fuel System" - the Fuel-Dialog has to be disabled (for every Variant)! GSX will not refuel the Plane directly anymore, that is done by the Tool now - GSX will "only act" as if it would be Refueling the Plane. The Fuel-Capacity will of course only be increased as long as the Fuel-Hose is connected (when Joe finally manages to get it connected - he is soooo slow 😆) - multiple Trips are no Problem when your Stand has no Underground Fuel!<br/><br/>
Here what the Aircraft-Configuration should look like:<br/>
![GSX-Aircraft-Configuration](img/GSX-Aircraft-Settings.jpg)<br/><br/>
Here the Global-Configuration I use (as set per FSLabs Recommendation - nothing special for my Tool to set here, that I'm aware of):<br/>
![GSX-Global-Configuration](img/GSX-Global-Settings.jpg)<br/><br/>

## GSX_AUTO
This Script keeps track of the current Service State / Cycle and is used by QW787_SYNC to trigger GSX-Actions. It registers Flags to be used with LuaToggle Events so the Functions can be called by Joystick (or StreamDeck).<br/>
The most interesting are: "GSX_AUTO_CONNECT" (Flag# 9) and "GSX_AUTO_SERVICE_CYCLE" (Flag# 10). The first one will connect or disconnect the Jetway/GPU (whichever is available) and the second one automatically calls the next Service (Refuel -> Cater -> Board -> Push -> Push confirm -> ... after Touchdown -> Deboard -> start over with Refuel).
- *delayOperator*: The Time in ms the Tool will wait for you to select something in the Ground-Handler Dialog from GSX (before the next Action will be triggered). The Script expects this Dialog to be closed before it selects the Jetways/calls the GPU. The Delay will be applied when the Script is in the "Refuel" State and the Ground-Service is not "connected" (Jetway/GPU in Place) - this is typically at your first Leg when you start on the Parking Stand. So this Delay will not be applied when the Deboard Service is called - it is assumed you've already told GSX which Stand to use on your Taxi-In (and answered that Question then).
- *writeOffsets*: Only useful if you're using PilotsDeck (or anything else to Display Offsets). When enabled it will the (De)Boarding and (Un)Loading Progress to the two Offsets below. The Service State (and therefore which Service can be called) will always be written to the numeric Lvar "GSX_AUTO_SERVICE_STATE".

## QW787_SYNC
This Script is an essential Part of the GSX Integration/Automation. It will handle the Doors, Chocks/Ext Power and requests the Jetway/GPU.
The File in this Repository is configured for Usage without PilotsDeck, so *syncPilotsDeck* is disabled. *syncGSX* is enabled of course :laugh:
- *syncPilotsDeck*: Only interesting if you use PilotsDeck and/or the Profiles I've published (MCP Display + Buttons, Baro). Make sure the preconfigured Offsets are not in use!
- *syncCabin*: Turn the Cabin Lights on/off with the Overhead Cab/Util Button
- *syncBrake*: If you have have a Joystick/Input Device like the TCA, the Parking Brake will be synced to that Buttons State. Configure both brake-Variables accordingly.
- *syncFD*: Set the FO's FD Switch to the State of the Captain's Switch
- *syncGSX*: The essential Setting to enable the whole Integration/Automation. Only usable with the GSX_AUTO Script running!
- *syncChocksAndPower*: Set or Remove External Power available and Chocks when Jetway/GPU is connected or removed. *syncGSX* has to be true for that. Can be temporarily overidden with Tow Power set to ON (it will not touch the Variables as long this Button is on).
- *operateJetways*: Automatically remove Jetway/GPU before Push-Back or call Jetway/GPU and Deboard when Arrived. You can disable that if you want to handle that manually.<br/>
For Removal the Trigger is: GSX_AUTO is in Push-State (happens when Boarding has finished) *AND* Beacon is ON *AND* Jetway/GPU in Place *AND* Parking Brake is SET *AND* FwdExt Pwr is OFF.
For Call the Trigger is: GSX_AUTO is in Deboard-State (happens when the Engines are Off) *AND* Jetway/GPU not in Place *AND* Engines are STOPPED *AND* Beacon is OFF

# FAQ

