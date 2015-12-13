using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using ICE;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Styles;
using ICE.Layouts;
using ICE.Creatures.EditorInfos;


namespace ICE.Creatures.EditorHandler
{
	
	public static class EditorStatus
	{	
		public static void Print( ICECreatureControl _control ){
			
			if( ! _control.Display.ShowStatus )
				return;
			
			ICEEditorStyle.SplitterByIndent( 0 );
			ICEEditorLayout.BeginHorizontal();
				_control.Display.FoldoutStatus = ICEEditorLayout.Foldout( _control.Display.FoldoutStatus, "Status" );
				if (GUILayout.Button("SAVE", ICEEditorStyle.ButtonMiddle ))
					CreatureIO.SaveStatusToFile( _control.Creature.Status, _control.gameObject.name );
				if (GUILayout.Button("LOAD", ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Status = CreatureIO.LoadStatusFromFile( _control.Creature.Status );				
				if (GUILayout.Button("RESET", ICEEditorStyle.ButtonMiddle ))
					ResetValues( _control );
			ICEEditorLayout.EndHorizontal( Info.STATUS );
			
			if( _control.Display.FoldoutStatus ) 
			{
				EditorGUILayout.Separator();
				
				DrawStatus( _control );
				DrawStatusBasics( _control );
				DrawStatusAdvanced( _control );
				DrawStatusInfluences( _control );
	
				EditorGUILayout.Separator();
			}
		}

		private static void ResetValues( ICECreatureControl _control )
		{				
			_control.Creature.Status.DamageInPercent = 0.0f;
			_control.Creature.Status.StressInPercent = 0.0f;
			
			_control.Creature.Status.DefaultAggressivity = 0;
			_control.Creature.Status.AggressivityDamageMultiplier  = 0.5f;
			_control.Creature.Status.AggressivityStressMultiplier = 0.5f;
			_control.Creature.Status.AggressivityHealthMultiplier = 0.8f;
			_control.Creature.Status.AggressivityStaminaMultiplier = 0.2f;
			_control.Creature.Status.AggressivityHungerMultiplier = 0.1f;
			_control.Creature.Status.AggressivityThirstMultiplier = 0.1f;
			_control.Creature.Status.AggressivityAgeMultiplier = 0.1f;
			_control.Creature.Status.AggressivityTemperaturMultiplier = 0.2f;
			
			_control.Creature.Status.DefaultHealth = 100;
			_control.Creature.Status.HealthDamageMultiplier = 0.8f;
			_control.Creature.Status.HealthStressMultiplier = 0.2f;
			_control.Creature.Status.HealthHungerMultiplier = 0.2f;
			_control.Creature.Status.HealthThirstMultiplier = 0.3f;
			_control.Creature.Status.HealthAgeMultiplier = 0.1f;
			_control.Creature.Status.HealthTemperaturMultiplier = 0.2f;
			
			_control.Creature.Status.DefaultStamina = 100;
			_control.Creature.Status.StaminaDamageMultiplier = 0.2f;
			_control.Creature.Status.StaminaStressMultiplier = 0.1f;
			_control.Creature.Status.StaminaHealthMultiplier = 0.8f;
			_control.Creature.Status.StaminaHungerMultiplier = 0.1f;
			_control.Creature.Status.StaminaThirstMultiplier = 0.3f;
			_control.Creature.Status.StaminaAgeMultiplier = 0.1f;
			_control.Creature.Status.StaminaTemperaturMultiplier = 0.2f;
			
			_control.Creature.Status.DefaultPower = 100;
			_control.Creature.Status.PowerDamageMultiplier  = 0.2f;
			_control.Creature.Status.PowerStressMultiplier = 0.5f;
			_control.Creature.Status.PowerHealthMultiplier = 0.8f;
			_control.Creature.Status.PowerStaminaMultiplier = 0.2f;
			_control.Creature.Status.PowerHungerMultiplier = 0.1f;
			_control.Creature.Status.PowerThirstMultiplier = 0.2f;
			_control.Creature.Status.PowerAgeMultiplier = 0.1f;
			_control.Creature.Status.PowerTemperaturMultiplier = 0.2f;
			
			_control.Creature.Status.UseTime = false;
			_control.Creature.Status.UseAging = false;
			_control.Creature.Status.SetAge( 0 );
			_control.Creature.Status.MaxAge = 3600;
			_control.Creature.Status.UseTemperature = false;
			_control.Creature.Status.TemperatureScale = TemperatureScaleType.CELSIUS;
			_control.Creature.Status.Temperature = 25;
			_control.Creature.Status.BestTemperature = 25;
			_control.Creature.Status.MinTemperature = -25;
			_control.Creature.Status.MaxTemperature = 50;
			_control.Creature.Status.UseArmor = false;
			_control.Creature.Status.ArmorInPercent = 100;
			
			_control.Creature.Status.PerceptionTime = 2;
			_control.Creature.Status.PerceptionTimeFitnessMultiplier = 0;
			_control.Creature.Status.ReactionTime = 2;
			_control.Creature.Status.ReactionTimeFitnessMultiplier = 0;
			_control.Creature.Status.RespawnTime = 20;
		}
		
		
		private static void DrawStatus( ICECreatureControl _control )
		{
			if( ! _control.Display.ShowStatus )
				return;

			EditorGUI.indentLevel++;

				if( _control.Creature.Status.UseAdvanced )
				{
					if( _control.Creature.Status.UseAging )
					{
						EditorGUILayout.LabelField( "Age", "Current age : " + (int)(_control.Creature.Status.Age/60) + "min. / Max. age : " + (int)(_control.Creature.Status.MaxAge/60) + "min." );
						EditorGUI.indentLevel++;					
							ICEEditorLayout.DrawProgressBar("Lifespan", _control.Creature.Status.LifespanInPercent);					
						EditorGUI.indentLevel--;
					}
					
					EditorGUILayout.Separator();					
					ICEEditorLayout.DrawProgressBar("Fitness (major vital sign)", _control.Creature.Status.FitnessInPercent);				
					EditorGUILayout.Separator();				
					EditorGUI.indentLevel++;				
						ICEEditorLayout.DrawProgressBar("Health", _control.Creature.Status.HealthInPercent );
						ICEEditorLayout.DrawProgressBar("Stamina", _control.Creature.Status.StaminaInPercent );
						ICEEditorLayout.DrawProgressBar("Power", _control.Creature.Status.PowerInPercent );				
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
					ICEEditorLayout.DrawProgressBar("Aggressivity (" + _control.Creature.Status.Aggressivity.ToString()  + ")", _control.Creature.Status.Aggressivity * 10 );
				}
				else
				{
					ICEEditorLayout.DrawProgressBar("Health", _control.Creature.Status.HealthInPercent );
				}
			
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
		}

		private static void DrawStatusInfluences( ICECreatureControl _control )
		{
			if( ! _control.Display.ShowStatus )
				return;
			
			
			EditorGUILayout.LabelField( "Influences", ICEEditorStyle.LabelBold );

			EditorGUI.indentLevel++;
				EditorGUILayout.LabelField( "Fitness" );
				EditorGUI.indentLevel++;
					_control.Creature.Status.FitnessSpeedMultiplier = ICEEditorLayout.Slider("Velocity Multiplier (-)","", _control.Creature.Status.FitnessSpeedMultiplier, 0.01f,0,1);
				EditorGUI.indentLevel--;
			EditorGUI.indentLevel--;
		}
		
		private static void DrawStatusBasics( ICECreatureControl _control )
		{
			if( ! _control.Display.ShowStatus )
				return;
			
			
			ICEEditorLayout.Label( "Basics", true, Info.STATUS_BASICS );
				
			EditorGUILayout.Separator();					
			EditorGUI.indentLevel++;
			
			_control.Creature.Status.UseAdvanced = ICEEditorLayout.Toggle( "Use Advanced","", _control.Creature.Status.UseAdvanced, Info.STATUS_ADVANCED  ); 

			
			EditorGUILayout.Separator();	
		
			_control.Creature.Status.PerceptionTime = ICEEditorLayout.DefaultSlider( "Perception Time (secs.)","", _control.Creature.Status.PerceptionTime, Init.STATUS_PERCEPTION_TIME_STEP, Init.STATUS_PERCEPTION_TIME_MIN, Init.STATUS_PERCEPTION_TIME_MAX, Init.STATUS_PERCEPTION_TIME_DEFAULT, Info.STATUS_PERCEPTION_TIME  );

			EditorGUI.indentLevel++;
			_control.Creature.Status.PerceptionTimeVariance = ICEEditorLayout.DefaultSlider("Variance Multiplier","", _control.Creature.Status.PerceptionTimeVariance,0.01f, 0,1, 0.25f, Info.STATUS_PERCEPTION_TIME_VARIANCE );

			if( _control.Creature.Status.UseAdvanced )
			{
				_control.Creature.Status.PerceptionTimeFitnessMultiplier = ICEEditorLayout.DefaultSlider("Fitness Multiplier (inv. +)","", _control.Creature.Status.PerceptionTimeFitnessMultiplier,0.01f, 0,1, 0.3f, Info.STATUS_PERCEPTION_TIME_MULTIPLIER );
				EditorGUILayout.Separator();
			}
			EditorGUI.indentLevel--;

			_control.Creature.Status.ReactionTime = ICEEditorLayout.DefaultSlider("Reaction Time (secs.)","", _control.Creature.Status.ReactionTime, Init.STATUS_REACTION_TIME_STEP, Init.STATUS_REACTION_TIME_MIN, Init.STATUS_REACTION_TIME_MAX, Init.STATUS_REACTION_TIME_DEFAULT, Info.STATUS_REACTION_TIME );

			EditorGUI.indentLevel++;
				_control.Creature.Status.ReactionTimeVariance = ICEEditorLayout.DefaultSlider("Variance Multiplier","", _control.Creature.Status.ReactionTimeVariance,0.01f, 0,1, 0.25f, Info.STATUS_REACTION_TIME_VARIANCE );
			EditorGUI.indentLevel--;

			if( _control.Creature.Status.UseAdvanced )
			{
				EditorGUI.indentLevel++;
				_control.Creature.Status.ReactionTimeFitnessMultiplier = ICEEditorLayout.DefaultSlider("Fitness Multiplier (inv. +)","", _control.Creature.Status.ReactionTimeFitnessMultiplier,0.01f, 0,1, 0.3f, Info.STATUS_REACTION_TIME_MULTIPLIER );
				EditorGUI.indentLevel--;
			}
			else
			{
				EditorGUILayout.Separator();
				_control.Creature.Status.DamageInPercent = ICEEditorLayout.Slider( "Damage","", _control.Creature.Status.DamageInPercent,1, 0,100, Info.STATUS_DAMAGE_IN_PERCENT );
			}

			EditorGUILayout.Separator();

			_control.Creature.Status.FitnessRecreationLimit = ICEEditorLayout.DefaultSlider("Recreation Limit (%)","If the fitness value reached this limit your creature will go home to recreate.", _control.Creature.Status.FitnessRecreationLimit, 0.5f, 0, 100,20, Info.STATUS_FITNESS_RECREATION_LIMIT );
			_control.Creature.Status.RespawnTime = ICEEditorLayout.DefaultSlider("Respawn Time (secs.)","Defines how the creature will be visible after dying and before respawning.", _control.Creature.Status.RespawnTime, 0.5f, 0, 360, 20, Info.STATUS_RESPAWN_TIME );
			EditorGUI.indentLevel++;
			_control.Creature.Status.RespawnTimeVariance = ICEEditorLayout.DefaultSlider("Variance Multiplier","", _control.Creature.Status.RespawnTimeVariance,0.01f, 0,1, 0.25f, Info.STATUS_RESPAWN_TIME_VARIANCE );
			EditorGUI.indentLevel--;

			
	
			EditorGUI.indentLevel--;
			
			EditorGUILayout.Separator();
		}


		private static void DrawStatusAdvanced( ICECreatureControl _control )
		{
			if( ! _control.Creature.Status.UseAdvanced )
				return;

			ICEEditorLayout.Label( "Advanced", true, Info.STATUS_ADVANCED );
			EditorGUILayout.Separator();
			EditorGUI.indentLevel++;

				_control.Creature.Status.UseAging = ICEEditorLayout.Toggle("Use Aging","", _control.Creature.Status.UseAging, Info.STATUS_AGING );			
				if( _control.Creature.Status.UseAging )
				{					
					EditorGUI.indentLevel++;
						_control.Creature.Status.SetAge( ICEEditorLayout.Slider( "Cur. Age (minutes)", "", _control.Creature.Status.Age/60, 1, 0, _control.Creature.Status.MaxAge/60, Info.STATUS_AGING_AGE )*60 );
						_control.Creature.Status.MaxAge = ICEEditorLayout.Slider( "Max. Age (minutes)","", _control.Creature.Status.MaxAge/60, 1, 1, 120, Info.STATUS_AGING_MAXAGE )*60;
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}

				_control.Creature.Status.UseTemperature = ICEEditorLayout.Toggle( "Use Temperature","", _control.Creature.Status.UseTemperature, Info.STATUS_TEMPERATURE );			
				if( _control.Creature.Status.UseTemperature )
				{
					EditorGUI.indentLevel++;
					_control.Creature.Status.TemperatureScale = (TemperatureScaleType)ICEEditorLayout.EnumPopup( "Scale","", _control.Creature.Status.TemperatureScale, Info.STATUS_TEMPERATURE_SCALE );


					_control.Creature.Status.MinTemperature = ICEEditorLayout.Float( "Min. Temperature","", _control.Creature.Status.MinTemperature, Info.STATUS_TEMPERATURE_MIN );
					_control.Creature.Status.MaxTemperature = ICEEditorLayout.Float( "Max. Temperature","", _control.Creature.Status.MaxTemperature, Info.STATUS_TEMPERATURE_MAX );

					EditorGUILayout.Separator();
					_control.Creature.Status.BestTemperature = ICEEditorLayout.Slider( "Comfort Temperature","", _control.Creature.Status.BestTemperature, 1,_control.Creature.Status.MinTemperature,_control.Creature.Status.MaxTemperature, Info.STATUS_TEMPERATURE_BEST );
					_control.Creature.Status.Temperature = ICEEditorLayout.Slider( "Temperature","", _control.Creature.Status.Temperature, 1,_control.Creature.Status.MinTemperature,_control.Creature.Status.MaxTemperature, Info.STATUS_TEMPERATURE_CURRENT );

					EditorGUI.indentLevel--;

					EditorGUILayout.Separator();
				}

				_control.Creature.Status.UseArmor = ICEEditorLayout.Toggle( "Use Armor","", _control.Creature.Status.UseArmor, Info.STATUS_ARMOR );			
				if( _control.Creature.Status.UseArmor )
				{
					EditorGUI.indentLevel++;
					_control.Creature.Status.ArmorInPercent = ICEEditorLayout.DefaultSlider( "Armor","", _control.Creature.Status.ArmorInPercent, 1,0,100, 100, Info.STATUS_ARMOR_IN_PERCENT );
					EditorGUI.indentLevel--;
				}

				EditorGUILayout.Separator();
				ICEEditorLayout.Label( "Influence Indicators", false, Info.STATUS_INFLUENCE_INDICATORS );
				EditorGUI.indentLevel++;
					_control.Creature.Status.DamageInPercent = ICEEditorLayout.DefaultSlider( "Damage","", _control.Creature.Status.DamageInPercent,1, 0,100,0, Info.STATUS_DAMAGE_IN_PERCENT);
					_control.Creature.Status.StressInPercent = ICEEditorLayout.DefaultSlider("Stress","", _control.Creature.Status.StressInPercent,1, 0,100,0, Info.STATUS_STRESS_IN_PERCENT);
					_control.Creature.Status.DebilityInPercent = ICEEditorLayout.DefaultSlider("Debility","", _control.Creature.Status.DebilityInPercent,1, 0,100,0, Info.STATUS_DEBILITY_IN_PERCENT);
					_control.Creature.Status.HungerInPercent = ICEEditorLayout.DefaultSlider( "Hunger","", _control.Creature.Status.HungerInPercent,1, 0,100,0, Info.STATUS_HUNGER_IN_PERCENT);
					_control.Creature.Status.ThirstInPercent = ICEEditorLayout.DefaultSlider("Thirst","", _control.Creature.Status.ThirstInPercent,1, 0,100,0, Info.STATUS_THIRST_IN_PERCENT);
				EditorGUI.indentLevel--;
				
			
				EditorGUILayout.Separator();
				ICEEditorLayout.Label( "Vital Indicators", false, Info.STATUS_VITAL_INDICATORS );
				EditorGUI.indentLevel++;
					_control.Creature.Status.DefaultHealth = ICEEditorLayout.Integer("Health","", _control.Creature.Status.DefaultHealth, Info.STATUS_VITAL_INDICATOR_HEALTH );
					EditorGUI.indentLevel++;			
						_control.Creature.Status.HealthDamageMultiplier = ICEEditorLayout.Slider("Damage Multiplier (-)","", _control.Creature.Status.HealthDamageMultiplier, 0.01f,0,1);
						_control.Creature.Status.HealthStressMultiplier = ICEEditorLayout.Slider("Stress Multiplier (-)","", _control.Creature.Status.HealthStressMultiplier, 0.01f,0,1);
						_control.Creature.Status.HealthDebilityMultiplier = ICEEditorLayout.Slider("Debility Multiplier (-)","", _control.Creature.Status.HealthDebilityMultiplier, 0.01f,0,1);
						_control.Creature.Status.HealthHungerMultiplier = ICEEditorLayout.Slider("Hunger Multiplier (-)","", _control.Creature.Status.HealthHungerMultiplier, 0.01f,0,1);
						_control.Creature.Status.HealthThirstMultiplier = ICEEditorLayout.Slider("Thirst Multiplier (-)","", _control.Creature.Status.HealthThirstMultiplier, 0.01f,0,1);
						_control.Creature.Status.HealthRecreationMultiplier = ICEEditorLayout.Slider("Recreation Multiplier (+)","", _control.Creature.Status.HealthRecreationMultiplier, 0.01f,0,1);

						if( _control.Creature.Status.UseTemperature )
							_control.Creature.Status.HealthTemperaturMultiplier = ICEEditorLayout.Slider("Temperatur Multiplier (+)","", _control.Creature.Status.HealthTemperaturMultiplier, 0.01f,0,1);
						if( _control.Creature.Status.UseAging )
							_control.Creature.Status.HealthAgeMultiplier = ICEEditorLayout.Slider("Aging Multiplier (+)","", _control.Creature.Status.HealthAgeMultiplier, 0.01f,0,1);
					EditorGUI.indentLevel--;
				
					EditorGUILayout.Separator();			
					_control.Creature.Status.DefaultStamina = ICEEditorLayout.Integer("Stamina","", _control.Creature.Status.DefaultStamina, Info.STATUS_VITAL_INDICATOR_STAMINA);
					EditorGUI.indentLevel++;			
						_control.Creature.Status.StaminaDamageMultiplier = ICEEditorLayout.Slider("Damage Multiplier (-)","", _control.Creature.Status.StaminaDamageMultiplier, 0.01f,0,1);
						_control.Creature.Status.StaminaStressMultiplier = ICEEditorLayout.Slider("Stress Multiplier  (-)","", _control.Creature.Status.StaminaStressMultiplier, 0.01f,0,1);
						_control.Creature.Status.StaminaDebilityMultiplier = ICEEditorLayout.Slider("Debility Multiplier (-)","", _control.Creature.Status.StaminaDebilityMultiplier, 0.01f,0,1);
						_control.Creature.Status.StaminaHealthMultiplier = ICEEditorLayout.Slider("Health Multiplier (-)","", _control.Creature.Status.StaminaHealthMultiplier, 0.01f,0,1);
						_control.Creature.Status.StaminaHungerMultiplier = ICEEditorLayout.Slider("Hunger Multiplier (-)","", _control.Creature.Status.StaminaHungerMultiplier, 0.01f,0,1);
						_control.Creature.Status.StaminaThirstMultiplier = ICEEditorLayout.Slider("Thirst Multiplier (-)","", _control.Creature.Status.StaminaThirstMultiplier, 0.01f,0,1);

						if( _control.Creature.Status.UseTemperature )
							_control.Creature.Status.StaminaTemperaturMultiplier = ICEEditorLayout.Slider("Temperatur Multiplier (+)","", _control.Creature.Status.StaminaTemperaturMultiplier, 0.01f,0,1);
						if( _control.Creature.Status.UseAging )
							_control.Creature.Status.StaminaAgeMultiplier = ICEEditorLayout.Slider("Aging Multiplier (+)","", _control.Creature.Status.StaminaAgeMultiplier, 0.01f,0,1);
					EditorGUI.indentLevel--;
				
					EditorGUILayout.Separator();			
					_control.Creature.Status.DefaultPower = ICEEditorLayout.Integer("Power","", _control.Creature.Status.DefaultPower, Info.STATUS_VITAL_INDICATOR_POWER);
					EditorGUI.indentLevel++;
						_control.Creature.Status.PowerDamageMultiplier = ICEEditorLayout.Slider("Damage Multiplier (-)","", _control.Creature.Status.PowerDamageMultiplier, 0.01f,0,1);
						_control.Creature.Status.PowerStressMultiplier = ICEEditorLayout.Slider("Stress Multiplier (+)","", _control.Creature.Status.PowerStressMultiplier, 0.01f,0,1);
						_control.Creature.Status.PowerDebilityMultiplier = ICEEditorLayout.Slider("Debility Multiplier (-)","", _control.Creature.Status.PowerDebilityMultiplier, 0.01f,0,1);
						_control.Creature.Status.PowerHealthMultiplier = ICEEditorLayout.Slider("Health Multiplier (-)","", _control.Creature.Status.PowerHealthMultiplier, 0.01f,0,1);
						_control.Creature.Status.PowerStaminaMultiplier = ICEEditorLayout.Slider("Stamina Multiplier (-)","", _control.Creature.Status.PowerStaminaMultiplier, 0.01f,0,1);
						_control.Creature.Status.PowerHungerMultiplier = ICEEditorLayout.Slider("Hunger Multiplier (-)","", _control.Creature.Status.PowerHungerMultiplier, 0.01f,0,1);
						_control.Creature.Status.PowerThirstMultiplier = ICEEditorLayout.Slider("Thirst Multiplier (-)","", _control.Creature.Status.PowerThirstMultiplier, 0.01f,0,1);

						if( _control.Creature.Status.UseTemperature )
							_control.Creature.Status.PowerTemperaturMultiplier = ICEEditorLayout.Slider("Temperatur Multiplier (+)","", _control.Creature.Status.PowerTemperaturMultiplier, 0.01f,0,1);
						if( _control.Creature.Status.UseAging )
							_control.Creature.Status.PowerAgeMultiplier = ICEEditorLayout.Slider("Aging Multiplier (+)","", _control.Creature.Status.PowerAgeMultiplier, 0.01f,0,1);
					EditorGUI.indentLevel--;				


					EditorGUILayout.Separator();
					_control.Creature.Status.DefaultAggressivity = (int)ICEEditorLayout.Slider("Aggressivity","", _control.Creature.Status.DefaultAggressivity,1, 0,10, Info.STATUS_DEFAULT_AGGRESSITY );
					EditorGUI.indentLevel++;			
						_control.Creature.Status.AggressivityDamageMultiplier = ICEEditorLayout.Slider("Damage Multiplier (+)","", _control.Creature.Status.AggressivityDamageMultiplier,0.1f, 0,1);
						_control.Creature.Status.AggressivityStressMultiplier = ICEEditorLayout.Slider("Stress Multiplier (+)","", _control.Creature.Status.AggressivityStressMultiplier, 0.01f,0,1);
						_control.Creature.Status.AggressivityDebilityMultiplier = ICEEditorLayout.Slider("Debility Multiplier (-)","", _control.Creature.Status.AggressivityDebilityMultiplier, 0.01f,0,1);
						_control.Creature.Status.AggressivityHealthMultiplier = ICEEditorLayout.Slider("Health Multiplier (-)","", _control.Creature.Status.AggressivityHealthMultiplier, 0.01f,0,1);
						_control.Creature.Status.AggressivityStaminaMultiplier = ICEEditorLayout.Slider("Stamina Multiplier (-)","", _control.Creature.Status.AggressivityStaminaMultiplier, 0.01f,0,1);
						_control.Creature.Status.AggressivityHungerMultiplier = ICEEditorLayout.Slider("Hunger Multiplier (+)","", _control.Creature.Status.AggressivityHungerMultiplier, 0.01f,0,1);
						_control.Creature.Status.AggressivityThirstMultiplier = ICEEditorLayout.Slider("Thirst Multiplier (+)","", _control.Creature.Status.AggressivityThirstMultiplier, 0.01f,0,1);
						
						if( _control.Creature.Status.UseTemperature )
							_control.Creature.Status.AggressivityTemperaturMultiplier = ICEEditorLayout.Slider("Temperatur Multiplier (+)","", _control.Creature.Status.AggressivityTemperaturMultiplier, 0.01f,0,1);
						if( _control.Creature.Status.UseAging )
							_control.Creature.Status.AggressivityAgeMultiplier = ICEEditorLayout.Slider("Aging Multiplier (+)","", _control.Creature.Status.AggressivityAgeMultiplier, 0.01f,0,1);
					EditorGUI.indentLevel--;
				EditorGUI.indentLevel--;
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
			
		}

	}
}