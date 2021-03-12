using UnityEngine;
using System.Collections;

public class Scenario_04_wine_cellar_lab : Scenario {
private void Start() {}
////////////////////////
//BEGIN MOANING HORROR//
/*Start the creak sounds + ps when entering basement, stop them when leaving the basement/area
 */
public void FuncMoanHorrors(string  asParent, string  asChild, int alState)
{
	float fMoan = RandFloat(5.5f,15.5f);
	
	AddTimer("moanTimer", 4.5f+fMoan, "FuncMoanTimer");	
	
	AddTimer("childTimer", 0.5f+fMoan, "FuncMoanTimer");	
	
	/*DEBUG
	 */
	AddDebugMessage("Begin moaning sounds", true);
}
/*Random placment of creak at random time
 */
public void FuncMoanTimer(string  asTimer)
{
	int iMoan = RandFloat(2, 6);	
	float fMoan = RandFloat(5.5f,15.5f);
	
	if(asTimer == "moanTimer") {
		PlaySoundAtEntity("moanSound"+iMoan, "scare_male_terrified.snt", "HorrorMoan_"+iMoan, 0.0f, false);
		
		AddTimer("moanTimer", 6.5f+fMoan, "FuncMoanTimer");
	} 
	else if(asTimer == "childTimer") {
		iMoan = RandFloat(2, 6);	
		
		PlaySoundAtEntity("childSound"+iMoan, "scare_baby_cry.snt", "HorrorMoan_"+iMoan, 0.0f, false);
		
		AddTimer("childTimer", 8.5f+fMoan, "FuncMoanTimer");
	}

	/*DEBUG
	 */
	AddDebugMessage("Now moaning in area: "+iMoan, false);
}	
//END MOANING HORROR//
//////////////////////


///////////////////////////////
//BEGIN CREAKING WOOD CEILING//
/*Start the creak sounds + ps when entering basement, stop them when leaving the basement/area
 */
public void FuncCreakHorrors(string  asParent, string  asChild, int alState)
{
	if(asChild == "EnterCreakOnArea") {
		float fCreak = RandFloat(0.5f,6.5f);
	
		AddTimer("creak", 2.5f+fCreak, "FuncCreakTimer");
		
		AddEntityCollideCallback("Player", "EnterCreakOffArea", "FuncCreakHorrors", true, 1);
		
	} else {
		RemoveTimer("creak");
		
		AddEntityCollideCallback("Player", "EnterCreakOnArea", "FuncCreakHorrors", true, 1);
	}
	
	/*DEBUG
	 */
	AddDebugMessage("Player in/out creak: "+asChild, true);
}
/*Random placment of creak at random time
 */
public void FuncCreakTimer(string  asTimer)
{
	int iCreak = RandFloat(1, 7);	
	float fCreak = RandFloat(4.5f,14.5f);
	
	CreateParticleSystemAtEntity("creakPS"+iCreak, "ps_dust_falling_small_thin.ps", "HorrorCreak_"+iCreak, false);
	
	PlaySoundAtEntity("creakSound"+iCreak, "scare_wood_creak_mix.snt", "HorrorCreak_"+iCreak, 0.0f, false);
	
	AddTimer("creak", 5.5f+fCreak, "FuncCreakTimer");
	
	/*DEBUG
	 */
	AddDebugMessage("Now creaking in area: "+iCreak+" Next creak in: "+(5.5f+fCreak), true);
}	
//END CREAKING WOOD CEILING//
/////////////////////////////


//////////////////////
//BEGIN BREAK STAIRS//
public void CollideBreakStairs(string  asParent, string  asChild, int alState)
{
	if(GetGlobalVarString("key_study_1") != "key_study_1") return;
	
	BreakMyStairs();
}
public void BreakMyStairs()
{
	if(GetLocalVarInt("StairsWentDownTheDrain") == 1) return;
	
	SetEntityActive("cave_in_1", false);
	SetEntityActive("cavedx_*", true);
	
	SetEntityActive("AreaQuestStairs", true);
	
	PlaySoundAtEntity("monster_break","04_break.snt", "AreaCaveMonster", 0, false);
	
	CreateParticleSystemAtEntity("stairsPS", "ps_dust_break_stair.ps", "AreaStairsParticle", false);
	
	PlaySoundAtEntity("rocks2", "04_rock_break.snt", "AreaCaveMonster", 1.0f, false);
	
	PlayMusic("04_event_stairs.ogg", false, 1.0f, 0.5f, 10, false);
	StartScreenShake(0.05f, 0.5f, 1.5f,1.0f);
	SetPlayerMoveSpeedMul(0.8f);
	SetPlayerRunSpeedMul(0.5f);
	MovePlayerHeadPos(0.0f, -0.4f, 0.0f, 0.2f, 0.1f);
	SetPropHealth("stairs_wood_1", 0);
	AddTimer("sanity", 1, "TimerSanity");
	AddTimer("sanity2", 2.5f, "TimerSanity");
	AddTimer("sanity3", 4, "TimerSanity");
	AddTimer("sanity4", 5, "TimerSanity");
	AddTimer("sanity5", 7, "TimerSanity");
	
	SetEntityPlayerInteractCallback("PlatformDynStickLift_1", "GiveStackHint", true);
	SetEntityPlayerInteractCallback("PlatformDynStickLift_2", "GiveStackHint", true);
	
	SetEntityActive("StickLift_1", true);
	
	SetLocalVarInt("StairsWentDownTheDrain", 1);
	
	RemoveTimer("rockbreath");
	
	//Turn off some lights!
	SetLampLit("torch_static01_16", false, true);
	SetLampLit("torch_static01_13", false, true);
	SetLampLit("torch_static01_12", false, true);
	SetLampLit("torch_static01_14", false, true);
}
public void TimerSanity(string  asTimer)
{
	if(asTimer == "sanity"){
		PlaySoundAtEntity("stairsS", "break_stairs.snt", "AreaStairsParticle", 0.5f, false);
		GiveSanityDamage(10, true);
		FadeSepiaColorTo(0.5f, 0.025f);
		FadeRadialBlurTo(0.1f, 0.025f);
		SetRadialBlurStartDist(0.2f);
		MovePlayerHeadPos(0.0f, 0, 0.0f, 1, 0.1f);
		SetPlayerMoveSpeedMul(0.6f);
		SetPlayerRunSpeedMul(0.4f);
	}
	else if(asTimer == "sanity2"){
		FadeImageTrailTo(1.0f, 0.1f);
		PlayGuiSound("react_scare", 0.8f);
		SetPlayerMoveSpeedMul(0.7f);
		SetPlayerRunSpeedMul(0.6f);
	}
	else if(asTimer == "sanity3"){
		PlaySoundAtEntity("rocks", "04_rock.snt", "AreaCaveMonster", 0.0f, false);
		SetPlayerMoveSpeedMul(0.8f);
		SetPlayerRunSpeedMul(0.7f);
	}
	else if(asTimer == "sanity4"){
		PlaySoundAtEntity("rocks3", "04_rock.snt", "AreaCaveMonster", 0.0f, false);
		PlaySoundAtEntity("monster_break","04_break02.snt", "AreaCaveMonster", 0, false);
		PlayGuiSound("react_breath", 0.7f);
		FadeSepiaColorTo(0, 0.1f);
		FadeRadialBlurTo(0, 0.1f);
		SetPlayerMoveSpeedMul(0.9f);
		SetPlayerRunSpeedMul(0.8f);
	}
	else if(asTimer == "sanity5"){
		FadeImageTrailTo(0.0f, 1.0f);
		PlayGuiSound("react_breath", 0.5f);
		SetPlayerMoveSpeedMul(1.0f);
		SetPlayerRunSpeedMul(1.0f);
	}
}

public void GiveStackHint(string  entity)
{
	GiveHint("stackhint", "Hints", "StickyArea", 0);
}

/*Extra space to stick the one for the ladder area
 */
public void CollideStickLiftExtra(string  asParent, string  asChild, int alState)
{
	string sArea = StringSub(asChild, 0, 11);

	AddDebugMessage("Name: " + sArea, false);
	
	if(GetLocalVarString("StickOneTime") == sArea) return;
	SetLocalVarString("StickOneTime", sArea);
	
	SetEntityActive(sArea, false);
	
	AddTimer(asParent+"_Body_1", 0.35f, "TimerChangeEntity");		
	AddTimer(sArea, 0.35f, "TimerChangeEntity");
}

public void CollideStickLift(string  asArea, string  asBody)
{
	/*Only allow one board to be stuck to an area
	 */
	if(GetLocalVarString("StickOneTime") == asArea) return;
	SetLocalVarString("StickOneTime", asArea);
	
	/*Remove object with grab and activate object with static and joint
	 */
	AddTimer(asBody, 0.35f, "TimerChangeEntity");
	AddTimer(asArea, 0.35f, "TimerChangeEntity");
}
public void TimerChangeEntity(string  asTimer)
{	
	if(asTimer == "PlatformDynStickLift_1_Body_1") SetEntityActive("PlatformDynStickLift_1", false);
	else if(asTimer == "PlatformDynStickLift_2_Body_1") SetEntityActive("PlatformDynStickLift_2", false);

	if(asTimer == "StickLift_2") SetEntityActive("LadderArea_2", true);
	
	SetEntityActive("Platform"+asTimer, true);
}

public void CollideQuestStairs(string  asParent, string  asChild, int alState)
{
	AddQuest("04Stairs", "04Stairs");
	
	SetEntityActive("AreaQuestStairs_2", true);
}
public void CollideQuestStairs02(string  asParent, string  asChild, int alState)
{
	CompleteQuest("04Stairs", "04Stairs");
}
//END BREAK STAIRS//
////////////////////


//////////////////////
//BEGIN ACID MACHINE//
/*Use jar on machine to place it under pipe
 */
public void ItemJarOnMachine(string  asItem, string  asEntity)
{
	SetEntityActive("chemical_container_static_1", true);
	
	RemoveItem(asItem);
	
	PlaySoundAtEntity("PlaceJar","puzzle_place_jar", "AreaCompleteSuccess", 0, false);
	
	SetLocalVarInt("chemical_container_static_1", 1);
}
/*Use chemicals on machine || valves to add it to bottles
 */
public void AddChemical(string  asItem, string  asEntity)
{
	RemoveItem(asItem);
	
	AddLocalVarInt("ChemicalsInMachine", 1);
	
	SetEntityActive("JarEmpty"+asItem, false);
	SetEntityActive("Jar"+asItem, true);
	
	PlaySoundAtEntity(asItem+"Sound","puzzle_add_chemical.snt", asEntity, 1, false);
	FadeLightTo("Light"+asItem, -1, -1, -1, -1, 0.5f, 0.1f);
	
	//Moved sanity boost to first instead of last, migth be better to boost early if player didn't get all 4.
	if(GetLocalVarInt("ChemicalsInMachine") == 1)
		GiveSanityBoostSmall();
		
	if(GetLocalVarInt("ChemicalsInMachine") == 3 )
		BreakMyStairs();
	
	if(GetLocalVarInt("ChemicalsInMachine") == 4){
		CompleteQuest("04FindChemicals","04FindChemicals");
		UnBlockHint("EntityWheel");
		for(int i=1;i<=4;i++) 
			SetEntityPlayerInteractCallback("Valve_"+i, "InteractAcidMachine02", true);	
		SetLocalVarInt("DoBigFeet", 0); 
	}
}
/*When touching the vales
 */
public void InteractAcidMachine(string  asEntity)
{
	if(GetLocalVarInt("ChemicalsInMachine") != 4)
	{
		AddQuest("04FindChemicals","04FindChemicals");
		SetMessage("Ch01Level04", "InteractAcidMachineNoChem",-1);
	}
	else if(GetLocalVarInt("ChemicalsInMachine") == 4)
	{
		SetWheelStuckState("ValveIgnite", 0, false);
	}
}
public void InteractAcidMachine02(string  asEntity)
{
	if(GetLocalVarInt("ValveIgniteFirst") == 0)
	{
		SetMessage("Ch01Level03", "MachineNeedsToStart", 0);
		SetLocalVarInt("ValveIgniteFirst", 1);
		for(int i=1;i<=4;i++) 
			SetEntityPlayerInteractCallback("Valve_"+i, "InteractAcidMachine", false);	
	}
}
/*Moving the big valve will turn on the burners and ready the machine
 */
public void InteractTurnOnAcidMachine(string  asName, string  asMainEntity, string  asConnectEntity, int alState)
{
	AddDebugMessage("Connect "+asMainEntity+" and "+asConnectEntity+" state:"+alState, false);
	
	if(alState == 1)
	{
		for(int i=1;i<=4;i++) {
			CreateParticleSystemAtEntity(asName+"Fire"+i, "ps_fire_candle.ps", "Valve_"+i+"_AreaBottle", false);
			
			FadeLightTo("Valve_"+i+"_Light", -1, -1, -1, -1, 0.4f, 1);
			
			SetWheelStuckState("Valve_"+i, 0, true);
		} 
		
		SetLocalVarInt("ValveIgniteFirst", 1);
		
		FadeLightTo("LightBurn", -1, -1, -1, -1, 3.0f, 2);
		
		PlaySoundAtEntity("IgniteSound", "general_fire_burning_low", "Valve_1_AreaBottle", 1, false);
		
		SetWheelStuckState("ValveIgnite", 1, true);
	} 
	else if(alState == -1)
	{
		for(int i=1;i<=4;i++){
			DestroyParticleSystem(asName+"Fire"+i);
			
			FadeLightTo("Valve_"+i+"_Light", -1, -1, -1, -1, 0, 1);
			
			SetWheelStuckState("Valve_"+i, -1, true); 
		} 		
		
		FadeLightTo("LightBurn", -1, -1, -1, -1, 0, 2);
		
		StopSound("IgniteSound", 1);
	}
}
/*Turning the four valves on/off to try and get the sequence right
 */
public void InteractTurnValve(string  asName, string  asMainEntity, string  asConnectEntity, int alState)
{
	/*START WHAT HAPPENS WHEN VAVLES ARE TURNED ON
	 */
	if(alState == 1) {

		 /*Mark Valve as on
		  */
		//SetLocalVarInt(asMainEntity, 1);
		AddLocalVarInt("ValvesNrOn", 1);
		
		AddDebugMessage("ValvesNrOn: "+GetLocalVarInt("ValvesNrOn")+" And Feet "+GetLocalVarInt("DoBigFeet"), false);
		
		if(GetLocalVarInt("ValvesNrOn") == 0){
			SetWheelStuckState("ValveIgnite", -1, true); 

			PlaySoundAtEntity("FinalBoil","puzzle_boil.snt", "Valve_4_AreaValve", 1, false);
			
			CreateParticleSystemAtEntity("FinalSteam", "ps_acid_machine_bubble_large02.ps", "Valve_4_AreaValve", false); 
			CreateParticleSystemAtEntity("FinalFlow", "ps_acid_machine_bubble_end.ps", "AreaCompleteSuccess", false); 
			
			FadeLightTo("LightBurn", -1, 1, -1, -1, 3.0f, 5);
			FadeLightTo("LightAcid", -1, -1, -1, -1, 0.3f, 2);
			
			AddTimer("done", 2, "TimerAcidDone");	
			
		} else DoEffectLarge(asName, asConnectEntity);
		
		/*Only an event, nothing puzzle related
		 */
		if(GetLocalVarInt("ValvesNrOn") == -2 && GetLocalVarInt("DoBigFeet") == 0){ 
			AddTimer("Feet_1", 2.5f, "TimerBigFeet"); 
			PlaySoundAtEntity("bang","general_thunder.snt", "Player", 0, false);
			PlaySoundAtEntity("biggus","04_big_feet.snt", "Player", 0, false); 
			SetLocalVarInt("DoBigFeet", 1); 
		} 
		
		/*If three vavles on, reset the machine
		 */
		/*if((GetLocalVarInt("Valve_1")+GetLocalVarInt("Valve_2")+GetLocalVarInt("Valve_3")+GetLocalVarInt("Valve_4")) > 2 ) {
			for(int i=1;i<=4;i++) SetWheelStuckState("Valve_"+i, -1, true); 
			
			AddTimer("release", 0.1f, "TimerReleaseValves");
			return;
		}*/

		/*If the valve turned on is the correct order move ahead, if not hint of wrong order
		 */
		/*if(asMainEntity == "Valve_1"){
			if(GetLocalVarInt("RoadToSuccess") == 0) DoEffectLarge(asName, asConnectEntity);
			else DoEffectSmall(asName, asConnectEntity);
		} 
		else if(asMainEntity == "Valve_2"){
			if(GetLocalVarInt("RoadToSuccess") == 1) DoEffectLarge(asName, asConnectEntity);
			else DoEffectSmall(asName, asConnectEntity);	
		} 
		else if(asMainEntity == "Valve_3"){
			if(GetLocalVarInt("RoadToSuccess") == 3){ 
				DoEffectLarge(asName, asConnectEntity);
				
				//This has nothing to do with puzzle, it's a pure event in level triggered by how far puzzle completed
				if(GetLocalVarInt("DoBigFeet") != 1){ AddTimer("Feet_1", 2.5f, "TimerBigFeet"); PlaySoundAtEntity("bang","general_thunder.snt", "Player", 0, false);
					PlaySoundAtEntity("biggus","04_big_feet.snt", "Player", 0, false); SetLocalVarInt("DoBigFeet", 1); 
				} 
			} 
			else DoEffectSmall(asName, asConnectEntity);	
		} 
		else if(asMainEntity == "Valve_4"){
			if(GetLocalVarInt("RoadToSuccess") == 5) DoEffectLarge(asName, asConnectEntity);
			else DoEffectSmall(asName, asConnectEntity);
		}*/
	} 
	
	/*START WHAT HAPPENS WHEN VAVLES ARE TURNED OFF
	 */
	else if(alState == -1){

		 /*Mark valve as off
		  */
		//SetLocalVarInt(asMainEntity, 0);
		
		AddLocalVarInt("ValvesNrOn", -1);
		
		DestroyParticleSystem(asName+"PSteam");
		StopSound(asName+"SBoil",1); StopSound(asName+"SSteam",1);
		
		/*If valved turned off in the right order, allow for attempt at turning the next valve in the correct order
		 */
		/*if(asMainEntity == "Valve_1" && GetLocalVarInt("RoadToSuccess") == 2) AddLocalVarInt("RoadToSuccess", 1);
		
		else if(asMainEntity == "Valve_2" && GetLocalVarInt("RoadToSuccess") == 4) AddLocalVarInt("RoadToSuccess", 1);	
		
		else if(asMainEntity == "Valve_3" && GetLocalVarInt("RoadToSuccess") == 6){ //Full sequence correct, spit out the chemical substance
			SetWheelStuckState("ValveIgnite", -1, true); 

			PlaySoundAtEntity("FinalBoil","puzzle_boil.snt", "Valve_4_AreaValve", 1, false);
			
			CreateParticleSystemAtEntity("FinalSteam", "ps_acid_machine_bubble_large02.ps", "Valve_4_AreaValve", false); 
			CreateParticleSystemAtEntity("FinalFlow", "ps_acid_machine_bubble_end.ps", "AreaCompleteSuccess", false); 
			
			FadeLightTo("LightBurn", -1, 1, -1, -1, 3.0f, 5);
			FadeLightTo("LightAcid", -1, -1, -1, -1, 0.3f, 2);
			
			AddTimer("done", 2, "TimerAcidDone");	
		} 
		else if(asMainEntity == "Valve_4" && GetLocalVarInt("RoadToSuccess") == 6) AddLocalVarInt("RoadToSuccess", -1);*/
		
		/*If all valves are off, reset the machine
		 */
		/*if((GetLocalVarInt("Valve_1")+GetLocalVarInt("Valve_2")+GetLocalVarInt("Valve_3")+GetLocalVarInt("Valve_4")) == 0 )
			SetLocalVarInt("RoadToSuccess", 0);*/
	}
	
	AddDebugMessage("Rate of success "+GetLocalVarInt("RoadToSuccess"), false);
	AddDebugMessage(asMainEntity+" is turned to "+alState, true);
}
/*Large effects as correct valve rotated
 */
public void DoEffectLarge(string  asName, string  asWhere)
{
	CreateParticleSystemAtEntity(asName+"PSteam", "ps_acid_machine_bubble_large.ps", asWhere, false); 
	
	PlaySoundAtEntity(asName+"SBoil","puzzle_boil_low.snt", asWhere, 1, false);
	PlaySoundAtEntity(asName+"SSteam","puzzle_gas.snt", asWhere, 1, false);
	
	AddLocalVarInt("RoadToSuccess", 1); 
}
/*Small effects as incorrect valve rotated
 */
public void DoEffectSmall(string  asName, string  asWhere)
{
	CreateParticleSystemAtEntity(asName+"PSteam", "ps_acid_machine_bubble_small.ps", asWhere, false); 
	
	PlaySoundAtEntity(asName+"SBoil","puzzle_boil_low.snt", asWhere, 1, false);
}
/*When reseting machine on more than 2 valves turned, this timer turns it on again
 */
public void TimerReleaseValves(string  asTimer)
{
	for(int i=1;i<=4;i++) SetWheelStuckState("Valve_"+i, 0, false);  
	
	SetLocalVarInt("RoadToSuccess", 0);
}
/*The acid spit on success, if no jar present the acid will just spill and it is possible to try again
 */
public void TimerAcidDone(string  asTimer)
{
	DestroyParticleSystem("Part4PSteam");
	DestroyParticleSystem("FinalSteam");

	PlaySoundAtEntity("AcidDone","puzzle_acid", "AreaCompleteSuccess", 0, false);
	
	StopSound("FinalBoil",1);
	
	FadeLightTo("LightAcid", -1, -1, -1, -1, 0, 4);
	FadeLightTo("LightBurn", -1, 0.3f, -1, -1, 0, 3);
	
	SetWheelStuckState("ValveIgnite", 0, true); 
	
	if(GetLocalVarInt("chemical_container_static_1") == 1){
		SetEntityActive("chemical_container_static_1", false);
		SetEntityActive("chemical_container_2", true);
		//SetPropActiveAndFade("chemical_container_static_1", false, 0.5f);
		//SetPropActiveAndFade("chemical_container_2", true, 0.5f);
	
		PlaySoundAtEntity("AcidSuccess","puzzle_acid_success", "AreaCompleteSuccess", 0, false);
		PlayMusic("04_puzzle_acid.ogg", false, 0.7f, 0.5f, 10, false);
		//GiveSanityBoostSmall();
		
		SetWheelStuckState("ValveIgnite", -1, true); 
		
	} 
	else {
		PlaySoundAtEntity("AcidFail","puzzle_acid_fail", "AreaCompleteSuccess", 0, false);
		SetMessage("Ch03Level26", "NoContainerBelowSqueezer", 0);
	}
}
public void PickFinalAcid(string  asEntity, string  asType)
{
	GiveSanityBoostSmall();
	
	SetGlobalVarString(asEntity, asEntity);
	
	AddTimer("Thunder", 0.75f, "TimerEnterClank");
}
public void EntityCallPickEmptyChem(string  asEntity, string  type)
{
	GiveSanityBoostSmall();
}

public void EntityCallPickNote(string  asEntity, string  type)
{
	AddQuest("04FindChemicals","04FindChemicals");
}
public void EntityCallPickNote02(string  asEntity, string  type)
{
	AddQuest("04ChemicalsMoved","04ChemicalsMoved");
}
//END ACID MACHINE//
////////////////////


///////////////////////////
//BEGIN LOOKING DOWN HOLE//
public void CollideActiveHole(string  asParent, string  asChild, int alState)
{
	SetEntityActive("AreaLookHole", true);
}
public void LookAtHole(string  entity, int alState)
{
	if(alState != 1) return;

	PlayMusic("04_event_hole.ogg", false, 0.7f, 0.5f, 10, false);
	
	AddTimer("TimerLookHole", 0.1f, "TimerLookHole");	
}
public void TimerLookHole(string  asTimer)
{	
	/*Configurables
	 */
	int iMaxEventStep = 12;		//How many steps there are in the switch event
	float fEventSpeed = 1.0f;	//The default time between steps in an event
	
	/*Helpers - Do not edit
	 */
	string sEvent = asTimer;	//Using first timer name for variable, timer name & callback for the timer that loops
	AddLocalVarInt(sEvent, 1);	//What step to play in the event

	/*Steps in the event
	 */
	switch(GetLocalVarInt(sEvent)){
		case 1:
			PlaySoundAtEntity("HoleCry","04_hole_squeal", "AreaHoleEffects", 0, false);
			StartScreenShake(0.004f, 6, 2, 2);
			GiveSanityDamage(10.0f, true);
		break;
		case 2:
			PlaySoundAtEntity("HoleWater","04_water_puff.snt", "AreaHoleEffects", 0, false);
			CreateParticleSystemAtEntity("HolePS1", "waterlurker_run_splash.ps", "AreaHoleEffects", false); 
			fEventSpeed = 0.25f;
		break;
		case 3:
			PlaySoundAtEntity("breath4","react_breath", "Player", 0.25f, false);
			PlaySoundAtEntity("HoleWater2","04_water_puff.snt", "AreaHoleEffects", 0, false);
			fEventSpeed = 0.5f;
		break;
		case 4:
			PlaySoundAtEntity("HoleWater3","04_water_puff.snt", "AreaHoleEffects", 0, false);
			CreateParticleSystemAtEntity("HolePS2", "waterlurker_run_splash.ps", "AreaHoleEffects", false); 
		break;
		case 5:
			PlaySoundAtEntity("HoleWater4","04_water_puff.snt", "AreaHoleEffects", 0, false);
			fEventSpeed = 0.5f;
		break;
		case 6:
			PlaySoundAtEntity("breath1","react_breath", "Player", 0.4f, false);
			PlaySoundAtEntity("HoleWater5","04_water_puff.snt", "AreaHoleEffects", 0.4f, false);
			CreateParticleSystemAtEntity("HolePS3", "waterlurker_run_splash.ps", "AreaHoleEffects", false); 
			fEventSpeed = 0.25f;
		break;
		case 7:
			PlaySoundAtEntity("HoleWater6","04_water_puff.snt", "AreaHoleEffects", 0.75f, false);
			fEventSpeed = 0.75f;
		break;
		case 8:
			PlaySoundAtEntity("HoleWater7","04_water_puff.snt", "AreaHoleEffects", 1, false);
		break;
		case 9:
			PlaySoundAtEntity("breath3","react_breath", "Player", 0.6f, false);
			PlaySoundAtEntity("HoleWater8","04_water_puff.snt", "AreaHoleEffects", 0.4f, false);
			CreateParticleSystemAtEntity("HolePS3", "waterlurker_run_splash.ps", "AreaHoleEffects", false); 
			fEventSpeed = 0.25f;
		break;
		case 10:
			PlaySoundAtEntity("HoleWater9","04_water_puff.snt", "AreaHoleEffects", 0.75f, false);
			fEventSpeed = 0.75f;
		break;
		case 11:
			PlaySoundAtEntity("HoleWater10","04_water_puff.snt", "AreaHoleEffects", 1, false);
		break;
		case 12:
			PlaySoundAtEntity("breath2","react_breath", "Player", 1, false);
		break;
	}
	
	if(GetLocalVarInt(sEvent) <= iMaxEventStep) AddTimer(sEvent, fEventSpeed, sEvent);
}
//END LOOKING DOWN HOLE//
/////////////////////////



///////////////////////
//BEGIN SPIDER CAVEIN//
public void CollideActivateSpider(string  asParent, string  asChild, int alState)
{
	SetEntityActive("AreaBeginSpider_1", true);
	AddDebugMessage("In Area 1", false);
	
	SetLocalVarFloat("RockBreath", 2.0f);
	
	AddTimer("rockbreath", 0.2f, "TimerRockBreath");	
}
public void TimerRockBreath(string  asTimer)
{
	if(GetLocalVarFloat("RockBreath") > 0.5f){
		AddLocalVarFloat("RockBreath", -0.2f);
		AddLocalVarFloat("RockShake", 0.001f);
	}
	
	PlaySoundAtEntity("monster_breath","04_breath.snt", "AreaCaveMonster", GetLocalVarFloat("RockBreath"), false);
	
	StartScreenShake(GetLocalVarFloat("RockShake"), 0.5f, 2.0f, 1.0f);
	CreateParticleSystemAtEntity("breathps", "ps_cave_monster_breath", "AreaCaveMonster", false);
	
	AddTimer("rockbreath", 5, "TimerRockBreath");	
	
	AddDebugMessage("Value: "+GetLocalVarFloat("RockBreath"), false);
}
public void LookAtCave(string  asEntity, int alState)
{
	PlaySoundAtEntity("monster_scream","04_warn.snt", "AreaCaveMonster", 0, false);
	
	StartScreenShake(0.007f, 2.0f, 0.5f, 2.0f);
	
	CreateParticleSystemAtEntity("breathps", "ps_cave_monster_warn.ps", "AreaCaveMonster", false);	

	AddTimer("monster1", 0.5f, "TimerRockMonster");
	AddTimer("monster2", 2.0f, "TimerRockMonster");
	AddTimer("monster3", 3.0f, "TimerRockMonster");
	AddTimer("monster4", 5.0f, "TimerRockMonster");
}
public void TimerRockMonster(string  asTimer)
{
	if(asTimer == "monster1"){
		StartPlayerLookAt("AreaCaveMonster", 1.0f, 1.0f, "");
		PlayGuiSound("react_scare", 0.6f);
		GiveSanityDamage(5.0f, false);
		FadeRadialBlurTo(0.05f, 0.025f);
		SetRadialBlurStartDist(0.1f);
	}
	else if(asTimer == "monster2"){
		StopPlayerLookAt();
	}
	else if(asTimer == "monster3"){
		PlayGuiSound("react_creath", 0.7f);
		FadeRadialBlurTo(0, 0.1f);
	}
	else{
		PlayGuiSound("react_creath", 0.5f);
	}	
}
public void CollideScreamCave(string  asParent, string  asChild, int alState)
{
	PlaySoundAtEntity("monster_scream","04_scream.snt", "AreaCaveMonster", 0, false);
	
	StartScreenShake(0.02f, 2.0f, 0.5f, 2.0f);
	
	CreateParticleSystemAtEntity("breathps", "ps_cave_monster_scream.ps", "AreaCaveMonster", false);	
	
	AddTimer("scream1", 0.5f, "TimerCaveScream");
	AddTimer("scream2", 1.0f, "TimerCaveScream");
	AddTimer("scream3", 3.0f, "TimerCaveScream");
	AddTimer("scream4", 5.0f, "TimerCaveScream");
}
public void TimerCaveScream(string  asTimer)
{
	if(asTimer == "scream1"){
		PlayGuiSound("react_scare", 0.8f);
		GiveSanityDamage(10.0f, false);
		FadeSepiaColorTo(0.5f, 0.025f);
		FadeRadialBlurTo(0.1f, 0.025f);
		SetRadialBlurStartDist(0.2f);
	}
	else if(asTimer == "scream2"){
	
	}
	else if(asTimer == "scream3"){
		PlayGuiSound("react_creath", 0.8f);
		FadeSepiaColorTo(0, 0.1f);
		FadeRadialBlurTo(0, 0.1f);
	}
	else{
		PlayGuiSound("react_creath", 0.6f);
	}
}
//END SPIDER CAVEIN//
/////////////////////


//////////////////
//BEGIN BIG FEET//
public void TimerBigFeet(string  asTimer)
{	
	if(asTimer == "Feet_1"){
		AddTimer("Feet_2", 1, "TimerBigFeet");
		AddTimer("Feet_3", 2.5f, "TimerBigFeet");
		AddTimer("Feet_4", 4, "TimerBigFeet");
		AddTimer("Feet_5", 5, "TimerBigFeet");
		
		DoSteps(0.007f,1.0f,0.75f,0.3f,asTimer);
	}
	else if(asTimer == "Feet_2"){
		GiveSanityDamage(10.0f, true);
		DoSteps(0.008f,0.3f,0.8f,0.3f,asTimer);
	}
	else if(asTimer == "Feet_3") DoSteps(0.01f,0.5f,0.9f,0.4f,asTimer);
	else if(asTimer == "Feet_4") DoSteps(0.008f,0.7f,0.7f,0.3f,asTimer);
	else if(asTimer == "Feet_5") DoSteps(0.007f,1.0f,1.5f,3.0f,asTimer);
}
public void DoSteps(float fShake, float fFade, float fShakeL, float fShakeF, string  asWhere)
{
	StartScreenShake(fShake, fShakeL, 0.15f, fShakeF);
	PlaySoundAtEntity("sigh"+asWhere,"react_breath.snt", "Player", fFade, false);
	CreateParticleSystemAtEntity("step"+asWhere, "ps_dust_falling_big_feet.ps", "Area"+asWhere, false); 
}
//END BIG FEET//
////////////////


//////////////////////////////
//BEGIN ENTER LEVEL CLANKS//
public void TimerEnterClank(string  asTimer)
{	
	if(asTimer == "Clank_1"){
		PlaySoundAtEntity("clanks","04_enter_clank.snt", "HorrorCreak_6", 0.5f, false);
		FadeImageTrailTo(1.5f, 2);
		
		AddTimer("Clank_2", 0.75f, "TimerEnterClank");
		AddTimer("Clank_3", 2.25f, "TimerEnterClank");
	}
	else if(asTimer == "Clank_2"){
		PlaySoundAtEntity("beath1","react_scare.snt", "Player", 0.75f, false);
		GiveSanityDamage(10, true);
	}	
	else if(asTimer == "Clank_3"){ 
		PlaySoundAtEntity("beath2","react_scare.snt", "Player", 1.25f, false);
		FadeImageTrailTo(0.0f, 2);
	}
	
	else if(asTimer == "Thunder"){
		PlaySoundAtEntity("thunder","general_thunder.snt", "Player", 0, false);
	}
	
}
//END ENTER LEVEL CLANKS//
//////////////////////////


public void EntityCallLamp(string  asEntity, string  type)
{
	AddLocalVarFloat("AmbLight", 0.1f);
	float fLight = GetLocalVarFloat("AmbLight");
	
	FadeLightTo("LightBurn_1", 0.15f+fLight, 0.125f+fLight, 0.1f+fLight, -1, -1, 1.0f);
}

////////////////////////////
// Run first time starting map
public override void OnStart()
{
	SetMapDisplayNameEntry("WineCellarLab");
	
	//----COLLIDE CALLBACKS----//
	AddEntityCollideCallback("Player", "EnterCreakOnArea", "FuncCreakHorrors", true, 1);
	AddEntityCollideCallback("Player", "AreaBreakStairs", "CollideBreakStairs", false, 1);
	AddEntityCollideCallback("Player", "AreaLookHoleActive", "CollideActiveHole", true, 1);
	AddEntityCollideCallback("Player", "AreaBeginSpider", "CollideActivateSpider", true, 1);
	AddEntityCollideCallback("Player", "AreaQuestStairs", "CollideQuestStairs", true, 1);
	AddEntityCollideCallback("Player", "AreaQuestStairs_2", "CollideQuestStairs02", true, 1);
	AddEntityCollideCallback("Player", "AreaScreamCave", "CollideScreamCave", true, 1);
	
	AddEntityCollideCallback("PlatformDynStickLift_1", "StickLift_2_Extra", "CollideStickLiftExtra", true, 1);
	AddEntityCollideCallback("PlatformDynStickLift_2", "StickLift_2_Extra", "CollideStickLiftExtra", true, 1);
	
	
	//----ENTITY INIT----//
	for(int i=1;i<=4;i++) {
		ConnectEntities("Part_"+i, "Valve_"+i, "Valve_"+i+"_AreaValve", false, 0, "InteractTurnValve"); 
		
		SetWheelStuckState("Valve_"+i, -1, false);
	}
	ConnectEntities("Ignite", "ValveIgnite", "Valve_1_AreaBottle", false, 0, "InteractTurnOnAcidMachine");
	
	SetEntityActive("chemical_container_static_1", false);
	SetEntityActive("chemical_container_2", false);
	
	SetWheelStuckState("ValveIgnite", -1, false);

	AddUseItemCallback("placebottle", "chemical_container_1", "AreaUseMachine", "ItemJarOnMachine", true);
	AddUseItemCallback("placebottle2", "chemical_container_1", "AreaPlaceJar", "ItemJarOnMachine", true);
	AddUseItemCallback("placebottle3", "chemical_container_1", "ValveIgnite", "ItemJarOnMachine", true);
	
	for(int i=1;i<=4;i++) {
		AddUseItemCallback("placebottlev"+i, "chemical_container_1", "Valve_"+i, "ItemJarOnMachine", true);
		AddUseItemCallback("Chemicalvi_"+i, "Chemical_"+i, "ValveIgnite", "AddChemical", true);
		AddUseItemCallback("Chemicala_"+i, "Chemical_"+i, "AreaUseMachine", "AddChemical", true);
		AddUseItemCallback("Chemicalm_"+i, "Chemical_"+i, "acid_machine_1", "AddChemical", true);
		
		for(int j=1;j<=4;j++)
			AddUseItemCallback("Chemicalv_"+i, "Chemical_"+i, "Valve_"+j, "AddChemical", true);
	}
	
	SetEntityPlayerInteractCallback("ValveIgnite", "InteractAcidMachine", false);
	for(int i=1;i<=4;i++) SetEntityPlayerInteractCallback("Valve_"+i, "InteractAcidMachine", false);	
	
	SetEntityPlayerLookAtCallback("AreaLookHole", "LookAtHole", true);
	
	BlockHint("EntityWheel");
	
	//----QUEST INIT----//
	SetNumberOfQuestsInMap(1);
	
	FuncMoanHorrors("", "AreaBeginChild", 1);

	SetGlobalVarInt("BeenToLab", 1);
	
	if(ScriptDebugOn())
	{
		GiveItemFromFile("lantern", "lantern.ent");
		GiveItemFromFile("Chemical_1", "flask01_aqua_regia.ent");
		GiveItemFromFile("Chemical_2", "flask01_calamine.ent");
		GiveItemFromFile("Chemical_3", "flask01_cuprite.ent");
		GiveItemFromFile("Chemical_4", "flask01_orpiment.ent");
		
		for(int i=0;i<10;i++) GiveItemFromFile("tinderbox_"+i, "tinderbox.ent");
		
		//SetWheelStuckState("ValveIgnite", 0, false);	//Normally stuck until all 4 chemicals in machine
	}
	
	SetFogProperties(4, 16, 1, true);
	SetFogColor(0.12f, 0.14f, 0.18f, 0.7f);
	SetFogActive(true);
}


////////////////////////////
// Run when entering map
public override void OnEnter()
{
    Finder.Bufferize("moanTimer", "FuncMoanTimer", "childTimer", "Begin moaning sounds", "moanSound", "HorrorMoan_0", "HorrorMoan_1", "HorrorMoan_2", "HorrorMoan_3", "HorrorMoan_4", "HorrorMoan_5", "HorrorMoan_6", "HorrorMoan_7", "HorrorMoan_8", "HorrorMoan_9", "HorrorMoan_10", "HorrorMoan_11", "HorrorMoan_12", "HorrorMoan_13", "HorrorMoan_14", "HorrorMoan_15", "HorrorMoan_16", "HorrorMoan_17", "HorrorMoan_18", "HorrorMoan_19", "HorrorMoan_20", "HorrorMoan_21", "HorrorMoan_22", "HorrorMoan_23", "HorrorMoan_24", "HorrorMoan_25", "HorrorMoan_26", "HorrorMoan_27", "HorrorMoan_28", "HorrorMoan_29", "HorrorMoan_30", "HorrorMoan_31", "HorrorMoan_32", "HorrorMoan_33", "HorrorMoan_34", "HorrorMoan_35", "HorrorMoan_36", "HorrorMoan_37", "HorrorMoan_38", "HorrorMoan_39", "HorrorMoan_40", "HorrorMoan_41", "HorrorMoan_42", "HorrorMoan_43", "HorrorMoan_44", "HorrorMoan_45", "HorrorMoan_46", "HorrorMoan_47", "HorrorMoan_48", "HorrorMoan_49", "HorrorMoan_50", "HorrorMoan_51", "HorrorMoan_52", "HorrorMoan_53", "HorrorMoan_54", "HorrorMoan_55", "HorrorMoan_56", "HorrorMoan_57", "HorrorMoan_58", "HorrorMoan_59", "HorrorMoan_60", "HorrorMoan_61", "HorrorMoan_62", "HorrorMoan_63", "HorrorMoan_64", "HorrorMoan_65", "HorrorMoan_66", "HorrorMoan_67", "HorrorMoan_68", "HorrorMoan_69", "HorrorMoan_70", "HorrorMoan_71", "HorrorMoan_72", "HorrorMoan_73", "HorrorMoan_74", "HorrorMoan_75", "HorrorMoan_76", "HorrorMoan_77", "HorrorMoan_78", "HorrorMoan_79", "HorrorMoan_80", "HorrorMoan_81", "HorrorMoan_82", "HorrorMoan_83", "HorrorMoan_84", "HorrorMoan_85", "HorrorMoan_86", "HorrorMoan_87", "HorrorMoan_88", "HorrorMoan_89", "HorrorMoan_90", "HorrorMoan_91", "HorrorMoan_92", "HorrorMoan_93", "HorrorMoan_94", "HorrorMoan_95", "HorrorMoan_96", "HorrorMoan_97", "HorrorMoan_98", "HorrorMoan_99", "childSound", "Now moaning in area: ", "EnterCreakOnArea", "creak", "FuncCreakTimer", "Player", "EnterCreakOffArea", "FuncCreakHorrors", "Player in/out creak: ", "creakPS", "HorrorCreak_0", "HorrorCreak_1", "HorrorCreak_2", "HorrorCreak_3", "HorrorCreak_4", "HorrorCreak_5", "HorrorCreak_6", "HorrorCreak_7", "HorrorCreak_8", "HorrorCreak_9", "HorrorCreak_10", "HorrorCreak_11", "HorrorCreak_12", "HorrorCreak_13", "HorrorCreak_14", "HorrorCreak_15", "HorrorCreak_16", "HorrorCreak_17", "HorrorCreak_18", "HorrorCreak_19", "HorrorCreak_20", "HorrorCreak_21", "HorrorCreak_22", "HorrorCreak_23", "HorrorCreak_24", "HorrorCreak_25", "HorrorCreak_26", "HorrorCreak_27", "HorrorCreak_28", "HorrorCreak_29", "HorrorCreak_30", "HorrorCreak_31", "HorrorCreak_32", "HorrorCreak_33", "HorrorCreak_34", "HorrorCreak_35", "HorrorCreak_36", "HorrorCreak_37", "HorrorCreak_38", "HorrorCreak_39", "HorrorCreak_40", "HorrorCreak_41", "HorrorCreak_42", "HorrorCreak_43", "HorrorCreak_44", "HorrorCreak_45", "HorrorCreak_46", "HorrorCreak_47", "HorrorCreak_48", "HorrorCreak_49", "HorrorCreak_50", "HorrorCreak_51", "HorrorCreak_52", "HorrorCreak_53", "HorrorCreak_54", "HorrorCreak_55", "HorrorCreak_56", "HorrorCreak_57", "HorrorCreak_58", "HorrorCreak_59", "HorrorCreak_60", "HorrorCreak_61", "HorrorCreak_62", "HorrorCreak_63", "HorrorCreak_64", "HorrorCreak_65", "HorrorCreak_66", "HorrorCreak_67", "HorrorCreak_68", "HorrorCreak_69", "HorrorCreak_70", "HorrorCreak_71", "HorrorCreak_72", "HorrorCreak_73", "HorrorCreak_74", "HorrorCreak_75", "HorrorCreak_76", "HorrorCreak_77", "HorrorCreak_78", "HorrorCreak_79", "HorrorCreak_80", "HorrorCreak_81", "HorrorCreak_82", "HorrorCreak_83", "HorrorCreak_84", "HorrorCreak_85", "HorrorCreak_86", "HorrorCreak_87", "HorrorCreak_88", "HorrorCreak_89", "HorrorCreak_90", "HorrorCreak_91", "HorrorCreak_92", "HorrorCreak_93", "HorrorCreak_94", "HorrorCreak_95", "HorrorCreak_96", "HorrorCreak_97", "HorrorCreak_98", "HorrorCreak_99", "creakSound", "Now creaking in area: ", " Next creak in: ", "key_study_1", "StairsWentDownTheDrain", "cave_in_1", "cavedx_*", "AreaQuestStairs", "monster_break", "AreaCaveMonster", "stairsPS", "AreaStairsParticle", "rocks2", "stairs_wood_1", "sanity", "TimerSanity", "sanity2", "sanity3", "sanity4", "sanity5", "PlatformDynStickLift_1", "GiveStackHint", "PlatformDynStickLift_2", "StickLift_1", "rockbreath", "torch_static01_16", "torch_static01_13", "torch_static01_12", "torch_static01_14", "stairsS", "react_scare", "rocks", "rocks3", "react_breath", "stackhint", "Hints", "StickyArea", "Name: ", "StickOneTime", "_Body_1", "TimerChangeEntity", "PlatformDynStickLift_1_Body_1", "PlatformDynStickLift_2_Body_1", "StickLift_2", "LadderArea_2", "Platform", "04Stairs", "AreaQuestStairs_2", "chemical_container_static_1", "PlaceJar", "puzzle_place_jar", "AreaCompleteSuccess", "ChemicalsInMachine", "JarEmpty", "Jar", "Sound", "Light", "04FindChemicals", "EntityWheel", "Valve_0", "Valve_1", "Valve_2", "Valve_3", "Valve_4", "Valve_5", "Valve_6", "Valve_7", "Valve_8", "Valve_9", "Valve_10", "Valve_11", "Valve_12", "Valve_13", "Valve_14", "Valve_15", "Valve_16", "Valve_17", "Valve_18", "Valve_19", "Valve_20", "Valve_21", "Valve_22", "Valve_23", "Valve_24", "Valve_25", "Valve_26", "Valve_27", "Valve_28", "Valve_29", "Valve_30", "Valve_31", "Valve_32", "Valve_33", "Valve_34", "Valve_35", "Valve_36", "Valve_37", "Valve_38", "Valve_39", "Valve_40", "Valve_41", "Valve_42", "Valve_43", "Valve_44", "Valve_45", "Valve_46", "Valve_47", "Valve_48", "Valve_49", "Valve_50", "Valve_51", "Valve_52", "Valve_53", "Valve_54", "Valve_55", "Valve_56", "Valve_57", "Valve_58", "Valve_59", "Valve_60", "Valve_61", "Valve_62", "Valve_63", "Valve_64", "Valve_65", "Valve_66", "Valve_67", "Valve_68", "Valve_69", "Valve_70", "Valve_71", "Valve_72", "Valve_73", "Valve_74", "Valve_75", "Valve_76", "Valve_77", "Valve_78", "Valve_79", "Valve_80", "Valve_81", "Valve_82", "Valve_83", "Valve_84", "Valve_85", "Valve_86", "Valve_87", "Valve_88", "Valve_89", "Valve_90", "Valve_91", "Valve_92", "Valve_93", "Valve_94", "Valve_95", "Valve_96", "Valve_97", "Valve_98", "Valve_99", "InteractAcidMachine02", "DoBigFeet", "Ch01Level04", "InteractAcidMachineNoChem", "ValveIgnite", "ValveIgniteFirst", "Ch01Level03", "MachineNeedsToStart", "InteractAcidMachine", "Connect ", " and ", " state:", "Fire", "_AreaBottle", "_Light", "LightBurn", "IgniteSound", "general_fire_burning_low", "Valve_1_AreaBottle", "ValvesNrOn", "ValvesNrOn: ", " And Feet ", "FinalBoil", "Valve_4_AreaValve", "FinalSteam", "FinalFlow", "LightAcid", "done", "TimerAcidDone", "Feet_1", "TimerBigFeet", "bang", "biggus", "release", "TimerReleaseValves", "RoadToSuccess", "PSteam", "SBoil", "SSteam", "Rate of success ", " is turned to ", "Part4PSteam", "AcidDone", "puzzle_acid", "chemical_container_2", "AcidSuccess", "puzzle_acid_success", "AcidFail", "puzzle_acid_fail", "Ch03Level26", "NoContainerBelowSqueezer", "Thunder", "TimerEnterClank", "04ChemicalsMoved", "AreaLookHole", "TimerLookHole", "HoleCry", "04_hole_squeal", "AreaHoleEffects", "HoleWater", "HolePS1", "breath4", "HoleWater2", "HoleWater3", "HolePS2", "HoleWater4", "breath1", "HoleWater5", "HolePS3", "HoleWater6", "HoleWater7", "breath3", "HoleWater8", "HoleWater9", "HoleWater10", "breath2", "AreaBeginSpider_1", "In Area 1", "RockBreath", "TimerRockBreath", "RockShake", "monster_breath", "breathps", "ps_cave_monster_breath", "Value: ", "monster_scream", "monster1", "TimerRockMonster", "monster2", "monster3", "monster4", "", "react_creath", "scream1", "TimerCaveScream", "scream2", "scream3", "scream4", "Feet_2", "Feet_3", "Feet_4", "Feet_5", "sigh", "step", "Area", "Clank_1", "clanks", "Clank_2", "Clank_3", "beath1", "beath2", "thunder", "AmbLight", "LightBurn_1", "WineCellarLab", "AreaBreakStairs", "CollideBreakStairs", "AreaLookHoleActive", "CollideActiveHole", "AreaBeginSpider", "CollideActivateSpider", "CollideQuestStairs", "CollideQuestStairs02", "AreaScreamCave", "CollideScreamCave", "StickLift_2_Extra", "CollideStickLiftExtra", "Part_0", "Part_1", "Part_2", "Part_3", "Part_4", "Part_5", "Part_6", "Part_7", "Part_8", "Part_9", "Part_10", "Part_11", "Part_12", "Part_13", "Part_14", "Part_15", "Part_16", "Part_17", "Part_18", "Part_19", "Part_20", "Part_21", "Part_22", "Part_23", "Part_24", "Part_25", "Part_26", "Part_27", "Part_28", "Part_29", "Part_30", "Part_31", "Part_32", "Part_33", "Part_34", "Part_35", "Part_36", "Part_37", "Part_38", "Part_39", "Part_40", "Part_41", "Part_42", "Part_43", "Part_44", "Part_45", "Part_46", "Part_47", "Part_48", "Part_49", "Part_50", "Part_51", "Part_52", "Part_53", "Part_54", "Part_55", "Part_56", "Part_57", "Part_58", "Part_59", "Part_60", "Part_61", "Part_62", "Part_63", "Part_64", "Part_65", "Part_66", "Part_67", "Part_68", "Part_69", "Part_70", "Part_71", "Part_72", "Part_73", "Part_74", "Part_75", "Part_76", "Part_77", "Part_78", "Part_79", "Part_80", "Part_81", "Part_82", "Part_83", "Part_84", "Part_85", "Part_86", "Part_87", "Part_88", "Part_89", "Part_90", "Part_91", "Part_92", "Part_93", "Part_94", "Part_95", "Part_96", "Part_97", "Part_98", "Part_99", "_AreaValve", "InteractTurnValve", "Ignite", "InteractTurnOnAcidMachine", "placebottle", "chemical_container_1", "AreaUseMachine", "ItemJarOnMachine", "placebottle2", "AreaPlaceJar", "placebottle3", "placebottlev", "Chemicalvi_0", "Chemicalvi_1", "Chemicalvi_2", "Chemicalvi_3", "Chemicalvi_4", "Chemicalvi_5", "Chemicalvi_6", "Chemicalvi_7", "Chemicalvi_8", "Chemicalvi_9", "Chemicalvi_10", "Chemicalvi_11", "Chemicalvi_12", "Chemicalvi_13", "Chemicalvi_14", "Chemicalvi_15", "Chemicalvi_16", "Chemicalvi_17", "Chemicalvi_18", "Chemicalvi_19", "Chemicalvi_20", "Chemicalvi_21", "Chemicalvi_22", "Chemicalvi_23", "Chemicalvi_24", "Chemicalvi_25", "Chemicalvi_26", "Chemicalvi_27", "Chemicalvi_28", "Chemicalvi_29", "Chemicalvi_30", "Chemicalvi_31", "Chemicalvi_32", "Chemicalvi_33", "Chemicalvi_34", "Chemicalvi_35", "Chemicalvi_36", "Chemicalvi_37", "Chemicalvi_38", "Chemicalvi_39", "Chemicalvi_40", "Chemicalvi_41", "Chemicalvi_42", "Chemicalvi_43", "Chemicalvi_44", "Chemicalvi_45", "Chemicalvi_46", "Chemicalvi_47", "Chemicalvi_48", "Chemicalvi_49", "Chemicalvi_50", "Chemicalvi_51", "Chemicalvi_52", "Chemicalvi_53", "Chemicalvi_54", "Chemicalvi_55", "Chemicalvi_56", "Chemicalvi_57", "Chemicalvi_58", "Chemicalvi_59", "Chemicalvi_60", "Chemicalvi_61", "Chemicalvi_62", "Chemicalvi_63", "Chemicalvi_64", "Chemicalvi_65", "Chemicalvi_66", "Chemicalvi_67", "Chemicalvi_68", "Chemicalvi_69", "Chemicalvi_70", "Chemicalvi_71", "Chemicalvi_72", "Chemicalvi_73", "Chemicalvi_74", "Chemicalvi_75", "Chemicalvi_76", "Chemicalvi_77", "Chemicalvi_78", "Chemicalvi_79", "Chemicalvi_80", "Chemicalvi_81", "Chemicalvi_82", "Chemicalvi_83", "Chemicalvi_84", "Chemicalvi_85", "Chemicalvi_86", "Chemicalvi_87", "Chemicalvi_88", "Chemicalvi_89", "Chemicalvi_90", "Chemicalvi_91", "Chemicalvi_92", "Chemicalvi_93", "Chemicalvi_94", "Chemicalvi_95", "Chemicalvi_96", "Chemicalvi_97", "Chemicalvi_98", "Chemicalvi_99", "Chemical_0", "Chemical_1", "Chemical_2", "Chemical_3", "Chemical_4", "Chemical_5", "Chemical_6", "Chemical_7", "Chemical_8", "Chemical_9", "Chemical_10", "Chemical_11", "Chemical_12", "Chemical_13", "Chemical_14", "Chemical_15", "Chemical_16", "Chemical_17", "Chemical_18", "Chemical_19", "Chemical_20", "Chemical_21", "Chemical_22", "Chemical_23", "Chemical_24", "Chemical_25", "Chemical_26", "Chemical_27", "Chemical_28", "Chemical_29", "Chemical_30", "Chemical_31", "Chemical_32", "Chemical_33", "Chemical_34", "Chemical_35", "Chemical_36", "Chemical_37", "Chemical_38", "Chemical_39", "Chemical_40", "Chemical_41", "Chemical_42", "Chemical_43", "Chemical_44", "Chemical_45", "Chemical_46", "Chemical_47", "Chemical_48", "Chemical_49", "Chemical_50", "Chemical_51", "Chemical_52", "Chemical_53", "Chemical_54", "Chemical_55", "Chemical_56", "Chemical_57", "Chemical_58", "Chemical_59", "Chemical_60", "Chemical_61", "Chemical_62", "Chemical_63", "Chemical_64", "Chemical_65", "Chemical_66", "Chemical_67", "Chemical_68", "Chemical_69", "Chemical_70", "Chemical_71", "Chemical_72", "Chemical_73", "Chemical_74", "Chemical_75", "Chemical_76", "Chemical_77", "Chemical_78", "Chemical_79", "Chemical_80", "Chemical_81", "Chemical_82", "Chemical_83", "Chemical_84", "Chemical_85", "Chemical_86", "Chemical_87", "Chemical_88", "Chemical_89", "Chemical_90", "Chemical_91", "Chemical_92", "Chemical_93", "Chemical_94", "Chemical_95", "Chemical_96", "Chemical_97", "Chemical_98", "Chemical_99", "AddChemical", "Chemicala_0", "Chemicala_1", "Chemicala_2", "Chemicala_3", "Chemicala_4", "Chemicala_5", "Chemicala_6", "Chemicala_7", "Chemicala_8", "Chemicala_9", "Chemicala_10", "Chemicala_11", "Chemicala_12", "Chemicala_13", "Chemicala_14", "Chemicala_15", "Chemicala_16", "Chemicala_17", "Chemicala_18", "Chemicala_19", "Chemicala_20", "Chemicala_21", "Chemicala_22", "Chemicala_23", "Chemicala_24", "Chemicala_25", "Chemicala_26", "Chemicala_27", "Chemicala_28", "Chemicala_29", "Chemicala_30", "Chemicala_31", "Chemicala_32", "Chemicala_33", "Chemicala_34", "Chemicala_35", "Chemicala_36", "Chemicala_37", "Chemicala_38", "Chemicala_39", "Chemicala_40", "Chemicala_41", "Chemicala_42", "Chemicala_43", "Chemicala_44", "Chemicala_45", "Chemicala_46", "Chemicala_47", "Chemicala_48", "Chemicala_49", "Chemicala_50", "Chemicala_51", "Chemicala_52", "Chemicala_53", "Chemicala_54", "Chemicala_55", "Chemicala_56", "Chemicala_57", "Chemicala_58", "Chemicala_59", "Chemicala_60", "Chemicala_61", "Chemicala_62", "Chemicala_63", "Chemicala_64", "Chemicala_65", "Chemicala_66", "Chemicala_67", "Chemicala_68", "Chemicala_69", "Chemicala_70", "Chemicala_71", "Chemicala_72", "Chemicala_73", "Chemicala_74", "Chemicala_75", "Chemicala_76", "Chemicala_77", "Chemicala_78", "Chemicala_79", "Chemicala_80", "Chemicala_81", "Chemicala_82", "Chemicala_83", "Chemicala_84", "Chemicala_85", "Chemicala_86", "Chemicala_87", "Chemicala_88", "Chemicala_89", "Chemicala_90", "Chemicala_91", "Chemicala_92", "Chemicala_93", "Chemicala_94", "Chemicala_95", "Chemicala_96", "Chemicala_97", "Chemicala_98", "Chemicala_99", "Chemicalm_0", "Chemicalm_1", "Chemicalm_2", "Chemicalm_3", "Chemicalm_4", "Chemicalm_5", "Chemicalm_6", "Chemicalm_7", "Chemicalm_8", "Chemicalm_9", "Chemicalm_10", "Chemicalm_11", "Chemicalm_12", "Chemicalm_13", "Chemicalm_14", "Chemicalm_15", "Chemicalm_16", "Chemicalm_17", "Chemicalm_18", "Chemicalm_19", "Chemicalm_20", "Chemicalm_21", "Chemicalm_22", "Chemicalm_23", "Chemicalm_24", "Chemicalm_25", "Chemicalm_26", "Chemicalm_27", "Chemicalm_28", "Chemicalm_29", "Chemicalm_30", "Chemicalm_31", "Chemicalm_32", "Chemicalm_33", "Chemicalm_34", "Chemicalm_35", "Chemicalm_36", "Chemicalm_37", "Chemicalm_38", "Chemicalm_39", "Chemicalm_40", "Chemicalm_41", "Chemicalm_42", "Chemicalm_43", "Chemicalm_44", "Chemicalm_45", "Chemicalm_46", "Chemicalm_47", "Chemicalm_48", "Chemicalm_49", "Chemicalm_50", "Chemicalm_51", "Chemicalm_52", "Chemicalm_53", "Chemicalm_54", "Chemicalm_55", "Chemicalm_56", "Chemicalm_57", "Chemicalm_58", "Chemicalm_59", "Chemicalm_60", "Chemicalm_61", "Chemicalm_62", "Chemicalm_63", "Chemicalm_64", "Chemicalm_65", "Chemicalm_66", "Chemicalm_67", "Chemicalm_68", "Chemicalm_69", "Chemicalm_70", "Chemicalm_71", "Chemicalm_72", "Chemicalm_73", "Chemicalm_74", "Chemicalm_75", "Chemicalm_76", "Chemicalm_77", "Chemicalm_78", "Chemicalm_79", "Chemicalm_80", "Chemicalm_81", "Chemicalm_82", "Chemicalm_83", "Chemicalm_84", "Chemicalm_85", "Chemicalm_86", "Chemicalm_87", "Chemicalm_88", "Chemicalm_89", "Chemicalm_90", "Chemicalm_91", "Chemicalm_92", "Chemicalm_93", "Chemicalm_94", "Chemicalm_95", "Chemicalm_96", "Chemicalm_97", "Chemicalm_98", "Chemicalm_99", "acid_machine_1", "Chemicalv_0", "Chemicalv_1", "Chemicalv_2", "Chemicalv_3", "Chemicalv_4", "Chemicalv_5", "Chemicalv_6", "Chemicalv_7", "Chemicalv_8", "Chemicalv_9", "Chemicalv_10", "Chemicalv_11", "Chemicalv_12", "Chemicalv_13", "Chemicalv_14", "Chemicalv_15", "Chemicalv_16", "Chemicalv_17", "Chemicalv_18", "Chemicalv_19", "Chemicalv_20", "Chemicalv_21", "Chemicalv_22", "Chemicalv_23", "Chemicalv_24", "Chemicalv_25", "Chemicalv_26", "Chemicalv_27", "Chemicalv_28", "Chemicalv_29", "Chemicalv_30", "Chemicalv_31", "Chemicalv_32", "Chemicalv_33", "Chemicalv_34", "Chemicalv_35", "Chemicalv_36", "Chemicalv_37", "Chemicalv_38", "Chemicalv_39", "Chemicalv_40", "Chemicalv_41", "Chemicalv_42", "Chemicalv_43", "Chemicalv_44", "Chemicalv_45", "Chemicalv_46", "Chemicalv_47", "Chemicalv_48", "Chemicalv_49", "Chemicalv_50", "Chemicalv_51", "Chemicalv_52", "Chemicalv_53", "Chemicalv_54", "Chemicalv_55", "Chemicalv_56", "Chemicalv_57", "Chemicalv_58", "Chemicalv_59", "Chemicalv_60", "Chemicalv_61", "Chemicalv_62", "Chemicalv_63", "Chemicalv_64", "Chemicalv_65", "Chemicalv_66", "Chemicalv_67", "Chemicalv_68", "Chemicalv_69", "Chemicalv_70", "Chemicalv_71", "Chemicalv_72", "Chemicalv_73", "Chemicalv_74", "Chemicalv_75", "Chemicalv_76", "Chemicalv_77", "Chemicalv_78", "Chemicalv_79", "Chemicalv_80", "Chemicalv_81", "Chemicalv_82", "Chemicalv_83", "Chemicalv_84", "Chemicalv_85", "Chemicalv_86", "Chemicalv_87", "Chemicalv_88", "Chemicalv_89", "Chemicalv_90", "Chemicalv_91", "Chemicalv_92", "Chemicalv_93", "Chemicalv_94", "Chemicalv_95", "Chemicalv_96", "Chemicalv_97", "Chemicalv_98", "Chemicalv_99", "LookAtHole", "AreaBeginChild", "BeenToLab", "lantern", "tinderbox_0", "tinderbox_1", "tinderbox_2", "tinderbox_3", "tinderbox_4", "tinderbox_5", "tinderbox_6", "tinderbox_7", "tinderbox_8", "tinderbox_9", "tinderbox_10", "tinderbox_11", "tinderbox_12", "tinderbox_13", "tinderbox_14", "tinderbox_15", "tinderbox_16", "tinderbox_17", "tinderbox_18", "tinderbox_19", "tinderbox_20", "tinderbox_21", "tinderbox_22", "tinderbox_23", "tinderbox_24", "tinderbox_25", "tinderbox_26", "tinderbox_27", "tinderbox_28", "tinderbox_29", "tinderbox_30", "tinderbox_31", "tinderbox_32", "tinderbox_33", "tinderbox_34", "tinderbox_35", "tinderbox_36", "tinderbox_37", "tinderbox_38", "tinderbox_39", "tinderbox_40", "tinderbox_41", "tinderbox_42", "tinderbox_43", "tinderbox_44", "tinderbox_45", "tinderbox_46", "tinderbox_47", "tinderbox_48", "tinderbox_49", "tinderbox_50", "tinderbox_51", "tinderbox_52", "tinderbox_53", "tinderbox_54", "tinderbox_55", "tinderbox_56", "tinderbox_57", "tinderbox_58", "tinderbox_59", "tinderbox_60", "tinderbox_61", "tinderbox_62", "tinderbox_63", "tinderbox_64", "tinderbox_65", "tinderbox_66", "tinderbox_67", "tinderbox_68", "tinderbox_69", "tinderbox_70", "tinderbox_71", "tinderbox_72", "tinderbox_73", "tinderbox_74", "tinderbox_75", "tinderbox_76", "tinderbox_77", "tinderbox_78", "tinderbox_79", "tinderbox_80", "tinderbox_81", "tinderbox_82", "tinderbox_83", "tinderbox_84", "tinderbox_85", "tinderbox_86", "tinderbox_87", "tinderbox_88", "tinderbox_89", "tinderbox_90", "tinderbox_91", "tinderbox_92", "tinderbox_93", "tinderbox_94", "tinderbox_95", "tinderbox_96", "tinderbox_97", "tinderbox_98", "tinderbox_99", "18_amb", "scare_male_terrified", "scare_baby_cry", "scare_wood_creak_mix", "break_stairs", "puzzle_add_chemical", "general_thunder", "04_big_feet", "puzzle_boil", "puzzle_gas", "puzzle_boil_low", "04_water_puff", "01_tiny2", "spider_die", "04_enter_clank", "ps_dust_falling_small_thin", "ps_dust_break_stair", "ps_acid_machine_bubble_large02", "ps_acid_machine_bubble_end", "ps_acid_machine_bubble_large", "ps_acid_machine_bubble_small", "waterlurker_walk_splash", "ps_dust_falling_big_feet", "PlayIntro");
    FakeDatabase.FindMusic("18_amb");
    FakeDatabase.FindMusic("04_event_hole.ogg");
    FakeDatabase.FindMusic("04_puzzle_acid.ogg");
    FakeDatabase.FindMusic("04_event_stairs.ogg");	
	//----PRELOADING----//
	PreloadSound("scare_male_terrified"); PreloadSound("scare_baby_cry"); PreloadSound("scare_wood_creak_mix"); PreloadSound("break_stairs"); 
	PreloadSound("puzzle_place_jar"); PreloadSound("puzzle_add_chemical"); PreloadSound("general_fire_burning_low"); PreloadSound("general_thunder"); 
	PreloadSound("04_big_feet"); PreloadSound("puzzle_boil"); PreloadSound("puzzle_gas"); PreloadSound("puzzle_boil_low"); 
	PreloadSound("puzzle_acid"); PreloadSound("puzzle_acid_success"); PreloadSound("puzzle_acid_fail"); PreloadSound("04_hole_squeal"); 
	PreloadSound("04_water_puff"); PreloadSound("react_breath"); PreloadSound("01_tiny2"); PreloadSound("spider_die"); 
	PreloadSound("04_enter_clank"); PreloadSound("react_scare");

	PreloadParticleSystem("ps_dust_falling_small_thin"); PreloadParticleSystem("ps_dust_break_stair"); PreloadParticleSystem("ps_acid_machine_bubble_large02");
	PreloadParticleSystem("ps_acid_machine_bubble_end"); PreloadParticleSystem("ps_acid_machine_bubble_large"); PreloadParticleSystem("ps_acid_machine_bubble_small"); 
	PreloadParticleSystem("waterlurker_walk_splash"); PreloadParticleSystem("ps_dust_falling_big_feet"); PreloadParticleSystem("ps_dust_falling_small_thin"); 
	 
	SetWheelStuckState("ValveIgnite", -1, false);
	 
	//----AUDIO----//
	PlayMusic("18_amb", true, 1, 5, 0, true);
		
	//---- TIMER INIT ----//
	if(GetLocalVarInt("PlayIntro") == 0){
		SetLocalVarInt("PlayIntro", 1);
		
		AutoSave();
		
		AddTimer("Clank_1", 0.25f, "TimerEnterClank");
	} 
	else AutoSave();
}

////////////////////////////
// Run when leaving map
public override void OnLeave()
{

}
}
