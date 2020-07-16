/*
 * CircuitControl.cpp
 *
 *  Created on: 13 mrt. 2018
 *      Author: shege
 */

#include "CircuitControl.hpp"

 //SearchforthisNamespace


using namespace Bittools;

namespace CircuitControl
{
    
void	CircuitControl::SetCircuitParameters(boolean enabled, uint16 refrigerant)
{
	Params.CircuitEnabled = enabled;
	// convert uint16 refirgerant type to enum
	if (refrigerant == 0){ Params.Refrigerant = Refrigerants::_R134a;}
	if (refrigerant == 1){ Params.Refrigerant = Refrigerants::_R513A;}
	if (refrigerant == 2){ Params.Refrigerant = Refrigerants::_R1234ze;}
};


void CircuitControl::Execute()
{
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


	//--------------------------------------------------------------------
	//CIRCUITSTATES


	// Alarm state when alarm is active
	if (Out.AlarmWord > 0)
	{


		Out.CircuitState = CIRCUITSTATE::Alarm;

	}
	// Disabled state if parameter circuit enabled is false
	if (Params.CircuitEnabled == false)
	{


		Out.CircuitState = CIRCUITSTATE::Disabled;
		PrgmState = PROGRAMSTATE::INIT;
	}
	// Idle state if par circuit enabled is true and Out.CircuitState is disabled, automatically re-init the state after disable or when circuit was stopping and compressor is not running(rpm<500)
	if ((Params.CircuitEnabled == true && Out.CircuitState == CIRCUITSTATE::Disabled) ||  (Out.CircuitState == CIRCUITSTATE::Stopping && (CMP.Out.ActualSpeed < 500)))
	{


		Out.CircuitState = CIRCUITSTATE::Standby;
	}
	// Go to starting state if compressor is ramping up, and was not ramping up
	if (	(CheckBitInWord(CMP.Out.ControlState, uint16(e_DTC_ControlStates::RampingUp))) && (!CheckBitInWord(LastCompressorState, uint16(e_DTC_ControlStates::RampingUp))))
	{


		Out.CircuitState = CIRCUITSTATE::Starting;
	}
	// Go to running if compressor was starting and is now in part vane
	if (CheckBitInWord(CMP.Out.ControlState, uint16(e_DTC_ControlStates::PartVane)) && (CheckBitInWord(LastCompressorState, uint16(e_DTC_ControlStates::RampingUp))))
	{


		Out.CircuitState = CIRCUITSTATE::Running;
	}
	// Go to stopping state if system was starting or running and cooling is now disabled ( cooling enable == false)
	if (
			((Out.CircuitState == CIRCUITSTATE::Starting) || (Out.CircuitState ==CIRCUITSTATE::Stopping )) && !In.CoolingEnabled)
	{


		Out.CircuitState = CIRCUITSTATE::Stopping;
	}

	Out.CircuitState	= uint16(Out.CircuitState);

	LastCompressorState = CMP.Out.ControlState;
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	// Run program

	switch ( PrgmState)
	{
	case PROGRAMSTATE::INIT:
	// Init program, only run the first loop

		// MAYBE: add init for the controls to reset all values

		PrgmState = PROGRAMSTATE::MAIN;


		break;

	case PROGRAMSTATE::MAIN:
	// Main Program, executed every loop.


//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
// ALARMS AND WARNINGS
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		// Manage alarms/warnings
		// Only enable alarms when circuit is enabled
		//First empty alarmword
		Out.AlarmWord 	= 0;

//		Bit Nr / Alarm

//
		/*0*//*Frost Protection*/  				SetBitInWord(In.Enabled && In.LEWT < Params.FrostProtectionLimit			, &Out.AlarmWord, 0);
		/*1*//*CW Low Flow*/  					SetBitInWord(In.Enabled && In.CwDp < Params.CwMinDpLimit					, &Out.AlarmWord, 1);
		/*2*//*High Cond Press*/	 			SetBitInWord(In.Enabled && In.ConPress > Params.MaxCondensingPressure		, &Out.AlarmWord, 2);
		/*3*//*Low Evap Press*/ 				SetBitInWord(In.Enabled && In.EvaSucPress < Params.MinSuctionPressure		, &Out.AlarmWord, 3);

		/*4*//*LEWT SensorFault*/				SetBitInWord(In.Enabled && In.SensorFaults.LewtSensor						, &Out.AlarmWord, 4);
		/*5*//*Eva PdT SensorFault*/			SetBitInWord(In.Enabled && In.SensorFaults.CwPdTSensor						, &Out.AlarmWord, 5);
		/*6*//*Con Press SensorFault*/			SetBitInWord(In.Enabled && In.SensorFaults.ConPressSensor					, &Out.AlarmWord, 6);
		/*7*//*Eva Press SensorFault*/			SetBitInWord(In.Enabled && In.SensorFaults.EvaSucPressSensor				, &Out.AlarmWord, 7);
		/*8*//*Eva Temp SensorFault*/			SetBitInWord(In.Enabled && In.SensorFaults.EvaSucTempSensor					, &Out.AlarmWord, 8);
//
//		/*9*//*Eva EEV Fault*/					SetBitInWord(In.Enabled && In.SensorFaults.EvaEEVfault						, &Out.AlarmWord, 9);


			/*TURBOCOR ALARMS*/
	//	/*10*//*CMP Communication Error */		SetBitInWord(In.Enabled && (CMP.Out.BMCCTemperature < 1 && CMP.Out.BMCCTemperature > 10000)		,&Out.AlarmWord, 10);	// Set boolean when BMCC temp is lower than 0 or higher than 10000
//		/*11*//*CMP High Inverter Temp */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,0)		,&Out.AlarmWord, 11);
//		/*12*//*CMP High Discharge Temp */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,1)		,&Out.AlarmWord, 12);
//		/*13*//*CMP Low Sucton Press*/			SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,2)		,&Out.AlarmWord, 13);
//		/*14*//*CMP High Discharge Press*/		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,3)		,&Out.AlarmWord, 14);
//		/*15*//*CMP 3Ph Overcurrent */			SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,4)		,&Out.AlarmWord, 15);
//		/*16*//*CMP High Cavity Temp */			SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,5)		,&Out.AlarmWord, 16);
//		/*17*//*CMP High Pressure Ratio */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,7)		,&Out.AlarmWord, 17);
//		/*18*//*CMP BMC Fault */				SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,8)		,&Out.AlarmWord, 18);
//		/*19*//*CMP Sensor Fault */				SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,9)		,&Out.AlarmWord, 19);
//		/*20*//*CMP High SCR Temperature */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,10)		,&Out.AlarmWord, 20);
//		/*21*//*CMP Locked-Out */				SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,11)		,&Out.AlarmWord, 21);
//		/*22*//*CMP Motor Windig Temp */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,12)		,&Out.AlarmWord, 22);
//		/*23*//*CMP High SSH */					SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,13)		,&Out.AlarmWord, 23);
//		/*24*//*CMP Earth Leakage */			SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,14)		,&Out.AlarmWord, 24);
//		/*25*//*CMP Softstart Temperature */	SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.CompressorAlarms,15)		,&Out.AlarmWord, 25);
//
//			/* Bearing Alarms*/
//
//		/*26*//*Bearing Calibration Failed*/	SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BearingFaults,4)			,&Out.AlarmWord, 26);
//		/*27*//*Bearing SelfTest Failed*/		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BearingFaults,5)			,&Out.AlarmWord, 27);
//		/*28*//*Bearing Axial Displacement*/	SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BearingFaults,6)			,&Out.AlarmWord, 28);
//		/*29*//*Bearing Axial Static Load*/		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BearingFaults,7)			,&Out.AlarmWord, 29);
//		/*30*//*Bearing FR Displacement X*/		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BearingFaults,8)			,&Out.AlarmWord, 30);
//		/*31*//*Bearing FR Displacement Y*/		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BearingFaults,9)			,&Out.AlarmWord, 31);
//		/*32*//*Bearing FR Static Load X*/		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BearingFaults,10)			,&Out.AlarmWord, 32);
//		/*33*//*Bearing FR Static Load Y*/		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BearingFaults,11)			,&Out.AlarmWord, 33);
//		/*34*//*Bearing RR Displacement X*/		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BearingFaults,12)			,&Out.AlarmWord, 34);
//		/*35*//*Bearing RR Displacement Y*/		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BearingFaults,13)			,&Out.AlarmWord, 35);
//		/*36*//*Bearing RR Static Load X*/		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BearingFaults,14)			,&Out.AlarmWord, 36);
//		/*37*//*Bearing RR Static Load Y*/		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BearingFaults,15)			,&Out.AlarmWord, 37);
//
//			/* BMC Alarms*/
//
//		/*38*//*Motor 1Ph Overcurrent */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,0)			,&Out.AlarmWord, 38);
//		/*39*//*DC Bus Overvoltage */			SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,1)			,&Out.AlarmWord, 39);
//		/*40*//*Motor High Current Fault */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,3)			,&Out.AlarmWord, 40);
//		/*41*//*Motor Inverter Error */			SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,4)			,&Out.AlarmWord, 41);
//		/*42*//*Motor Rotor may be locked*/ 	SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,5)			,&Out.AlarmWord, 42);
//		/*43*//*Motor Bearing Fault */			SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,6)			,&Out.AlarmWord, 43);
//		/*44*//*Voltage -> No Current */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,8)			,&Out.AlarmWord, 44);
//		/*45*//*DC Bus Under/Overvoltage */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,9)			,&Out.AlarmWord, 45);
//		/*46*//*24VDC Out of Range */			SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,10)			,&Out.AlarmWord, 46);
//		/*47*//*Low Motor Back EMF */			SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,11)			,&Out.AlarmWord, 47);
//		/*48*//*EEPROM Checksum Error */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,12)			,&Out.AlarmWord, 48);
//		/*49*//*Generator Mode */				SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,13)			,&Out.AlarmWord, 49);
//		/*50*//*SCR Phase Loss */				SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,14)			,&Out.AlarmWord, 50);
//		/*51*//*Compressor is Booting Up */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,15)			,&Out.AlarmWord, 51);
//
//			/* Sensor Alarms */
//
//		/*52*//*Inverter Temp Sensor Error*/	SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.SensorFaults,0)			,&Out.AlarmWord, 52);
//		/*53*//*Cavity Temp Sensor Error*/		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.SensorFaults,1)			,&Out.AlarmWord, 53);
//		/*54*//*Suction Temp Sensor Error*/		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.SensorFaults,2)			,&Out.AlarmWord, 54);
//		/*55*//*Discharge Temp SensorError*/	SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.SensorFaults,3)			,&Out.AlarmWord, 55);
//		/*56*//*Suction Press Sensor Error*/	SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.SensorFaults,4)			,&Out.AlarmWord, 56);
//		/*57*//*Discharge Press Sensor Error*/	SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.SensorFaults,5)			,&Out.AlarmWord, 57);
//		/*58*//*Invalid Bearing Calibration*/	SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.SensorFaults,6)			,&Out.AlarmWord, 58);
//		/*59*//*Inverter Cooling Control */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.SensorFaults,7)			,&Out.AlarmWord, 59);
//		/*60*//*Motor Cooling Control */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.SensorFaults,8)			,&Out.AlarmWord, 60);
//		/*61*//*Soft Start Temp Sensor Error*/	SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.SensorFaults,9)			,&Out.AlarmWord, 61);

			/* WARNINGS*/

		/* 0*//*Con PdT SensorFault*/			SetBitInWord(In.Enabled && In.SensorFaults.LewtSensor										,&Out.WarningWord, 0);
		/* 1*//*Con Liq Temp SensorFault*/		SetBitInWord(In.Enabled && In.SensorFaults.EvaSucPressSensor								,&Out.WarningWord, 1);
		/* 2*//*Eco Liquid Temp SensorFault*/	SetBitInWord(In.Enabled && Params.EconomizerEnabled && In.SensorFaults.EcoLiquidTempSensor	,&Out.WarningWord,2);
		/* 3*//*Eco Press SensorFault*/			SetBitInWord(In.Enabled && Params.EconomizerEnabled && In.SensorFaults.EcoSucPressSensor	,&Out.WarningWord,3);
		/* 4*//*Eco Temp SensorFault*/			SetBitInWord(In.Enabled && Params.EconomizerEnabled && In.SensorFaults.EcoSucTempSensor		,&Out.WarningWord,4);
		/* 5*//*Eco EEV Fault*/					SetBitInWord(In.Enabled && Params.EconomizerEnabled && In.SensorFaults.EcoEEVfault			,&Out.WarningWord,5);

		/* 6*//*Eva EEV Feedback */				SetBitInWord(In.Enabled && CheckMinmax(Out.EvaShValvePosition, 	(In.EvaEevFeedback-Params.MaxFeedbackDifference),	(In.EvaEevFeedback+Params.MaxFeedbackDifference)), &Out.WarningWord,6);
		/* 7*//*Eco EEV Feedback */				SetBitInWord(In.Enabled && CheckMinmax(Out.EcoShValvePosition, 	(In.EcoEevFeedback-Params.MaxFeedbackDifference), 	(In.EcoEevFeedback+Params.MaxFeedbackDifference)), &Out.WarningWord,7);
		/* 8*//*Hgb Mcv Feedback */				SetBitInWord(In.Enabled && CheckMinmax(Out.HGBValvePosition, 	(In.HgbMcvFeedback-Params.MaxFeedbackDifference),	(In.HgbMcvFeedback+Params.MaxFeedbackDifference)), &Out.WarningWord,8);
		/* 9*//*Con Ccv Feedback */				SetBitInWord(In.Enabled && CheckMinmax(Out.CPValvePosition, 	(In.CcvFeedback- Params.MaxFeedbackDifference), 	(In.CcvFeedback+Params.MaxFeedbackDifference)), &Out.WarningWord,9);


		/* 10*//*Motor High Current Warning */	SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,2)		,&Out.WarningWord, 10);
		/*11*//*Motor Bearing Warning */		SetBitInWord(In.Enabled && CheckBitInWord(CMP.Out.BMCSystemState,7)		,&Out.WarningWord, 11);


