using UnityEngine;
using System.Collections;

public class Scenario_intro : Scenario {
private void Start() {}

public override void OnEnter()
{
	PlayMusic("game_menu.ogg", true, 1, 0, 0, false);
}
}
