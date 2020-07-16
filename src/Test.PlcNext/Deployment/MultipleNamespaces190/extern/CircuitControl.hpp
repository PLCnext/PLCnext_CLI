/*
 * CircuitControl.hpp
 *
 *  Created on: 24 jan. 2018
 *      Author: shege
 *
 */


#include "Arp/System/Core/Arp.h"
#include "Arp/Plc/Commons/Esm/ProgramBase.hpp"
#include "Arp/System/Commons/Logging.h"


#include "../../Libraries/VhspUtilities/VhspUtilities.hpp"
#include "../../Libraries/TTimer/TTimer.hpp"
#include "../../Libraries/TurbocorControl/TurbocorControl.hpp"
#include "../../Libraries/SuperHeatControl/SuperHeatControl.hpp"
#include "../../Libraries/HotGasBypass/HotGasBypass.hpp"
#include "../../Libraries/CondenserPressureControl/CondenserPressureControl.hpp"
#include "../../Libraries/Press2Temp/Press2SatTemp.hpp"

//SearchforthisNamespace


using namespace Arp;
using namespace Arp::Plc::Commons::Esm;

 

#ifndef CIRCUITCONTROL_HPP_
#define CIRCUITCONTROL_HPP_

namespace CircuitControl
{

class CircuitControl {


public:
	struct testytes{
		int16 test1;
		int16 test2;
	};

	struct CircuitSensorFaults
	{
		boolean LewtSensor				= 0;
		boolean	EvaSucPressSensor		= 0;
		boolean	EvaSucTempSensor		= 0;
		boolean	ConPressSensor			= 0;
		boolean	ConLiquidTempSensor		= 0;
		boolean	EcoSucPressSensor		= 0;
		boolean	EcoSucTempSensor		= 0;
		boolean	EcoLiquidTempSensor		= 0;
		boolean	CwPdTSensor				= 0;
		boolean	SwPdTSensor				= 0;
		boolean	EvaEEVfault				= 0;
		boolean EcoEEVfault				= 0;
		boolean HgbMCVfault				= 0;

	};


	struct	Parameters
	{
		boolean				CircuitEnabled  		= false;
		Refrigerants 	Refrigerant = Refrigerants::_R134a;
		float64 			LewtSetpoint			= 0;
		float64			FrostProtectionLimit	= 2;			// Initialize to 2.0 degree, fail-safe if the parameter is not yet connected, water still cannot freeze
		float64 			MaxCondensingPressure	= 0;
		float64 			MinSuctionPressure		= 0;
		float64			CwMinDpLimit			= 0;
		float64			SwMinDpLimit			= 0;
		float64			StartDemand				= 0;
		float64			StartDemandTime			= 0;
		uint16			SHStartSpeed			= 0;
		int16			DtcSucPressCompensation	= 0;
		int16			DtcSucTempCompensation	= 0;
		int16			DtcDisPressCompensation	= 0;
		boolean				EconomizerEnabled		= false;
		uint16			MaxFeedbackDifference	= 0;

	};


	struct Inputs
	{
		//Inputs
			boolean		Enabled				= false;
			boolean		CoolingEnabled 		= false;
			boolean		ForceHGBOpen 		= false;
			float64	CoolingDemand	 	= 0;
			float64	LEWT				= 0;
			float64	CwDp 				= 0;
			float64	SwDp 				= 0;
			float64 	EvaSucPress 		= 0;
			float64	EvaSucTemp 			= 0;
			float64 	EvaEevFeedback		= 0;
			float64 	HgbMcvFeedback		= 0;
			float64	ConPress 			= 0;
			float64	ConLiquidTemp		= 0;
			float64	CcvFeedback			= 0;
			float64	EcoSucPress			= 0;
			float64	EcoSucTemp			= 0;
			float64	EcoLiquidTemp		= 0;
			float64 	EcoLiquidSubCooling = 0;
			float64 	EcoEevFeedback		= 0;

			CircuitSensorFaults				SensorFaults;
	};

	struct CircuitControlOutputs
	{
	public:
		//CircuitControlOutputs
			float64 	CondTemp			= 0.0;
			float64 	ConLiquidSubCooling	= 0.0;
			uint16	CircuitState		= 0;
			uint64 	AlarmWord			= 0;
			uint64 	WarningWord			= 0;
			float64	EvaShValvePosition 	= 0.0;
			float64	EcoShValvePosition 	= 0.0;
			float64	HGBValvePosition	= 0.0;
			float64	CPValvePosition		= 0.0;
	};
private:
	enum CIRCUITSTATE{
		Alarm		= 99,
		Disabled 	= 0,
		Standby 	= 1,
		Starting	= 2,
		Running		= 3,
		Stopping	= 4,
	};
	enum class PROGRAMSTATE{
		INIT = 0, MAIN =1
	};
	enum class e_DTC_ControlStates
		{
			// Control Modes
			CalibrationMode 		= 0,	//0x0000,
			LevitateOnlyMode		= 1,	//0x0001,
			ManualControlMode		= 2,	//0x0002,
			AnalogInputMode			= 3,	//0x0004,
			ModbusNetworkMode 		= 4,	//0x0008,

			//Compressor States
			LockedOut				= 5,	//0x0020,
			SystemInReset			= 6,	//0x0040,
			RampingUp				= 7,	//0x0080,
			PartVane				= 8,	//0x0100,
			NormalOperation			= 9,	//0x0200,
			MaxCapacity				= 10,	//0x0400,
			MinIGVPosition			= 11,	//0x0800,
			InterLockOpen			= 12,	//0x1000
			WaitingForReset			= 13,	//0x2000;
			CoolDown				= 14,	//0x4000;
			Standby					= 15,	//0x8000;
		};


private:
	//enums
	PROGRAMSTATE	PrgmState 		= PROGRAMSTATE::INIT;
	CIRCUITSTATE	CircuitState	= CIRCUITSTATE::Disabled;


	//Variables
	uint16	LastCompressorState = 0;






public:

	float64	DemandToCompressor	= 0;


	Parameters	Params;
	Inputs		In;
	CircuitControlOutputs		Out;

	//Functions
		TurbocorControl		CMP;
		SuperHeatControl	EVA;
		HotGasBypass		HGB;
		CondPressControl	CON;
	//	SuperHeatControl	ECO;






	void	SetCircuitParameters(boolean enabled, uint16 refrigerant);
	void 	Execute();
//

};

}


#endif /* CIRCUITCONTROL_HPP_ */
//Searchforthis }