		if ( !Params.CircuitEnabled)
		{
			// Force alarm CircuitControlOutputs to 0 if circuit is disabled.
			Out.AlarmWord 	= 0;
			Out.WarningWord 	= 0;
		}

		//Disable cooling in case of alarm
		if (Out.AlarmWord > 0)
		{
			In.CoolingEnabled = false;
		}


		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		// END ALARMS AND WARNINGS
		//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//
//
//
//		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//			// Replace sensors in case of alarm
//
//			// Replace SuctionPressure sensor value with Turbocor value if sensor alarm is active
//			float64 EvaporatingPressure;
//
//			EvaporatingPressure = IO.EvaSucPress;
//			if ( IO.SensorFaults.EvaSucPressSensor == true){ EvaporatingPressure = CMP.IO.SuctionPressure - Params.DtcSucPressCompensation;};
//
//			// Replace SuctionPressure sensor value with Turbocor value if sensor alarm is active
//			float64 EvaporatingTemperature;
//			EvaporatingTemperature = IO.EvaSucTemp;
//			if ( IO.SensorFaults.EvaSucTempSensor== true){ EvaporatingTemperature = CMP.IO.SuctionTemperature- Params.DtcSucTempCompensation;};
//
//			// Replace Condenser pressure sensor value with Turbocor value if sensor alarm is active ** ONLY WHEN RUNNING??? NON RETURN VALVE!!
//			float64 CondenserPressure ;
//			CondenserPressure = IO.ConPress;
//			if ( IO.SensorFaults.ConPressSensor== true){ CondenserPressure = CMP.IO.DischargePressure- Params.DtcDisPressCompensation;};
//
//
//			//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

