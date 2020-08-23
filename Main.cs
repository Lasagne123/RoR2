using JetBrains.Annotations;
using RoR2;
using RoR2.UI;
using RoR2.UI.MainMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace AROR2 {
    public class Main : MonoBehaviour {
		public PlayerCharacterMasterController Pcontroller;
		public CharacterMotor Cmotor;
		public CharacterBody Cbody;
		public CharacterMaster Cmaster;
		public TeamManager Tmanager;
		public PurchaseInteraction Pinteraction;
		public UnityEngine.Object[] Pinterobj;
	    	public UnityEngine.Object[] Tinterobj; 
        int itemindex = 0;
        public void Start() {
			GetObject();
		}
		public void GetObject()
        {
			//---------------------------------------------------------------------\\
			Pcontroller = FindObjectOfType<PlayerCharacterMasterController>();
			Cbody = FindObjectOfType<CharacterBody>();
			Cmaster = FindObjectOfType<CharacterMaster>();
			Pinteraction = FindObjectOfType<PurchaseInteraction>();
			Tmanager = FindObjectOfType<TeamManager>();
			Cmotor = FindObjectOfType<CharacterMotor>();
			Pinterobj = UnityEngine.Object.FindObjectsOfType(typeof(PurchaseInteraction));
			Tinterobj = UnityEngine.Object.FindObjectsOfType(typeof(TeleporterInteraction));
			//---------------------------------------------------------------------\\
		}
        private void Update(){
			if (Pcontroller)
			{
				if (Input.GetKey(KeyCode.H))
				{
					Tmanager.GiveTeamMoney(TeamIndex.Player, 1000);
					Tmanager.GiveTeamExperience(TeamIndex.Player, 1000);
					if (Input.GetKeyDown(KeyCode.P))
					{
						Cmaster.RespawnExtraLife();
					}
				}
				if (Input.GetKey(KeyCode.G))
				{
					EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/BrittleDeath"), new EffectData
					{
						origin = this.Cbody.aimOrigin,
						rotation = UnityEngine.Random.rotation,
						scale = 10f
					}, true);
				}
				if (Input.GetKey(KeyCode.LeftAlt))
				{
					this.Cbody.baseMoveSpeed += 5f;
				}
				if (Input.GetKey(KeyCode.X))
				{
					this.Cbody.baseMoveSpeed = 7f;
				}
				if (Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					Run.instance.AdvanceStage(Run.instance.nextStageScene);
				}
				if (Input.GetKey(KeyCode.LeftBracket))
				{
					TeleporterInteraction.instance.shouldAttemptToSpawnGoldshoresPortal = true;
					TeleporterInteraction.instance.shouldAttemptToSpawnShopPortal = true;
					TeleporterInteraction.instance.shouldAttemptToSpawnMSPortal = true;
				}
				if (Input.GetKey(KeyCode.Mouse4))
				{
					this.Cbody.baseAttackSpeed += 2f;
				}
				if (Input.GetKey(KeyCode.V))
				{
					this.Cbody.baseAttackSpeed = 2f;
				}
				if (Input.GetKey(KeyCode.J))
				{
					this.Cbody.baseJumpCount++;
				}
				if (Input.GetKey(KeyCode.K))
				{
					if (this.Cbody.baseJumpCount == 1)
					{
						this.Cbody.baseJumpCount = 1;
						return;
					}
					this.Cbody.baseJumpCount--;
				}
				if (Input.GetKey(KeyCode.N))
				{
					this.Cbody.baseJumpPower += 1f;
				}
				if (Input.GetKey(KeyCode.M))
				{
					if (this.Cbody.baseJumpPower <= 1f)
					{
						this.Cbody.baseJumpPower = 1f;
						return;
					}
					this.Cbody.baseJumpPower -= 1f;
				}
				if (Input.GetKeyUp(KeyCode.Q))
				{
					Cmotor.mass = 0f;
				}
				if (Input.GetKeyUp(KeyCode.I))
				{
					PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex((EquipmentIndex)this.i), this.Cbody.transform.position + Vector3.up * 1.5f, Vector3.up * 20f + this.Cbody.transform.forward * 2f);
					PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex((ItemIndex)this.i), this.Cbody.transform.position + Vector3.up * 1.5f, Vector3.up * 20f + this.Cbody.transform.forward * 2f);
					if (itemindex > 32)
						itemindex = 0;
					this.itemindex++;
				}
				if (Input.GetKey(KeyCode.O))
				{
					typeof(HoldoutZoneController).GetProperty("charge").SetValue(TeleporterInteraction.instance.holdoutZoneController, 1f);
				}
				if (Input.GetKey(KeyCode.E))
				{
					PlayerCharacterMasterController cachedMasterController = LocalUserManager.GetFirstLocalUser().cachedMasterController;
					if (cachedMasterController)
					{
						CharacterBody characterCbody = cachedMasterController.master.GetBody();
						if (characterCbody)
						{
							InputBankTest component = characterCbody.GetComponent<InputBankTest>();
							Ray ray = new Ray(component.aimOrigin, component.aimDirection);
							BullseyeSearch bullseyeSearch = new BullseyeSearch();
							TeamComponent component2 = characterCbody.GetComponent<TeamComponent>();
							bullseyeSearch.teamMaskFilter = TeamMask.all;
							bullseyeSearch.teamMaskFilter.RemoveTeam(component2.teamIndex);
							bullseyeSearch.filterByLoS = true;
							bullseyeSearch.searchOrigin = ray.origin;
							bullseyeSearch.searchDirection = ray.direction;
							bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
							bullseyeSearch.maxDistanceFilter = float.MaxValue;
							bullseyeSearch.maxAngleFilter = 179.9f;
							bullseyeSearch.RefreshCandidates();
							HurtBox hurtBox = Enumerable.FirstOrDefault<HurtBox>(bullseyeSearch.GetResults());
							if (hurtBox)
							{
								Vector3 aimDirection2 = hurtBox.transform.position - ray.origin;
								component.aimDirection = aimDirection2;
							}
						}
					}
				}
			}
		}
        public void OnGUI()
		{
			GUI.color = Color.white;
			GUI.Label(new Rect(10f, 100f, 150f, 20f), this.Cbody.GetUserName());
			GUI.Label(new Rect(10f, 115f, 150f, 20f), "AttackSpeed: " + this.Cbody.baseAttackSpeed.ToString());
			GUI.Label(new Rect(10f, 130f, 150f, 20f), "KillCount: " + this.Cbody.killCount.ToString());
			GUI.Label(new Rect(10f, 145f, 150f, 20f), "MoveSpeed: " + this.Cbody.baseMoveSpeed.ToString());
			GUI.Label(new Rect(10f, 160f, 150f, 20f), "DoubleJumps: " + this.Cbody.baseJumpCount.ToString());
			GUI.Label(new Rect(10f, 175f, 150f, 20f), "JumpStrength: " + this.Cbody.baseJumpPower.ToString());
			GUI.Label(new Rect(10f, 190f, 150f, 20f), "TelporterPercentage: " + TeleporterInteraction.instance.holdoutZoneController.displayChargePercent.ToString());
			GUI.Label(new Rect(10f, 220f, 100f, 20f), "Show Binds (Z)");
			if (Input.GetKey(KeyCode.Z))
			{
				GUI.color = Color.white;
				GUI.Label(new Rect(750f, 360f, 400f, 400f), "Give Money/Lunar Coins (H)\n" +
					"Increase Speed       (LALT)\n" +
					"Reset Speed                  (X)\n" +
					"Skip Level                     (ENTER)\n" +
					"Spawn Portals               (LB)\n" +
					"Increase Attack Speed   (M4)\n" +
					"Reset Attack Speed       (V)\n" +
					"Increase Double-Jumps  (J)\n" +
					"Decrease Double-Jumps (K)\n" +
					"Increase Jump Power     (N)\n" +
					"Decrease Jump Power    (M)\n" +
					"Spawn Items                  (I)\n" +
					"Effect Spammer             (G)\n" +
					"Aimbot On Key               (E)\n");
			}
			esp();
		}
		public void esp()
        {
			foreach (PurchaseInteraction Pinteraction in Pinterobj)
			{
				if (Pinteraction.available)
				{
					Vector3 vector = Camera.main.WorldToScreenPoint(Pinteraction.transform.position);
					if ((double)vector.z > 0.01)
					{
						GUI.color = Color.cyan;
						string displayName = Pinteraction.GetDisplayName();
						GUI.Label(new Rect(vector.x, (float)Screen.height - vector.y, 100f, 50f), displayName);
					}
				}
			}
			foreach (TeleporterInteraction teleporterInteraction in Tinterobj)
			{
				Vector3 vector2 = Camera.main.WorldToScreenPoint(teleporterInteraction.transform.position);
				if ((double)vector2.z > 0.01)
				{
					GUI.color = Color.green;
					GUI.Label(new Rect(vector2.x, (float)Screen.height - vector2.y, 100f, 50f), "Teleporter");
				}
			}
		}
	}
}
