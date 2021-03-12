using UnityEngine;
using System.Collections;

public class Scenario_01_old_archives : Scenario {
private void Start() {}
/////////////////////////////////
//BEGIN OPENING DOORS WITH DUST//
/*Some dusts flows out and door bangs open 
 */
public void CollideOpeningDustDoor(string  asParent, string  asChild, int alState)
{
	StopRandomLook();	//De-activate the spinning head
	
	CreateParticleSystemAtEntity("PSDoor_1", "ps_dust_paper_blow.ps", "AreaDoor_1", false);
	CreateParticleSystemAtEntity("PSDoor_2", "ps_dust_push.ps", "AreaDoor_1", false);
	
	PlaySoundAtEntity("SoundDoor_1", "scare_wind_reverse", "AreaDoor_1", 1.0f, false);
	PlaySoundAtEntity("creaking_door", "joint_door_move_special.snt", "Door_1", 1.0f / 2.5f, false);
	PlaySoundAtEntity("SoundBong", "scare_tingeling.snt", "Player", 0.0f, false);
	
	AddTimer("opendoor_1", 0.25f, "TimerOpenDoor01");
	AddTimer("lightsout", 1, "TimerOpenDoor01");
	AddTimer("stopeffect", 2, "TimerOpenDoor01");
	
	StartScreenShake(0.007f,2, 0.25f,1);
	
	FadePlayerFOVMulTo(1.5f, 0.5f);
	
	/*For CollideBeginSwirl
	 */
	PlaySoundAtEntity("SoundFeet1", "01_tiny1", "AreaBeginSwirl", 15.0f, false);
	
	/*DEBUG
	 */
	AddDebugMessage("The door at "+asChild+" opens with dust", true);
}
public void TimerOpenDoor01(string  asTimer)
{
	if(asTimer == "stopeffect"){
		FadePlayerFOVMulTo(1, 1);
		PlaySoundAtEntity("breath", "react_breath.snt", "Player", 1.0f / 0.75f, false);
		AddTimer("lookloop", 1, "TimerRandomLook");	//Re-activate the spinning head
		return;
	}
	
	if(asTimer == "lightsout"){
		for(int i=1;i<=4;i++) SetLampLit("torch_static01_"+i, false, true);
		for(int i=1;i<=4;i++) FadeLightTo("LightOff_"+i, 0, 0, 0, 0, 0, 1.5f);
		PlaySoundAtEntity("breath2", "react_breath.snt", "Player", 1.0f / 1, false);
		return;
	}
	
	PlaySoundAtEntity("Wind", "general_wind_whirl", "Player", 2, false);
	PlaySoundAtEntity("scare", "react_scare.snt", "Player", 0.75f, false);

	StopSound("creaking_door", 1.0f);
	
	PlayMusic("01_amb_darkness.ogg", true, 1.0f, 0, 0, true);
	
	CreateParticleSystemAtEntity("PSDoor_3", "ps_dust_push.ps", "AreaDoor_2", false);
	CreateParticleSystemAtEntity("PSDoor_4", "ps_dust_push.ps", "AreaDoor_3", false);
	
	SetSwingDoorClosed("Door_1", false, false);
	SetSwingDoorDisableAutoClose("Door_1", true);
	
	AddTimer("Door_1", 0.01f, "TimerSwingDoor");
	
	GiveSanityDamage(10, true);
}
public void TimerSwingDoor(string  asTimer)
{
	if(GetLocalVarInt("SwingDoor") == 10){
		SetLocalVarInt("SwingDoor", 0);
		return;
	}
	
	if(asTimer == "Door_1") AddPropForce(asTimer, 160.0f, 0, 0, "World"); 
	else AddPropForce(asTimer, -60.0f, 0, 0, "World"); 
	
	AddLocalVarInt("SwingDoor", 1);
	
	AddTimer(asTimer, 0.03f, "TimerSwingDoor");
	
	AddDebugMessage("Swing: "+GetLocalVarInt("SwingDoor"), false);
}
//END OPENING DOORS WITH DUST//
///////////////////////////////


////////////////////////////
//START CLOSING DOOR SCARE//
/*Player in area in abandoned storage and door closes + scare event
 */
public void CollideBeginSwirl(string  asParent, string  asChild, int alState)
{
	StopRandomLook();	//De-activate the spinning head
	
	//FadePlayerAspectMulTo(2, 0.05f);
	FadeImageTrailTo(1.7f,1.5f);
	
	//INCREASE THE INTENSITY AND VOLUME OF THE RUNNING FEET SOUND
	StartScreenShake(0.01f,1, 0.1f,0.5f);
	
	//if(!GetSwingDoorClosed("Door_1")) PlaySoundAtEntity("SoundCloseDoor_1", "scare_slam_door", "Door_1",0.0f, false);
	
	//SetSwingDoorLocked("Door_1", true, true);
	
	PlaySoundAtEntity("SoundFeet2", "01_tiny2", "AreaFeet_1", 3, false);
	//PlaySoundAtEntity("scare", "react_scare.snt", "Player", 0.5f, false);

	AddTimer("swirl", 0.5f, "TimerSwirlHorror");
}
public void TimerSwirlHorror(string  asTimer)
{
	AddLocalVarInt("VarSwirl", 1);	//What step to play in the event
	float fSpeedWhirl = 0.5f;	//The default time between steps in an event
	
	switch(GetLocalVarInt("VarSwirl")) {
		case 1:
			PlaySoundAtEntity("SoundFeet3", "01_tiny3", "AreaFeet_2", 3, false);
			//CreateParticleSystemAtEntity("PSSwirl", "ps_dust_ghost.ps", "Player", false);
			//PlaySoundAtEntity("SoundSwirl", "scare_whine_loop", "AreaBeginSwirl", 0.0f, false);
			PlaySoundAtEntity("SoundBong2", "scare_tingeling_rev2.snt", "Player", 0.0f, false);
			//PlaySoundAtEntity("breath2", "react_breath.snt", "Player", 0.3f, false);
			fSpeedWhirl = 3.0f;
		break;
		case 2:
			PlaySoundAtEntity("SoundFeet4", "01_tiny4", "AreaFeet_3", 1, false);
			//MovePlayerHeadPos(0, -0.5f, 0, 1.25f, 0.5f);
			//StartEffectFlash(0.15f, 0.05f, 0.15f);
			//PlaySoundAtEntity("SoundFlash1", "scare_thump_flash.snt", "Player", 0.0f, false);
			SetPropActiveAndFade("cockroach_*", true, 0.5f);
			//FadeLightTo("BoxLight_1",0.5f,0.25f,0,1,-1,0.01f);
		break;
		case 3:
			//FadeOut(0.75f);
			FadePlayerAspectMulTo(2, 0.05f);
			FadeImageTrailTo(1.7f,1.5f);
			PlaySoundAtEntity("scare", "react_scare.snt", "Player", 0.5f, false);
			PlayMusic("01_event_critters.ogg", false, 1, 0, 10, false);
			GiveSanityDamage(10, true);
			//FadeLightTo("BoxLight_1",0,0,0,0,-1,0.01f);
			//SetPropActiveAndFade("cockroach_*", false, 0.5f);
		break;
		case 4:
			//MovePlayerHeadPos(0, 0, 0, 1, 0.5f);
			//FadeIn(1);
			//FadeLightTo("BoxLight_1",0.75f,0.25f,0,1,-1,0.01f);
			//SetPropActiveAndFade("cockroach_*", true, 0.5f);
			fSpeedWhirl = 0.15f;
		break;
		case 5:
			PlaySoundAtEntity("scare", "react_scare.snt", "Player", 0.6f, false);
			//FadeLightTo("BoxLight_1",0,0,0,0,-1,0.01f);
			fSpeedWhirl = 2.5f;
		break;
		case 6:
			//FadeOut(0.8f);
			//MovePlayerHeadPos(0, -0.5f, 0, 1.0f, 0.5f);
			//StartEffectFlash(0.15f, 0.25f, 0.05f);
			//FadeLightTo("BoxLight_1",0.75f,0,0.15f,1,-1,0.01f);
			//PlaySoundAtEntity("SoundFlash1", "scare_thump_flash.snt", "Player", 0.0f, false);
		break;
		case 7:
			for(int i=1;i<=4;i++) StopSound("SoundFeet"+i, 2);
			//FadeIn(1);
			PlaySoundAtEntity("breath2", "react_breath.snt", "Player", 0.7f, false);
			//FadeLightTo("BoxLight_1",0,0,0,0,-1,0.01f);
			//StopSound("SoundSwirl", 1.5f);
			fSpeedWhirl = 1.0f;
		break;
		case 8:
			//MovePlayerHeadPos(0, 0, 0, 1, 0.5f);
			FadeImageTrailTo(1.6f,3.0f);
			FadePlayerAspectMulTo(2, 0.01f);
			FadeOut(20);
			StopMusic(10, 0);
			FadeGlobalSoundSpeed(0.4f, 10);
			FadeGlobalSoundVolume(0.1f, 10);
			StartScreenShake(0.02f,0.1f, 2,5);
			PlaySoundAtEntity("rumbleoo", "00_loop", "Player", 4.0f, false);
			PlaySoundAtEntity("scare3", "react_sigh.snt", "Player", 0.8f, false);
			AddTimer("breath", 1, "TimerBreath");	
			SetPlayerRunSpeedMul(0);
			SetPlayerMoveSpeedMul(0.75f);
			fSpeedWhirl = 1.0f;
		break;
		case 9:
			SetPlayerMoveSpeedMul(0.5f);
			//MovePlayerHeadPos(0, -1.3f, 0, 0.05f, 0.5f);
			//SetSwingDoorLocked("Door_1", false, true);
			PlaySoundAtEntity("faint", "00_faint", "Player", 8, false);
			fSpeedWhirl = 1.0f;
		break;
		case 10:
			SetPropActiveAndFade("cockroach_*", false, 1);
			PlaySoundAtEntity("scare3", "react_sigh.snt", "Player", 0.8f, false);
			SetPlayerMoveSpeedMul(0.75f);
			FadeIn(5);
			//MovePlayerHeadPos(0, 0, 0, 0.2f, 0.5f);
			FadeImageTrailTo(0,0.1f);
			FadePlayerAspectMulTo(1, 0.05f);
			FadeGlobalSoundSpeed(1, 5);
			FadeGlobalSoundVolume(1, 5);
			StopSound("rumbleoo", 5.0f);
			PlayMusic("01_amb_darkness.ogg", true, 1.0f, 0, 0, true);
			AddTimer("lookloop", 3, "TimerRandomLook");	
			RemoveTimer("breath");
			/* AddLocalVarInt("VarEventsDone", 1); //Makes sure the event to notice the player of a door opening is only played when no other main event occurs.
			if(GetLocalVarInt("VarEventsDone") == 3) DoDoorOpening(10); */
		break;
	}

	if(GetLocalVarInt("VarSwirl") < 10) AddTimer("swirl", fSpeedWhirl, "TimerSwirlHorror");
}
public void TimerBreath(string  asTimer)
{
	PlaySoundAtEntity("breath", "react_breath_no3d", "AreaDoor_1", 1, false);
	
	AddTimer("breath", 3, "TimerBreath");	
}
//END CLOSING DOOR SCARE//
//////////////////////////

/////////////////////
//BEGIN BEGIN GHOST//
/*In aban study, steps is seen and book thrown.
 */
public void CollideBeginGhost(string  asParent, string  asChild, int alState)
{
	StopRandomLook();	//De-activate the spinning head
	PlaySoundAtEntity("SoundScratch_5", "scare_scratch", "AreaGhostWind", 2, false);
	PlaySoundAtEntity("whirly", "01_whirl.snt", "Player", 4, false);
	//PlayMusic("01_event_dust.ogg", false, 1, 3, 10, false);
	CreateParticleSystemAtEntity("PSGhostWind", "ps_dust_whirl_large.ps", "AreaGhostWind", false);
	AddTimer("ghost", 0.2f, "TimerGhost");
	RemoveTimer("wall_scrape");
}
int iLoopGhost = 1;	//If a step should loop, 1 as it is used for selecting what area to begin footsteps in
public void TimerGhost(string  asTimer)
{
	AddLocalVarInt("VarGhost", 1);	//What step to play in the event
	float fSpeedGhost = 0.5f;	//The default time between steps in an event

	switch(GetLocalVarInt("VarGhost")) {
		case 1:
			FadeOut(20);
			PlaySoundAtEntity("SoundScratch_2", "scare_scratch_intense", "AreaScratch_1", 4, false);
		break;
		case 2:
			GiveSanityDamage(10, false);
			//StartPlayerLookAt("AreaDustBoom_2", 0.5f, 0.5f, "");
			PlaySoundAtEntity("SoundFear2", "scare_male_terrified.snt", "AreaGhostStep_1", 0, false);
			PlaySoundAtEntity("breath2", "react_breath.snt", "Player", 0.6f, false);
			PlaySoundAtEntity("DustBoom1", "scare_wall_stomp.snt", "AreaDustBoom_1", 0.5f, false);
			AddPropForce("ghostbook_1", 50, 100, 50, "world");
			CreateParticleSystemAtEntity("PSDustBoom1", "ps_dust_impact_vert.ps", "AreaDustBoom_1", false);
			StartScreenShake(0.01f,0.1f, 0.5f,0.9f);
		break;
		case 3:
			MovePlayerHeadPos(0, -1.3f, 0, 0.1f, 0.5f);
			PlaySoundAtEntity("scare2", "react_scare.snt", "Player", 0.4f, false);
			FadePlayerAspectMulTo(2, 0.02f);
			FadeImageTrailTo(1.7f,1.1f);
		break;
		case 4:
			//StartPlayerLookAt("AreaDustBoom_3", 0.5f, 0.5f, "");
			PlaySoundAtEntity("DustBoom2", "scare_wall_stomp.snt", "AreaDustBoom_2", 0.25f, false);
			PlaySoundAtEntity("SoundFear3x", "scare_male_terrified.snt", "AreaGhostStep_1", 1, false);
			AddPropForce("ghostbook_2", 50, 100, 70, "world");
			AddPropForce("ghostbook_3", 50, 100, 60, "world");
			CreateParticleSystemAtEntity("PSDustBoom2", "ps_dust_impact_vert.ps", "AreaDustBoom_2", false);
			StartScreenShake(0.02f,0.1f, 0.5f,0.9f);
			PlaySoundAtEntity("SoundScratch_3", "scare_scratch", "AreaScratch_2", 2, false);
		break;
		case 5:
			MovePlayerHeadPos(0, 0, 0, 0.1f, 0.5f);
			FadeIn(5);
			PlaySoundAtEntity("breath2", "react_breath.snt", "Player", 0.2f, false);
			FadeImageTrailTo(1.3f,1.1f);
			PlaySoundAtEntity("SoundGhostScream", "scare_ghost.snt", "AreaGhostStep_1", 0.0f, false);
		break;
		case 6:
			PlaySoundAtEntity("SoundFear4x", "scare_male_terrified5.snt", "AreaGhostStep_1", 1, false);
			PlaySoundAtEntity("breath4", "react_breath.snt", "Player", 0.3f, false);
			FadeImageTrailTo(1.6f,1.1f);
			fSpeedGhost = 1.5f;
		break;
		case 7:
			PlaySoundAtEntity("scare3", "react_scare.snt", "Player", 0.2f, false);
			PlaySoundAtEntity("DustBoom4", "scare_wall_stomp.snt", "AreaDustBoom_4", 0, false);
			CreateParticleSystemAtEntity("PSDustBoom4", "ps_dust_impact.ps", "AreaDustBoom_4", false);
			StartScreenShake(0.04f,0.1f, 0.5f,0.9f);
			AddPropForce("lantern", 20, 220, 40, "world");
			StartPlayerLookAt("lantern", 2, 2, "");
			FadeOut(15);
			MovePlayerHeadPos(0, -1.3f, 0, 0.05f, 0.5f);
			fSpeedGhost = 1.5f;
		break;
		case 8:
			//StartPlayerLookAt("AreaDustBoom_4", 0.5f, 0.5f, "");
			PlaySoundAtEntity("DustBoom3", "scare_wall_stomp.snt", "AreaDustBoom_3", 0.1f, false);
			CreateParticleSystemAtEntity("PSDustBoom3", "ps_dust_impact_vert.ps", "AreaDustBoom_3", false);
			AddPropForce("ghostbook_4", 30, 120, 70, "world");
			StartScreenShake(0.03f,0.1f, 0.5f,0.9f);
		break;
		case 9:
			StopPlayerLookAt();
			StartScreenShake(0.02f,0.1f, 2,2);
			AddTimer("wall_scrape", RandFloat(3.0f,15.0f), "TimerWallScrape");	
			PlaySoundAtEntity("breath3", "react_breath.snt", "Player", 0.5f, false);
			for(int i=1;i<=5;i++) StopSound("SoundScratch_"+i, 4.0f);
			StopSound("whirly", 4.0f);
			FadePlayerAspectMulTo(1, 0.02f);
			FadeImageTrailTo(0.0f,1.1f);
			StopMusic(8, 10);
			fSpeedGhost = 1.5f;
		break;
		case 10:
			FadeIn(5);
			MovePlayerHeadPos(0, 0, 0, 0.1f, 0.5f);
			PlaySoundAtEntity("breath3", "react_breath.snt", "Player", 0.7f, false);
			/* AddLocalVarInt("VarEventsDone", 1); //Makes sure the event to notice the player of a door opening is only played when no other main event occurs.
			if(GetLocalVarInt("VarEventsDone") == 3) DoDoorOpening(6); */
		break;
	}
	
	if(GetLocalVarInt("VarGhost") < 10)  AddTimer("ghost", fSpeedGhost, "TimerGhost");
}
//END BEGIN GHOST//
///////////////////


//////////////////////////
//BEGIN RANDOM LOOK SPIN//
/*Player has a bit of a random head during the level
 */
bool bRoll = true;
public void TimerRandomLook(string  asTimer)
{
	int iLook = RandFloat(1,5);
	
	if(iLook > 4){
		SetPlayerRunSpeedMul(0.5f);
		SetPlayerMoveSpeedMul(1);
		FadePlayerRollTo(0, 0.5f, 1); 
		FadePlayerFOVMulTo(1, 1);
		FadeImageTrailTo(0,1.5f);
		
		AddTimer("lookloop", RandFloat(6.0f,8.0f), "TimerRandomLook");
		return;
	}
	
	if(iLook == 1 || iLook == 3)
		PlaySoundAtEntity("sigh", "react_sigh.snt", "Player", 1.0f / 0.75f, false);
	
	FadePlayerFOVMulTo(RandFloat(0.8f,1.2f), RandFloat(0.06f,0.09f));
	
	SetPlayerMoveSpeedMul(RandFloat(0.75f,0.95f));
	SetPlayerRunSpeedMul(RandFloat(0.3f,0.5f));
	
	FadeImageTrailTo(RandFloat(0.4f,0.75f),RandFloat(0.75f,1.0f));
	
	bRoll= bRoll == false ? true : false;
	
	if(bRoll)
		FadePlayerRollTo(RandFloat(2,8), RandFloat(0.075f,0.35f), RandFloat(0.35f,0.85f)); 
	else
		FadePlayerRollTo(RandFloat(-2,-8), RandFloat(0.05f,0.25f), RandFloat(0.3f,0.75f)); 
		
	AddTimer("lookloop", RandFloat(3.0f,6.0f), "TimerRandomLook");
}
public void TimerRandomLook02(string  asTimer)
{
	int iLook = RandFloat(1,5);
	
	if(iLook > 4){
		SetPlayerRunSpeedMul(0.5f);
		SetPlayerMoveSpeedMul(1);
		FadePlayerRollTo(0, 0.5f, 1); 
		FadePlayerFOVMulTo(1, 1);
		FadeImageTrailTo(0,1.5f);
		
		AddTimer("lookloop2", RandFloat(6.0f,8.0f), "TimerRandomLook02");
		return;
	}
	
	if(iLook == 1 || iLook == 3)
		PlaySoundAtEntity("sigh", "react_sigh.snt", "Player", 1.0f / 0.5f, false);
	
	FadePlayerFOVMulTo(RandFloat(0.8f,1.2f), RandFloat(0.06f,0.09f));
	
	SetPlayerMoveSpeedMul(RandFloat(0.85f,1.00f));
	SetPlayerRunSpeedMul(RandFloat(0.5f,0.7f));
	
	FadeImageTrailTo(RandFloat(0.1f,0.4f),RandFloat(0.85f,1.0f));
	
	bRoll= bRoll == false ? true : false;
	
	if(bRoll)
		FadePlayerRollTo(RandFloat(1,4), RandFloat(0.075f,0.15f), RandFloat(0.35f,0.6f)); 
	else
		FadePlayerRollTo(RandFloat(-1,-4), RandFloat(0.05f,0.1f), RandFloat(0.3f,0.5f)); 
		
	AddTimer("lookloop2", RandFloat(3.0f,6.0f), "TimerRandomLook02");
}
public void StopRandomLook()
{
	RemoveTimer("lookloop");
	
	SetPlayerMoveSpeedMul(1);
	FadePlayerFOVMulTo(1, 0.5f);
	FadeImageTrailTo(0,1.0f);
	FadePlayerRollTo(0, 0.5f, 2); 
}
//END RANDOM LOOK SPING//
/////////////////////////

////////////////////////////
//BEGIN PICK LANTERN & OIL//
public void PickLanterAndOil(string  asEntity, string  asType)
{
	//AddLocalVarInt("VarPicked", 2);	
	
	//if(GetLocalVarInt("VarPicked") == 2){
		if(GetLocalVarInt("DoCreakPartOfOpenDoor") == 1)
				DoDoorOpening(1);
			else 
				DoSimpleDoorOpening();	//No creak || music || swing open on door.
				
		/* AddLocalVarInt("VarEventsDone", 1);	//Makes sure the event to notice the player of a door opening is only played when no other main event occurs.
		
		if(GetLocalVarInt("VarEventsDone") == 3) {
			
		}	 */
	//}	
	
	if(asEntity == "lantern")
		SetPlayerLampOil(25.0f);
		
	/*DEBUG
	 */
	AddDebugMessage("Picked "+asEntity, true);
}
public void DoSimpleDoorOpening()
{
	RemoveTimer("lookloop");
	AddTimer("lookloop2", 6, "TimerRandomLook02");
	//AddTimer("Door_3", 0.01f, "TimerSwingDoor");
	
	SetSwingDoorLocked("Door_3", false, false);
	//SetSwingDoorClosed("Door_3", false, false);
	//SetSwingDoorDisableAutoClose("Door_3", true);
	
	SetEntityPlayerInteractCallback("Door_3", "", true);
}
public void DoDoorOpening(int iDelay)
{
	RemoveTimer("lookloop");
	AddTimer("lookloop2", 6, "TimerRandomLook02");
	AddTimer("delayeffect", iDelay, "TimerDelayEffect");
		
	SetSwingDoorLocked("Door_3", false, false);
	SetSwingDoorClosed("Door_3", false, false);
	SetSwingDoorDisableAutoClose("Door_3", true);
	
	PlaySoundAtEntity("grunt", "01_idle.snt", "AreaGrunt", 0.0f, false);
	
	SetEntityPlayerInteractCallback("Door_3", "", true);
}
public void TimerDelayEffect(string  asTimer)
{
	PlayMusic("10_puzzle01.ogg", false, 0.7f, 2, 0, false);
	
	PlayGuiSound("unlock_door.snt", 0.5f);
	
	PlaySoundAtEntity("SoundDoorCreak", "01_door.snt", "Door_3", 0.0f, false);
	StartScreenShake(0.01f, 1, 0.5f,1);
	
	AddTimer("Door_3", 0.01f, "TimerSwingDoor");
}
//END PICK LANTERN & OIL//
//////////////////////////


////////////////////////
//BEGIN COMPLETE TRAIL//
public void CollideCompleteTrail(string  asParent, string  asChild, int alState)
{
	for(int i=1;i<=3;i++) SetPropActiveAndFade("cockroache_"+i, true, 0.5f);
	
	CompleteQuest("00Trail", "00Trail");
	
	PlayMusic("15_puzzle_hole.ogg", false, 1.0f, 0.5f, 5, false);
		
	RemoveTimer("lookloop2");
	
	StopRandomLook();
	
	StartPlayerLookAt("note_generic_1", 2.0f, 2.0f, "");
	
	AddTimer("stoplook", 3.0f, "TimerStopLook");
}
public void TimerStopLook(string  asTimer)
{
	StopPlayerLookAt();
}
//END COMPLETE TRAIL//
//////////////////////


///////////////////////
//BEGIN EFFECT TIMERS//
public void TimerWallScrape(string  asTimer)
{
	int iArea = RandFloat(1,6);
	
	PlaySoundAtEntity("wall_scrape"+iArea, "scare_wall_crawl_single.snt", "AreaWallScrape_"+iArea, 0.0f, false);
	
	AddTimer("wall_scrape", RandFloat(3.0f,15.0f), "TimerWallScrape");	
}
public void TimerBeginWind(string  asTimer)
{
	PlaySoundAtEntity("BeginWindSound", "general_wind_whirl.snt", "Player", 4.0f, false);
}
//END EFFECT TIMERS//
/////////////////////


///////////////////
//BEGIN LAST WIND//
public void CollideLastWind(string  asParent, string  asChild, int alState)
{
	//if(GetLocalVarInt("VarPicked") != 2) return;
	
	CreateParticleSystemAtEntity("PSLastWind", "ps_dust_push.ps", "AreaWindLast", false);
	CreateParticleSystemAtEntity("PSLastWind2", "ps_dust_paper_blow.ps", "AreaWindLast", false);
	PlaySoundAtEntity("LastWindSound", "general_wind_whirl.snt", "AreaWindLast", 3.0f, false);
}
//END LAST WIND//
/////////////////


//////////////////////////
//BEGIN OPEN SECRET DOOR//
public void PickNoteToSelf(string  entity, string  type)
{
	PlayMusic("01_paper_self.ogg", false, 0.7f, 0, 10, false);
	
	//AddTimer("Hint", 0.5f, "TimerHint");	//On door exit instead
}
public void TimerHint(string  asTimer)
{
	GiveSanityBoostSmall();
	GiveHint("sanity", "Hints", "SanityAdd", 0);	
}
public void CreateDust(string  asConnectionName, string  asMainEntity, string  asConnectEntity, int alState)
{
	//for(int i=1;i<10;i++) AddPropForce("jara_"+i, 1.0f, 1.0f, 1.0f, "world");
	//for(int i=1;i<4;i++) AddPropForce("jbok_"+i, 1.0f, 1.0f, 1.0f, "world");
	
	if(GetLocalVarInt("DoSecretDoorEffects") == 1) return;
	
	CreateParticleSystemAtEntity("PSDoorDust", "ps_dust_falling_door.ps", "secret_door", false);
	
	AddTimer("sucess_music", 2, "TimerMusic");	
	
	AddTimer("Hint", 0.5f, "TimerHint");
	
	StartScreenShake(0.006f, 0.5f, 2.0f,2.0f);
	
	SetLocalVarInt("DoSecretDoorEffects", 1);
}
public void TimerMusic(string  asTimer)
{
	PlayMusic("01_puzzle_passage.ogg", false, 0.7f, 0, 10, false);
}
//END OPEN SECRET DOOR//
////////////////////////


/////////////////////
//BEGIN MISC THINGS//
public void CollideHintLean(string  asParent, string  asChild, int alState)
{
	GiveHint("lean", "Hints", "LeanHint", 0);
}

public void CollideTremble(string  asParent, string  asChild, int alState)
{
	/*Turn off rain for the level
	 */
	StopSound("amb_2", 0.0f); StopSound("amb_3", 0.0f); StopSound("amb_4", 0.0f); StopSound("amb_8", 0.0f);
	DestroyParticleSystem("ParticleSystem_41"); DestroyParticleSystem("ParticleSystem_40");
	
	/*All below + Timer is for shake events when entering the second last room
	 */
	PlaySoundAtEntity("rumble3", "general_rock_rumble_no3d", "Player", 15.0f, false);
	PlaySoundAtEntity("breath", "react_breath.snt", "Player", 2.0f, false);
	MovePlayerHeadPos(0.5f, -0.5f, 0.5f, 0.1f, 0.01f);
	
	StartScreenShake(0.03f, 1.0f, 4.0f,8.0f);

	for(int i=1;i<6;i++) CreateParticleSystemAtEntity("dust"+i, "ps_dust_falling_door", "AreaTrembleFall_"+i, false); 
	
	AddPropForce("chandelier_simple_short_1", -75.0f, 0, 75.0f, "world");
	AddPropForce("chandelier_simple_short_4", 75.0f, 0, -75.0f, "world");
	
	AddTimer("stoprumb", 6.0f, "TimerStopRumble");
	AddTimer("force1", 1.0f, "TimerStopRumble");
	AddTimer("force", 2.0f, "TimerStopRumble");
	AddTimer("force", 3.0f, "TimerStopRumble");
	AddTimer("force", 4.0f, "TimerStopRumble");
}
public void TimerStopRumble(string  asTimer)
{
	if(asTimer == "force" || asTimer == "force1"){
		if(asTimer == "force1") FadeImageTrailTo(1.0f, 6.0f);
		
		int i = 150;
		
		AddPropForce("chandelier_simple_short_1", i, 0, -i, "world");
		AddPropForce("chandelier_simple_short_4", -i, 0, i, "world");
		
		if(i == -150) PlaySoundAtEntity("chain", "general_chain_rattle_single", "chandelier_simple_short_1", 0.5f, false);
		else PlaySoundAtEntity("chain", "general_chain_rattle_single", "chandelier_simple_short_4", 0.5f, false);
		
		AddDebugMessage("I is: "+i, false);
		
		return;
	}
	PlaySoundAtEntity("breath2", "react_breath.snt", "Player", 1.0f, false);
	MovePlayerHeadPos(0, 0, 0, 0.2f, 0.01f);
	FadeImageTrailTo(0.0f, 12.0f);
	StopSound("rumble3", 10.0f);
}
//END MISC THINGS//
///////////////////


//SEEN THE MAGIC OPENING DOOR
public void CollideAreaCollideSeenDoor(string  asParent, string  asChild, int alState)
{
	SetLocalVarInt("DoCreakPartOfOpenDoor", 1);
}

//Locked door message
public void InteractLockedDoor(string  asEntity)
{
	SetMessage("Ch01Level03", "DoorLocked", 0);
	
	PlayGuiSound("locked_door.snt", 0.8f);
	
	AddTimer(asEntity, 1.0f, "TimerDoorMessageOnAgain");
}

public void TimerDoorMessageOnAgain(string  asTimer)
{
	SetEntityPlayerInteractCallback(asTimer, "InteractLockedDoor", true);
}

////////////////////////////
// Run first time starting map
public override void OnStart()
{
	SetMapDisplayNameEntry("ArchivesOld");
	
	//----COLLIDE CALLBACKS----//
	AddEntityCollideCallback("Player", "AreaTriggerDoor_1", "CollideOpeningDustDoor", true, 1);	//Door that swings open 1st corridor
	AddEntityCollideCallback("Player", "AreaBeginSwirl", "CollideBeginSwirl", true, 1);			//Swril and horror in room behind swing door
	AddEntityCollideCallback("Player", "AreaTriggerGhost", "CollideBeginGhost", true, 1);		//The steps and throwing book
	AddEntityCollideCallback("Player", "AreaTriggerTrail", "CollideCompleteTrail", true, 1);		//Complete the follow trail quest
	AddEntityCollideCallback("Player", "AreaTriggerLastWind", "CollideLastWind", true, 1);	//A last wind puff at end of hall
	AddEntityCollideCallback("Player", "AreaHintLean", "CollideHintLean", true, 1);	//Enable a hint to help finding the lever part
	AddEntityCollideCallback("Player", "AreaCollideTremble", "CollideTremble", true, 1);
	AddEntityCollideCallback("Player", "AreaCollideSeenDoor", "CollideAreaCollideSeenDoor", true, 1);
	
	//----ENTITY INIT----//
	CreateParticleSystemAtEntity("BeginWindPS", "ps_dust_push.ps", "AreaBeginWind", false);
	
	//----ITEM CALLBACKS----//
	SetSwingDoorLocked("shelf_secret_door_joint_1", true, false);
	SetEntityInteractionDisabled("shelf_secret_door_joint_1", true);
	
	
	//----CONNECT CALLBACKS----//
	/*Secret hole in wall
	 */
		ConnectEntities("door_connection",		//Name of connection
		"secret_lever", 	//Parent entity (Affects)
		"secret_door",	//Child entity 	(Affected) 
		false,		//Invert the state sent
		1, 		//States used (0=both), checked before invertion.
		"CreateDust");	//callback	
		
		ConnectEntities("shelf_connection",		//Name of connection
		"secret_lever", 	//Parent entity (Affects)
		"secret_shelf",	//Child entity 	(Affected) 
		false,		//Invert the state sent
		1, 		//States used (0=both), checked before invertion.
		"");	//callback	
		
		//SetMoveObjectAngularOffsetArea("secret_shelf", "rotatearea");
	
	UnBlockHint("SanityAdd"); UnBlockHint("LanternNoItem"); UnBlockHint("LanternNoOil"); UnBlockHint("LeanHint");
	UnBlockHint("PushHint"); UnBlockHint("ThrowHint"); UnBlockHint("EntitySlide"); UnBlockHint("EntityLever"); 
	UnBlockHint("SanityHit");
	
	//----QUEST INIT----//
	
	SetNumberOfQuestsInMap(2);
	
	//----TIMER INIT----//
	AddTimer("lookloop", 0.1f, "TimerRandomLook");	//Re-activate the spinning head
	AddTimer("wall_scrape", 0.1f, "TimerWallScrape");	//Random scrapings in the walls
	AddTimer("BeginWindTimer", 0.75f, "TimerBeginWind");
	
	//----DEBUG----//
	if(ScriptDebugOn())
	{
		GiveItemFromFile("lantern", "lantern.ent");
		//SetPlayerLampOil(0);	//Is normally set in level00
	}
}

////////////////////////////
// Run when entering map
public override void OnEnter()
{
    Finder.Bufferize("PSDoor_1", "AreaDoor_1", "PSDoor_2", "SoundDoor_1", "scare_wind_reverse", "creaking_door", "Door_1", "SoundBong", "Player", "opendoor_1", "TimerOpenDoor01", "lightsout", "stopeffect", "SoundFeet1", "01_tiny1", "AreaBeginSwirl", "The door at ", " opens with dust", "breath", "lookloop", "TimerRandomLook", "torch_static01_0", "torch_static01_1", "torch_static01_2", "torch_static01_3", "torch_static01_4", "torch_static01_5", "torch_static01_6", "torch_static01_7", "torch_static01_8", "torch_static01_9", "torch_static01_10", "torch_static01_11", "torch_static01_12", "torch_static01_13", "torch_static01_14", "torch_static01_15", "torch_static01_16", "torch_static01_17", "torch_static01_18", "torch_static01_19", "torch_static01_20", "torch_static01_21", "torch_static01_22", "torch_static01_23", "torch_static01_24", "torch_static01_25", "torch_static01_26", "torch_static01_27", "torch_static01_28", "torch_static01_29", "torch_static01_30", "torch_static01_31", "torch_static01_32", "torch_static01_33", "torch_static01_34", "torch_static01_35", "torch_static01_36", "torch_static01_37", "torch_static01_38", "torch_static01_39", "torch_static01_40", "torch_static01_41", "torch_static01_42", "torch_static01_43", "torch_static01_44", "torch_static01_45", "torch_static01_46", "torch_static01_47", "torch_static01_48", "torch_static01_49", "torch_static01_50", "torch_static01_51", "torch_static01_52", "torch_static01_53", "torch_static01_54", "torch_static01_55", "torch_static01_56", "torch_static01_57", "torch_static01_58", "torch_static01_59", "torch_static01_60", "torch_static01_61", "torch_static01_62", "torch_static01_63", "torch_static01_64", "torch_static01_65", "torch_static01_66", "torch_static01_67", "torch_static01_68", "torch_static01_69", "torch_static01_70", "torch_static01_71", "torch_static01_72", "torch_static01_73", "torch_static01_74", "torch_static01_75", "torch_static01_76", "torch_static01_77", "torch_static01_78", "torch_static01_79", "torch_static01_80", "torch_static01_81", "torch_static01_82", "torch_static01_83", "torch_static01_84", "torch_static01_85", "torch_static01_86", "torch_static01_87", "torch_static01_88", "torch_static01_89", "torch_static01_90", "torch_static01_91", "torch_static01_92", "torch_static01_93", "torch_static01_94", "torch_static01_95", "torch_static01_96", "torch_static01_97", "torch_static01_98", "torch_static01_99", "LightOff_0", "LightOff_1", "LightOff_2", "LightOff_3", "LightOff_4", "LightOff_5", "LightOff_6", "LightOff_7", "LightOff_8", "LightOff_9", "LightOff_10", "LightOff_11", "LightOff_12", "LightOff_13", "LightOff_14", "LightOff_15", "LightOff_16", "LightOff_17", "LightOff_18", "LightOff_19", "LightOff_20", "LightOff_21", "LightOff_22", "LightOff_23", "LightOff_24", "LightOff_25", "LightOff_26", "LightOff_27", "LightOff_28", "LightOff_29", "LightOff_30", "LightOff_31", "LightOff_32", "LightOff_33", "LightOff_34", "LightOff_35", "LightOff_36", "LightOff_37", "LightOff_38", "LightOff_39", "LightOff_40", "LightOff_41", "LightOff_42", "LightOff_43", "LightOff_44", "LightOff_45", "LightOff_46", "LightOff_47", "LightOff_48", "LightOff_49", "LightOff_50", "LightOff_51", "LightOff_52", "LightOff_53", "LightOff_54", "LightOff_55", "LightOff_56", "LightOff_57", "LightOff_58", "LightOff_59", "LightOff_60", "LightOff_61", "LightOff_62", "LightOff_63", "LightOff_64", "LightOff_65", "LightOff_66", "LightOff_67", "LightOff_68", "LightOff_69", "LightOff_70", "LightOff_71", "LightOff_72", "LightOff_73", "LightOff_74", "LightOff_75", "LightOff_76", "LightOff_77", "LightOff_78", "LightOff_79", "LightOff_80", "LightOff_81", "LightOff_82", "LightOff_83", "LightOff_84", "LightOff_85", "LightOff_86", "LightOff_87", "LightOff_88", "LightOff_89", "LightOff_90", "LightOff_91", "LightOff_92", "LightOff_93", "LightOff_94", "LightOff_95", "LightOff_96", "LightOff_97", "LightOff_98", "LightOff_99", "breath2", "Wind", "general_wind_whirl", "scare", "PSDoor_3", "AreaDoor_2", "PSDoor_4", "AreaDoor_3", "TimerSwingDoor", "SwingDoor", "World", "Swing: ", "SoundCloseDoor_1", "scare_slam_door", "SoundFeet2", "01_tiny2", "AreaFeet_1", "swirl", "TimerSwirlHorror", "VarSwirl", "SoundFeet3", "01_tiny3", "AreaFeet_2", "PSSwirl", "SoundSwirl", "scare_whine_loop", "SoundBong2", "SoundFeet4", "01_tiny4", "AreaFeet_3", "SoundFlash1", "cockroach_*", "BoxLight_1", "SoundFeet", "rumbleoo", "00_loop", "scare3", "TimerBreath", "faint", "00_faint", "VarEventsDone", "react_breath_no3d", "SoundScratch_5", "scare_scratch", "AreaGhostWind", "whirly", "PSGhostWind", "ghost", "TimerGhost", "wall_scrape", "VarGhost", "SoundScratch_2", "scare_scratch_intense", "AreaScratch_1", "AreaDustBoom_2", "", "SoundFear2", "AreaGhostStep_1", "DustBoom1", "AreaDustBoom_1", "ghostbook_1", "world", "PSDustBoom1", "scare2", "AreaDustBoom_3", "DustBoom2", "SoundFear3x", "ghostbook_2", "ghostbook_3", "PSDustBoom2", "SoundScratch_3", "AreaScratch_2", "SoundGhostScream", "SoundFear4x", "breath4", "DustBoom4", "AreaDustBoom_4", "PSDustBoom4", "lantern", "DustBoom3", "PSDustBoom3", "ghostbook_4", "TimerWallScrape", "breath3", "SoundScratch_0", "SoundScratch_1", "SoundScratch_4", "SoundScratch_6", "SoundScratch_7", "SoundScratch_8", "SoundScratch_9", "SoundScratch_10", "SoundScratch_11", "SoundScratch_12", "SoundScratch_13", "SoundScratch_14", "SoundScratch_15", "SoundScratch_16", "SoundScratch_17", "SoundScratch_18", "SoundScratch_19", "SoundScratch_20", "SoundScratch_21", "SoundScratch_22", "SoundScratch_23", "SoundScratch_24", "SoundScratch_25", "SoundScratch_26", "SoundScratch_27", "SoundScratch_28", "SoundScratch_29", "SoundScratch_30", "SoundScratch_31", "SoundScratch_32", "SoundScratch_33", "SoundScratch_34", "SoundScratch_35", "SoundScratch_36", "SoundScratch_37", "SoundScratch_38", "SoundScratch_39", "SoundScratch_40", "SoundScratch_41", "SoundScratch_42", "SoundScratch_43", "SoundScratch_44", "SoundScratch_45", "SoundScratch_46", "SoundScratch_47", "SoundScratch_48", "SoundScratch_49", "SoundScratch_50", "SoundScratch_51", "SoundScratch_52", "SoundScratch_53", "SoundScratch_54", "SoundScratch_55", "SoundScratch_56", "SoundScratch_57", "SoundScratch_58", "SoundScratch_59", "SoundScratch_60", "SoundScratch_61", "SoundScratch_62", "SoundScratch_63", "SoundScratch_64", "SoundScratch_65", "SoundScratch_66", "SoundScratch_67", "SoundScratch_68", "SoundScratch_69", "SoundScratch_70", "SoundScratch_71", "SoundScratch_72", "SoundScratch_73", "SoundScratch_74", "SoundScratch_75", "SoundScratch_76", "SoundScratch_77", "SoundScratch_78", "SoundScratch_79", "SoundScratch_80", "SoundScratch_81", "SoundScratch_82", "SoundScratch_83", "SoundScratch_84", "SoundScratch_85", "SoundScratch_86", "SoundScratch_87", "SoundScratch_88", "SoundScratch_89", "SoundScratch_90", "SoundScratch_91", "SoundScratch_92", "SoundScratch_93", "SoundScratch_94", "SoundScratch_95", "SoundScratch_96", "SoundScratch_97", "SoundScratch_98", "SoundScratch_99", "sigh", "lookloop2", "TimerRandomLook02", "VarPicked", "DoCreakPartOfOpenDoor", "Picked ", "Door_3", "delayeffect", "TimerDelayEffect", "grunt", "AreaGrunt", "SoundDoorCreak", "cockroache_0", "cockroache_1", "cockroache_2", "cockroache_3", "cockroache_4", "cockroache_5", "cockroache_6", "cockroache_7", "cockroache_8", "cockroache_9", "cockroache_10", "cockroache_11", "cockroache_12", "cockroache_13", "cockroache_14", "cockroache_15", "cockroache_16", "cockroache_17", "cockroache_18", "cockroache_19", "cockroache_20", "cockroache_21", "cockroache_22", "cockroache_23", "cockroache_24", "cockroache_25", "cockroache_26", "cockroache_27", "cockroache_28", "cockroache_29", "cockroache_30", "cockroache_31", "cockroache_32", "cockroache_33", "cockroache_34", "cockroache_35", "cockroache_36", "cockroache_37", "cockroache_38", "cockroache_39", "cockroache_40", "cockroache_41", "cockroache_42", "cockroache_43", "cockroache_44", "cockroache_45", "cockroache_46", "cockroache_47", "cockroache_48", "cockroache_49", "cockroache_50", "cockroache_51", "cockroache_52", "cockroache_53", "cockroache_54", "cockroache_55", "cockroache_56", "cockroache_57", "cockroache_58", "cockroache_59", "cockroache_60", "cockroache_61", "cockroache_62", "cockroache_63", "cockroache_64", "cockroache_65", "cockroache_66", "cockroache_67", "cockroache_68", "cockroache_69", "cockroache_70", "cockroache_71", "cockroache_72", "cockroache_73", "cockroache_74", "cockroache_75", "cockroache_76", "cockroache_77", "cockroache_78", "cockroache_79", "cockroache_80", "cockroache_81", "cockroache_82", "cockroache_83", "cockroache_84", "cockroache_85", "cockroache_86", "cockroache_87", "cockroache_88", "cockroache_89", "cockroache_90", "cockroache_91", "cockroache_92", "cockroache_93", "cockroache_94", "cockroache_95", "cockroache_96", "cockroache_97", "cockroache_98", "cockroache_99", "00Trail", "note_generic_1", "stoplook", "TimerStopLook", "AreaWallScrape_0", "AreaWallScrape_1", "AreaWallScrape_2", "AreaWallScrape_3", "AreaWallScrape_4", "AreaWallScrape_5", "AreaWallScrape_6", "AreaWallScrape_7", "AreaWallScrape_8", "AreaWallScrape_9", "AreaWallScrape_10", "AreaWallScrape_11", "AreaWallScrape_12", "AreaWallScrape_13", "AreaWallScrape_14", "AreaWallScrape_15", "AreaWallScrape_16", "AreaWallScrape_17", "AreaWallScrape_18", "AreaWallScrape_19", "AreaWallScrape_20", "AreaWallScrape_21", "AreaWallScrape_22", "AreaWallScrape_23", "AreaWallScrape_24", "AreaWallScrape_25", "AreaWallScrape_26", "AreaWallScrape_27", "AreaWallScrape_28", "AreaWallScrape_29", "AreaWallScrape_30", "AreaWallScrape_31", "AreaWallScrape_32", "AreaWallScrape_33", "AreaWallScrape_34", "AreaWallScrape_35", "AreaWallScrape_36", "AreaWallScrape_37", "AreaWallScrape_38", "AreaWallScrape_39", "AreaWallScrape_40", "AreaWallScrape_41", "AreaWallScrape_42", "AreaWallScrape_43", "AreaWallScrape_44", "AreaWallScrape_45", "AreaWallScrape_46", "AreaWallScrape_47", "AreaWallScrape_48", "AreaWallScrape_49", "AreaWallScrape_50", "AreaWallScrape_51", "AreaWallScrape_52", "AreaWallScrape_53", "AreaWallScrape_54", "AreaWallScrape_55", "AreaWallScrape_56", "AreaWallScrape_57", "AreaWallScrape_58", "AreaWallScrape_59", "AreaWallScrape_60", "AreaWallScrape_61", "AreaWallScrape_62", "AreaWallScrape_63", "AreaWallScrape_64", "AreaWallScrape_65", "AreaWallScrape_66", "AreaWallScrape_67", "AreaWallScrape_68", "AreaWallScrape_69", "AreaWallScrape_70", "AreaWallScrape_71", "AreaWallScrape_72", "AreaWallScrape_73", "AreaWallScrape_74", "AreaWallScrape_75", "AreaWallScrape_76", "AreaWallScrape_77", "AreaWallScrape_78", "AreaWallScrape_79", "AreaWallScrape_80", "AreaWallScrape_81", "AreaWallScrape_82", "AreaWallScrape_83", "AreaWallScrape_84", "AreaWallScrape_85", "AreaWallScrape_86", "AreaWallScrape_87", "AreaWallScrape_88", "AreaWallScrape_89", "AreaWallScrape_90", "AreaWallScrape_91", "AreaWallScrape_92", "AreaWallScrape_93", "AreaWallScrape_94", "AreaWallScrape_95", "AreaWallScrape_96", "AreaWallScrape_97", "AreaWallScrape_98", "AreaWallScrape_99", "BeginWindSound", "PSLastWind", "AreaWindLast", "PSLastWind2", "LastWindSound", "Hint", "TimerHint", "sanity", "Hints", "SanityAdd", "jara_0", "jara_1", "jara_2", "jara_3", "jara_4", "jara_5", "jara_6", "jara_7", "jara_8", "jara_9", "jara_10", "jara_11", "jara_12", "jara_13", "jara_14", "jara_15", "jara_16", "jara_17", "jara_18", "jara_19", "jara_20", "jara_21", "jara_22", "jara_23", "jara_24", "jara_25", "jara_26", "jara_27", "jara_28", "jara_29", "jara_30", "jara_31", "jara_32", "jara_33", "jara_34", "jara_35", "jara_36", "jara_37", "jara_38", "jara_39", "jara_40", "jara_41", "jara_42", "jara_43", "jara_44", "jara_45", "jara_46", "jara_47", "jara_48", "jara_49", "jara_50", "jara_51", "jara_52", "jara_53", "jara_54", "jara_55", "jara_56", "jara_57", "jara_58", "jara_59", "jara_60", "jara_61", "jara_62", "jara_63", "jara_64", "jara_65", "jara_66", "jara_67", "jara_68", "jara_69", "jara_70", "jara_71", "jara_72", "jara_73", "jara_74", "jara_75", "jara_76", "jara_77", "jara_78", "jara_79", "jara_80", "jara_81", "jara_82", "jara_83", "jara_84", "jara_85", "jara_86", "jara_87", "jara_88", "jara_89", "jara_90", "jara_91", "jara_92", "jara_93", "jara_94", "jara_95", "jara_96", "jara_97", "jara_98", "jara_99", "jbok_0", "jbok_1", "jbok_2", "jbok_3", "jbok_4", "jbok_5", "jbok_6", "jbok_7", "jbok_8", "jbok_9", "jbok_10", "jbok_11", "jbok_12", "jbok_13", "jbok_14", "jbok_15", "jbok_16", "jbok_17", "jbok_18", "jbok_19", "jbok_20", "jbok_21", "jbok_22", "jbok_23", "jbok_24", "jbok_25", "jbok_26", "jbok_27", "jbok_28", "jbok_29", "jbok_30", "jbok_31", "jbok_32", "jbok_33", "jbok_34", "jbok_35", "jbok_36", "jbok_37", "jbok_38", "jbok_39", "jbok_40", "jbok_41", "jbok_42", "jbok_43", "jbok_44", "jbok_45", "jbok_46", "jbok_47", "jbok_48", "jbok_49", "jbok_50", "jbok_51", "jbok_52", "jbok_53", "jbok_54", "jbok_55", "jbok_56", "jbok_57", "jbok_58", "jbok_59", "jbok_60", "jbok_61", "jbok_62", "jbok_63", "jbok_64", "jbok_65", "jbok_66", "jbok_67", "jbok_68", "jbok_69", "jbok_70", "jbok_71", "jbok_72", "jbok_73", "jbok_74", "jbok_75", "jbok_76", "jbok_77", "jbok_78", "jbok_79", "jbok_80", "jbok_81", "jbok_82", "jbok_83", "jbok_84", "jbok_85", "jbok_86", "jbok_87", "jbok_88", "jbok_89", "jbok_90", "jbok_91", "jbok_92", "jbok_93", "jbok_94", "jbok_95", "jbok_96", "jbok_97", "jbok_98", "jbok_99", "DoSecretDoorEffects", "PSDoorDust", "secret_door", "sucess_music", "TimerMusic", "lean", "LeanHint", "amb_2", "amb_3", "amb_4", "amb_8", "ParticleSystem_41", "ParticleSystem_40", "rumble3", "general_rock_rumble_no3d", "dust", "ps_dust_falling_door", "AreaTrembleFall_0", "AreaTrembleFall_1", "AreaTrembleFall_2", "AreaTrembleFall_3", "AreaTrembleFall_4", "AreaTrembleFall_5", "AreaTrembleFall_6", "AreaTrembleFall_7", "AreaTrembleFall_8", "AreaTrembleFall_9", "AreaTrembleFall_10", "AreaTrembleFall_11", "AreaTrembleFall_12", "AreaTrembleFall_13", "AreaTrembleFall_14", "AreaTrembleFall_15", "AreaTrembleFall_16", "AreaTrembleFall_17", "AreaTrembleFall_18", "AreaTrembleFall_19", "AreaTrembleFall_20", "AreaTrembleFall_21", "AreaTrembleFall_22", "AreaTrembleFall_23", "AreaTrembleFall_24", "AreaTrembleFall_25", "AreaTrembleFall_26", "AreaTrembleFall_27", "AreaTrembleFall_28", "AreaTrembleFall_29", "AreaTrembleFall_30", "AreaTrembleFall_31", "AreaTrembleFall_32", "AreaTrembleFall_33", "AreaTrembleFall_34", "AreaTrembleFall_35", "AreaTrembleFall_36", "AreaTrembleFall_37", "AreaTrembleFall_38", "AreaTrembleFall_39", "AreaTrembleFall_40", "AreaTrembleFall_41", "AreaTrembleFall_42", "AreaTrembleFall_43", "AreaTrembleFall_44", "AreaTrembleFall_45", "AreaTrembleFall_46", "AreaTrembleFall_47", "AreaTrembleFall_48", "AreaTrembleFall_49", "AreaTrembleFall_50", "AreaTrembleFall_51", "AreaTrembleFall_52", "AreaTrembleFall_53", "AreaTrembleFall_54", "AreaTrembleFall_55", "AreaTrembleFall_56", "AreaTrembleFall_57", "AreaTrembleFall_58", "AreaTrembleFall_59", "AreaTrembleFall_60", "AreaTrembleFall_61", "AreaTrembleFall_62", "AreaTrembleFall_63", "AreaTrembleFall_64", "AreaTrembleFall_65", "AreaTrembleFall_66", "AreaTrembleFall_67", "AreaTrembleFall_68", "AreaTrembleFall_69", "AreaTrembleFall_70", "AreaTrembleFall_71", "AreaTrembleFall_72", "AreaTrembleFall_73", "AreaTrembleFall_74", "AreaTrembleFall_75", "AreaTrembleFall_76", "AreaTrembleFall_77", "AreaTrembleFall_78", "AreaTrembleFall_79", "AreaTrembleFall_80", "AreaTrembleFall_81", "AreaTrembleFall_82", "AreaTrembleFall_83", "AreaTrembleFall_84", "AreaTrembleFall_85", "AreaTrembleFall_86", "AreaTrembleFall_87", "AreaTrembleFall_88", "AreaTrembleFall_89", "AreaTrembleFall_90", "AreaTrembleFall_91", "AreaTrembleFall_92", "AreaTrembleFall_93", "AreaTrembleFall_94", "AreaTrembleFall_95", "AreaTrembleFall_96", "AreaTrembleFall_97", "AreaTrembleFall_98", "AreaTrembleFall_99", "chandelier_simple_short_1", "chandelier_simple_short_4", "stoprumb", "TimerStopRumble", "force1", "force", "chain", "general_chain_rattle_single", "I is: ", "Ch01Level03", "DoorLocked", "TimerDoorMessageOnAgain", "InteractLockedDoor", "ArchivesOld", "AreaTriggerDoor_1", "CollideOpeningDustDoor", "CollideBeginSwirl", "AreaTriggerGhost", "CollideBeginGhost", "AreaTriggerTrail", "CollideCompleteTrail", "AreaTriggerLastWind", "CollideLastWind", "AreaHintLean", "CollideHintLean", "AreaCollideTremble", "CollideTremble", "AreaCollideSeenDoor", "CollideAreaCollideSeenDoor", "BeginWindPS", "AreaBeginWind", "shelf_secret_door_joint_1", "door_connection", "secret_lever", "CreateDust", "shelf_connection", "secret_shelf", "rotatearea", "LanternNoItem", "LanternNoOil", "PushHint", "ThrowHint", "EntitySlide", "EntityLever", "SanityHit", "BeginWindTimer", "TimerBeginWind", "joint_door_move_special", "scare_tingeling", "react_breath", "react_scare", "scare_tingeling_rev2", "scare_thump_flash", "react_sigh", "01_whirl", "scare_male_terrified", "scare_wall_stomp", "scare_ghost", "01_door", "scare_wall_crawl_single", "ps_dust_paper_blow", "ps_dust_push", "ps_dust_ghost", "ps_dust_whirl_large", "ps_dust_impact_vert", "ps_dust_impact");
    FakeDatabase.FindMusic("01_puzzle_passage.ogg");
    FakeDatabase.FindMusic("01_paper_self.ogg");
    FakeDatabase.FindMusic("15_puzzle_hole.ogg");
    FakeDatabase.FindMusic("10_puzzle01.ogg");
    FakeDatabase.FindMusic("01_event_dust.ogg");
    FakeDatabase.FindMusic("01_event_critters.ogg");
    FakeDatabase.FindMusic("01_amb_darkness.ogg");	
	//----PRELOADING----//
	PreloadSound("scare_wind_reverse"); PreloadSound("joint_door_move_special"); PreloadSound("scare_tingeling"); PreloadSound("01_tiny1"); 
	PreloadSound("react_breath"); PreloadSound("general_wind_whirl"); PreloadSound("react_scare"); PreloadSound("scare_slam_door"); 
	PreloadSound("01_tiny2"); PreloadSound("01_tiny3"); PreloadSound("scare_whine_loop"); PreloadSound("scare_tingeling_rev2"); 
	PreloadSound("01_tiny4"); PreloadSound("scare_thump_flash"); PreloadSound("00_loop"); PreloadSound("react_sigh"); 
	PreloadSound("00_faint"); PreloadSound("react_breath_no3d"); PreloadSound("scare_scratch"); PreloadSound("01_whirl"); 
	PreloadSound("scare_scratch_intense"); PreloadSound("scare_male_terrified"); PreloadSound("scare_wall_stomp"); PreloadSound("scare_ghost"); 
	PreloadSound("01_door"); PreloadSound("scare_wall_crawl_single"); PreloadSound("general_rock_rumble_no3d");

	PreloadParticleSystem("ps_dust_paper_blow"); PreloadParticleSystem("ps_dust_push"); PreloadParticleSystem("ps_dust_ghost");
	PreloadParticleSystem("ps_dust_whirl_large"); PreloadParticleSystem("ps_dust_impact_vert"); PreloadParticleSystem("ps_dust_impact");
	PreloadParticleSystem("ps_dust_falling_door");
	
	ClearSavedMaps();
	
	//----AUDIO----//
	AutoSave();
}

////////////////////////////
// Run when leaving map
public override void OnLeave()
{
	
}
}