			// Run Circuit Controls
			// Parameters for the circuit controls are set via the control IO structure and set in the MyProgram // System Variables

			//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			// TURBOCORCONTROL
			//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			// Run TurbocorControl
			// Parameters are directly passed from MyProgram.cpp to TurbocorControl.cpp
			if (Out.CircuitState == CIRCUITSTATE::Starting)
			{
				//TODO Implement Start Demand timer??
				DemandToCompressor = Params.StartDemand;
			}
			DemandToCompressor = In.CoolingDemand;



			CMP.In.CoolingEnabled 	= In.CoolingEnabled;
			CMP.In.Demand			= DemandToCompressor;

			CMP.Params.MaxFROD = 100;

			// Run compressor communication
			CMP.Execute();
			// Close interlock based on cooling enabled

			CMP.Out.InterlockToCompressor = In.CoolingEnabled;


			//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			// EVAPORATOR CONTROL
			//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


			EVA.In.Enable 			= In.CoolingEnabled;
			EVA.In.Pressure 		= In.EvaSucPress;
			EVA.In.Temperature 		= In.EvaSucTemp;
			EVA.In.CompressorSpeed 	= CMP.Out.ActualSpeed;

			//Run code
			EVA.Execute();

			// CircuitControlOutputs
			Out.EvaShValvePosition = EVA.Out.ValvePosition;


