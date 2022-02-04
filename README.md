# QualityWings2GSX: SimBrief and GSX Integration for the QualityWings 787
If this Toolset is of interest to you, can be answered easily by yourself:
- Wished that you could just get the planned Fuel and Weights from your SimBrief OFP automatically by calling the GSX Services? Without any Payload Dialog or Dispatch Page / Dispatcher involved?
- Wished that the Plane was actually & progessively refuelled and (De-)Boarded when GSX is active? (Not refueling and boarding an already loaded Plane - really seeing the GW rise and fall)
- Wished that the Ground- and Cabin-Crew would just take care of the Doors, instead you "leaving" the Cockpit to open/close all that Doors?
- Wished the Jetway/GPU would just automatically be called / removed, the Chocks being set / removed and External Power connected / disconnected?<br/>

So if the Answer is 'yes' to any of these Questions: Here is a tool you might want to try out :wink:<br/>But before getting to excited, here the caveat: you need a registered Version of FSUIPC to use it!<br/><br/>

Since you're still interested, here the Features of the individual Tools/Scripts contained in this Toolset:
- An external Program (*QualityWings2GSX*) which will read your current SimBrief OFP and uses the Fuel Weights, Passenger and Bag Count (and their configured Weight in SimBrief) to progressively Refuel, Board and Deboard the Plane. It will set these Values in / use the Values with GSX so that the Refuel, Board and Deboard Animation is "in synch" with your OFP Data.<br/>It does not matter if your OFP is in kgs or lbs, if your APU is running or not and it is "turn-around-safe": After being deboarded the new OFP will be loaded and used for the next Refuel, Board and Deboard Cycle.
- A small "GSX Lua Library" (*GSX_AUTO*) which can also be used for other Planes - it does the GSX Menu Handling and some Automation. The Functions can be called with "LuaToggles" via FSUIPC. Or from your StreamDeck - when used together with PilotsDeck you can display the current (De-)Boarding and Cargo (Un-)Loading Progress and what is the current State / Service that can be called.
- A Lua Script to automate the Ground-Service Handling (*QW787_SYNC*) for the QualityWings 787. It opens / closes the respective Doors as requested from GSX, automatically after Boarding is completed or when the Cargo (Un-)Loading is finished. It will set / remove the Chocks & External Power and will move the Jetway/GPU automatically for you.

# Installation
## QualityWings2GSX
You can basically place it anywhere you want - preferrably with your other Addons / Sim Files. Your AV-Scanner might being triggered, so place it in a Location where it is excluded.<br/>
The Tool is intended to being started when the Sim is running and the QualityWings 787 is loaded - otherwise it will just exit. You can, of course, start it manually or let it start automatically via FSUIPC. Add this to your FSUIPC6.ini for an automatic Start:
```
[Programs]
RunIf1=READY,KILL,X:\Path\to\the\Installation-Folder\QualityWings2GSX.exe
```
It will then be automatically started (and stopped) by FSUIPC when FSUIPC/the Sim is ready for Requests/Processing. With that Mechanic it will be loaded for every Plane, but don't worry: it closes immediately when the loaded Plane isn't a QW787.

# Configuration
## QualityWings2GSX / SimBrief / GSX
### QualityWings2GSX
The most important Settings you have to set are your SimBrief's **Pilot ID** (look it up [here](https://www.simbrief.com/system/profile.php#settings) and the Maximums for the Fuel-Tanks.<br/>
If you're not aware: there is Discrepancy with the QW787 showing different Maximums for the Fuel-Quantity (or specifically Quantity-to-Weight-Ratios). The EFB and Dispatcher will report a higher Capacity than actually configured in the aircraft.cfg. This is not an Issue when using the EFB - 100% there will give you 100%. But when other Programs (GSX, my Tool) set the Tanks to 100% the EFB will report ~97% and EICAS a lower than expected Fuel-Weight. So you have two Options:
- (Recommended) Modify the aircraft.cfg (all three Variants) to use the Maximum Values configured in the Tool (so it everything is "aligned" to the same Maximums). It is *5737* gallons for the Wings and *23278* for the Center. The Maximum fuel Capacity to be used in SimBrief is *232809* then.
- If you're not willing or don't want to modify the aircraft.cfg, modify the .config File of the Tool to use whatever you have in your aircraft.cfg - by Default it is *5570* gallons for the Wings and *22470* for the Center.<br/>
### SimBrief
Configure the Plane in SimBrief with **lbs** - even when you use only OFP's in kgs! The Passenger-Weight to be used is 190lbs and Bag-Weight is 55lbs. Whichever option you choose for the Fuel-Capacity: use the Total Capacity in gallons from your aircraft.cfg, multiply it with *6.69921875* and use the Result as the Max Fuel Capacity in SimBrief.
### GSX
Make sure you're Installation is updated and uses the Version released from January 31st 2022! FSDT has added Support for the QW787's Chocks, you can now release the Parking Brake and GSX will continue with its Service.<br/>
You have to change the GSX-Configuration for the Plane, it has now a "Custom Fuel System" - the Fuel-Dialog has to be disabled! GSX will not refuel the Plan directly anymore, that is done by the Tool now - GSX will "only act" as if it would be Refueling the Plane. The Fuel-Capacity will of course only be increased as long as the Fuel-Hose is connected (when Joe finally manages to get it connected - he is soooo slow 😆) - multiple Trips are no Problem when your Stand has no Underground Fuel!<br/>
Here what the Aircraft-Configuration should look like:
[GSX-Aircraft-Configuration](img/GSX-Aircraft-Settings.jpg)<br/><br/>
Here the Global-Configuration I use (as set per FSLabs Recommendation - nothing special for my Tool to set here, that I'm aware of):
[GSX-Global-Configuration](img/GSX-Global-Settings.jpg)<br/><br/>

# Troubleshooting