			//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			// HOTGASBYPASS CONTROL
			//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

			// Get valve position
			HGB.In.Enable = In.CoolingEnabled;
			HGB.In.CompressorControlState = CMP.Out.ControlState;
			HGB.In.LEWT = In.LEWT;
			HGB.In.CompressorAtSurge = CMP.Out.CompressorAtSurge;
			HGB.In.CompressorPressureRatio = CMP.Out.PressureRatio;
			HGB.In.ForceValveOpen = false;

			HGB.Execute();

			Out.HGBValvePosition = HGB.Out.Output;


			//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			// CONDENSER PRESSURE CONTROL
			//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			//Run Condenser Pressure Control
//
//			CON.SetParameters(Params.Refrigerant, Params.CON.MinReference, Params.CON.MaxReference, Params.CON.ReferenceIncreaseRate, Params.CON.ReferenceDecreaseRate, Params.CON.Kp, Params.CON.Ki, Params.CON.Imax, Params.CON.Kd, Params.CON.Dmax, Params.CON.OutMin, Params.CON.OutMax);
//
//			IO.CPValvePosition = CON.GetValvePostion(CondenserPressure, CMP.IO.CompresssorAtChoke, true);
//
//			//Calculate subcooling
//
			Out.CondTemp			= GetSatTemp(Params.Refrigerant, In.ConPress,0);
			Out.ConLiquidSubCooling = GetSubCooling(Params.Refrigerant, In.ConPress, In.ConLiquidTemp);
//

		break;
	}



};

//Searchforthis }
}
