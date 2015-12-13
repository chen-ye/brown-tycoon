using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures.EnumTypes;

namespace ICE
{
	namespace Creatures
	{
		namespace Objects
		{
			[System.Serializable]
			public class StatusObject : System.Object
			{
				public bool UseAdvanced = false;
				public bool UseArmor = false;

				public int DefaultAggressivity = 0;
				public float AggressivityHealthMultiplier = 1.0f;
				public float AggressivityDamageMultiplier = 0.01f;
				public float AggressivityStaminaMultiplier = 0.01f;
				public float AggressivityStressMultiplier = 0.01f;
				public float AggressivityDebilityMultiplier = 0.25f;
				public float AggressivityHungerMultiplier = 0.0f;
				public float AggressivityThirstMultiplier = 0.0f;
				public float AggressivityTemperaturMultiplier = 0.0f;
				public float AggressivityAgeMultiplier = 0.0f;

					// Default Fitness
				public int DefaultHealth = 100;
				public float HealthDamageMultiplier = 1.0f;
				public float HealthStressMultiplier = 1.0f;
				public float HealthDebilityMultiplier = 1.0f;
				public float HealthHungerMultiplier = 0.0f;
				public float HealthThirstMultiplier = 0.0f;
				public float HealthRecreationMultiplier = 0.2f;
				public float HealthTemperaturMultiplier = 0.0f;
				public float HealthAgeMultiplier = 0.0f;

				public int DefaultPower = 100;
				public float PowerHealthMultiplier = 1.0f;
				public float PowerDamageMultiplier = 0.01f;
				public float PowerStaminaMultiplier = 0.01f;
				public float PowerStressMultiplier = 0.01f;
				public float PowerDebilityMultiplier = 0.01f;
				public float PowerHungerMultiplier = 0.0f;
				public float PowerThirstMultiplier = 0.0f;
				public float PowerTemperaturMultiplier = 0.0f;
				public float PowerAgeMultiplier = 0.0f;

				public int DefaultStamina = 100;
				public float StaminaHealthMultiplier = 0.01f;
				public float StaminaDamageMultiplier = 0.01f;
				public float StaminaStressMultiplier = 0.01f;
				public float StaminaDebilityMultiplier = 0.01f;
				public float StaminaHungerMultiplier = 0.0f;
				public float StaminaThirstMultiplier = 0.0f;
				public float StaminaTemperaturMultiplier = 0.0f;
				public float StaminaAgeMultiplier = 0.0f;

				public float RecreationMultiplier = 0.01f;

				public float FitnessSpeedMultiplier = 0.01f;

				public float SpeedMultiplier
				{
					get{
						float current_value = 100;
						
						current_value -= ( 100 - FitnessInPercent ) * FitnessSpeedMultiplier;
						
						return FixedMultiplier( current_value / 100 );
					}
				}

				public float FitnessRecreationLimit = 25;
				public bool RecreationRequired
				{
					get{
						if( FitnessRecreationLimit == 0 )
							return false;
						else if( FitnessInPercent <= FitnessRecreationLimit )
							return true;
						else
							return false;
					}

				}

				public bool ConsiderBreathing = false;


				public bool UseTemperature = false;
				private TemperatureScaleType m_TemperatureScale;
				public TemperatureScaleType TemperatureScale
				{
					get{ return m_TemperatureScale; }
					set{
						if( m_TemperatureScale != value )
						{
							if( value == TemperatureScaleType.CELSIUS && m_TemperatureScale == TemperatureScaleType.FAHRENHEIT )
							{
								Temperature = Tools.FahrenheitToCelsius( Temperature );
								MaxTemperature = Tools.FahrenheitToCelsius( MaxTemperature );
								MinTemperature = Tools.FahrenheitToCelsius( MinTemperature );
								BestTemperature = Tools.FahrenheitToCelsius( BestTemperature );
							}
							else if( value == TemperatureScaleType.FAHRENHEIT && m_TemperatureScale == TemperatureScaleType.CELSIUS )
							{
								Temperature = Tools.CelsiusToFahrenheit( Temperature );
								MaxTemperature = Tools.CelsiusToFahrenheit( MaxTemperature );
								MinTemperature = Tools.CelsiusToFahrenheit( MinTemperature );
								BestTemperature = Tools.CelsiusToFahrenheit( BestTemperature );
							}
						}
						m_TemperatureScale = value;
					}

				}

				public float MinTemperature = -25;
				public float MaxTemperature = 50;
				public float BestTemperature = 25;
				public float Temperature = 25;

				public bool UseTime = false;
				public int TimeHour = 0;
				public int TimeMinutes = 0;
				public int TimeSeconds = 0;

				public bool UseDate = false;
				public int DateDay = 0;
				public int DateMonth = 0;
				public int DateYear = 0;

				public WeatherType Weather = WeatherType.UNDEFINED;

				public float PerceptionTime = 2.0f;
				public float PerceptionTimeVariance = 0.25f;
				public bool AutoPerceptionTime = true;
				public float PerceptionTimeFitnessMultiplier = 0.0f;

				public float ReactionTime = 2.0f;
				public float ReactionTimeVariance = 0.25f;
				public bool AutoReactionTime = true;
				public float ReactionTimeFitnessMultiplier = 0.0f;

				public float RespawnTime = 20.0f;
				public float RespawnTimeVariance = 0.25f;

				public float MaxAge = 2.0f;
		

				private GameObject m_Owner = null;
				
				public void Init( GameObject gameObject )
				{
					m_Owner = gameObject;

					PerceptionTime = PerceptionTime + ( PerceptionTime * Random.Range( - PerceptionTimeVariance, PerceptionTimeVariance ) );
					ReactionTime = ReactionTime + ( ReactionTime * Random.Range( - ReactionTimeVariance, ReactionTimeVariance ) );
					RespawnTime = RespawnTime + ( RespawnTime * Random.Range( - RespawnTimeVariance, RespawnTimeVariance ) );

					m_SenseTimer = PerceptionTime;
					m_ReactionTimer = ReactionTime;
					m_RespawnTimer = RespawnTime;

				}


				public bool UseAging = false;
				private float m_Age = 0.0f;
				public float Age{ 
					get{ return m_Age; }
				}

				public void SetAge( float _age )
				{ 
					if( _age >= 0 && _age <= MaxAge )
						m_Age = _age;
				}

				public float LifespanInPercent
				{
		
					get{ return FixedPercent( 100 - ( 100/MaxAge*m_Age) ); }
				}
		
				public float TemperaturDeviationInPercent
				{

					get{ 
						float _tmp_1 = 0;
						float _tmp_2 = 0;
						if( Temperature < BestTemperature )
						{
							_tmp_1 = Mathf.Abs(MinTemperature - BestTemperature);
							_tmp_2 = Mathf.Abs(MinTemperature - Temperature);
						}
						else
						{
							_tmp_1 = Mathf.Abs(MaxTemperature - BestTemperature);
							_tmp_2 = Mathf.Abs(MaxTemperature - Temperature);
						}

						return FixedPercent( 100 - ( 100/_tmp_1*_tmp_2 ) ); 
					}
				}

				public void UpdateEnvironmentInfos( EnvironmentInfoContainer _infos )
				{
					Temperature = _infos.Temperature;
					DateDay = _infos.DateDay;
					DateMonth = _infos.DateMonth;
					DateYear = _infos.DateYear;
					TimeHour = _infos.TimeHour;
					TimeMinutes = _infos.TimeMinutes;
					TimeSeconds = _infos.TimeSeconds;
					Weather = _infos.Weather;
			
				}

				
				private float m_SenseTimer = 0.0f;
				public bool IsSenseTime()
				{
					if( IsDead )
						return false;

					float _delay = 0;
					if( UseAdvanced && ReactionTimeFitnessMultiplier > 0 )
						_delay = ( ( 100 - FitnessInPercent ) * ReactionTimeFitnessMultiplier * 0.1f );

					if( m_SenseTimer >= PerceptionTime + _delay )
					{
						m_SenseTimer = 0;

						UpdateEnvironmentInfos( CreatureRegister.EnvironmentInfos );



						return true;
					}
					else
					{
						return false;
					}
					
				}

				public bool IsReactionForced = false;
				private float m_ReactionTimer = 0.0f;
				public bool IsReactionTime()
				{
					if( IsDead )
						return false;

					float _delay = 0;
					if( UseAdvanced && ReactionTimeFitnessMultiplier > 0 )
						_delay = ( ( 100 - FitnessInPercent ) * ReactionTimeFitnessMultiplier * 0.1f );

					if( m_ReactionTimer >= ReactionTime + _delay || IsReactionForced )
					{
						m_ReactionTimer = 0;
						IsReactionForced = false;
						return true;
					}
					else
					{
						return false;
					}
					
				}

				private bool m_RespawnRequired = false;
				private float m_RespawnTimer = 0.0f;
				public bool IsRespawnTime{
					get{ return ( m_RespawnTimer >= RespawnTime )?true:false; }
				}



				public void RespawnRequest()
				{
					if( m_RespawnRequired || ! IsDead )
						return;

					m_RespawnTimer = 0.0f;
					m_RespawnRequired = true;
				}
				
				public void Respawn()
				{
					if( ! m_RespawnRequired || ! IsDead )
						return;

					m_RespawnTimer = 0.0f;
					m_RespawnRequired = false;
					
					CreatureRegister.DeathNotice( m_Owner );
				}

				/// <summary>
				/// Reset the status values.
				/// </summary>
				public void Reset()
				{
					m_SenseTimer = PerceptionTime;
					m_ReactionTimer = ReactionTime;

					m_RespawnTimer = 0.0f;
					m_RespawnRequired = false;

					m_Age = 0;
					m_DamageInPercent = 0;
					m_StressInPercent = 0;
					m_DebilityInPercent = 0;
					m_HungerInPercent = 0;
					m_ThirstInPercent = 0;

					if( ! UseArmor )
						m_ArmorInPercent = 100;

				}

				public void Kill()
				{
					m_Age = MaxAge;
					m_DamageInPercent = 100;
					m_StressInPercent = 100;
					m_DebilityInPercent = 100;
					m_HungerInPercent = 100;
					m_ThirstInPercent = 100;
					
					if( ! UseArmor )
						m_ArmorInPercent = 0;
				}

				private float m_ArmorInPercent = 0;
				public float ArmorInPercent
				{
					set{ m_ArmorInPercent = value; }
					get{ return FixedPercent( m_ArmorInPercent ); }
				}

				private float HandleArmor( float _damage )
				{
					if( ! UseArmor )
						return _damage;

					if( _damage < 0 )
						return _damage;

					m_ArmorInPercent -= _damage;

					if( ArmorInPercent > 0 )
						_damage -= m_ArmorInPercent / 100;
		
					return _damage;
				}

				public void ResetArmor()
				{
					if( UseArmor )
						m_ArmorInPercent = 100;
					else
						m_ArmorInPercent = 0;
				}


				private float m_DamageInPercent = 0;
				public float DamageInPercent
				{
					set{ m_DamageInPercent = value; }
					get{ return FixedPercent( m_DamageInPercent ); }
				}

				public void AddDamage( float _damage )
				{
					_damage = HandleArmor( _damage );

					m_DamageInPercent += _damage;
					m_DamageInPercent = FixedPercent( m_DamageInPercent );
				}

				private float m_StressInPercent = 0;
				public float StressInPercent
				{
					set{ m_StressInPercent = value; }
					get{ return FixedPercent( m_StressInPercent ); }
				}
			
				public void AddStress( float _stress )
				{
					m_StressInPercent += _stress;
					m_StressInPercent = FixedPercent( m_StressInPercent );
				}

				private float m_DebilityInPercent = 0;
				public float DebilityInPercent
				{
					set{ m_DebilityInPercent = value; }
					get{ return FixedPercent( m_DebilityInPercent ); }
				}

				public void AddDebility( float _debility )
				{
					m_DebilityInPercent += _debility;
					m_DebilityInPercent = FixedPercent( m_DebilityInPercent );
				}


				private float m_HungerInPercent = 0;
				public float HungerInPercent
				{
					set{ m_HungerInPercent = value; }
					get{ return FixedPercent( m_HungerInPercent ); }
				}

				public void AddHunger( float _hunger )
				{
					m_HungerInPercent += _hunger;
					m_HungerInPercent = FixedPercent( m_HungerInPercent );
				}

				private float m_ThirstInPercent = 0;
				public float ThirstInPercent
				{
					set{ m_ThirstInPercent = value; }
					get{ return FixedPercent( m_ThirstInPercent ); }
				}

				public void AddThirst( float _thirst )
				{
					m_ThirstInPercent += _thirst;
					m_ThirstInPercent = FixedPercent( m_ThirstInPercent );
				}


				public float HealthInPercent
				{
					get{
						float max_value = DefaultHealth;
						float current_value = max_value;
										
						if( max_value > 0 )
							current_value = ( 100 / max_value ) * current_value;
						else
							current_value = 0;

						if( UseAdvanced )
						{
							current_value -= DamageInPercent * HealthDamageMultiplier;
							current_value -= StressInPercent * HealthStressMultiplier;
							current_value -= DebilityInPercent * HealthDebilityMultiplier;
							current_value -= HungerInPercent * HealthHungerMultiplier;
							current_value -= ThirstInPercent * HealthThirstMultiplier;

							if( UseTemperature )
								current_value -= TemperaturDeviationInPercent * HealthTemperaturMultiplier;
							
							if( UseAging )
								current_value -= ( 100 - LifespanInPercent ) * HealthAgeMultiplier;

							if( UseAging && m_Age >= MaxAge )
								current_value = 0;
						}
						else
						{
							current_value -= DamageInPercent;
						}

									
						return FixedPercent( current_value );
					}
				}

				public float StaminaInPercent
				{
					get{
						float max_value = DefaultStamina;
						float current_value = max_value;
							
						if( max_value > 0 )
							current_value = ( 100 / max_value ) * current_value;
						else
							current_value = 0;

						if( UseAdvanced )
						{
							if( UseTemperature )
								current_value -= TemperaturDeviationInPercent * StaminaTemperaturMultiplier;

							if( UseAging )
								current_value -= ( 100 - LifespanInPercent ) * StaminaAgeMultiplier;

							current_value -= DamageInPercent * StaminaDamageMultiplier;
							current_value -= StressInPercent * StaminaStressMultiplier;
							current_value -= HungerInPercent * StaminaHungerMultiplier;
							current_value -= ThirstInPercent * StaminaThirstMultiplier;
							current_value -= DebilityInPercent * StaminaDebilityMultiplier;

							current_value -= ( ( 100 - HealthInPercent ) * StaminaHealthMultiplier );
						}

						if( IsDead )
							current_value = 0;

						return FixedPercent( current_value );
					}
				}

				public float PowerInPercent
				{
					get{
						float max_value = DefaultPower;
						float current_value = max_value;

						if( max_value > 0 )
							current_value = ( 100 / max_value ) * current_value;
						else
							current_value = 0;

						if( UseAdvanced )
						{
							if( UseTemperature )
								current_value -= TemperaturDeviationInPercent * PowerTemperaturMultiplier;

							if( UseAging )
								current_value -= ( 100 - LifespanInPercent ) * PowerAgeMultiplier;

							current_value += StressInPercent * PowerStressMultiplier;
							current_value -= DebilityInPercent * PowerDebilityMultiplier;
							current_value -= DamageInPercent * PowerDamageMultiplier;
							current_value -= HungerInPercent * PowerHungerMultiplier;
							current_value -= ThirstInPercent * PowerThirstMultiplier;
							current_value -= ( ( 100 - StaminaInPercent ) * PowerStaminaMultiplier );
							current_value -= ( ( 100 - HealthInPercent ) * PowerHealthMultiplier );
						}

						if( IsDead )
							current_value = 0;

						return FixedPercent( current_value );
					}
				}

				public bool IsDead
				{
					get{ return ( HealthInPercent <= 0 )?true:false; }
				}

				public int Aggressivity
				{
					get{
						float default_aggressivity = DefaultAggressivity;
						float current_aggressivity = default_aggressivity;

						if( default_aggressivity > 0 )
							current_aggressivity = ( 100 / default_aggressivity ) * current_aggressivity;
						else
							current_aggressivity = 0;

						if( UseAdvanced )
						{
							if( UseTemperature )
								current_aggressivity -= TemperaturDeviationInPercent * AggressivityTemperaturMultiplier;

							if( UseAging )
								current_aggressivity -= ( 100 - LifespanInPercent ) * AggressivityAgeMultiplier;

							current_aggressivity += DamageInPercent * AggressivityDamageMultiplier;
							current_aggressivity += StressInPercent * AggressivityStressMultiplier;
							current_aggressivity -= DebilityInPercent * AggressivityDebilityMultiplier;
							current_aggressivity += HungerInPercent * AggressivityHungerMultiplier;
							current_aggressivity += ThirstInPercent * AggressivityThirstMultiplier;
							current_aggressivity -= ( ( 100 - StaminaInPercent ) * AggressivityStaminaMultiplier );
							current_aggressivity -= ( ( 100 - HealthInPercent ) * AggressivityHealthMultiplier );

							current_aggressivity = default_aggressivity * ( current_aggressivity / 100 );
						}

						if( IsDead )
							current_aggressivity = 0;
							
						return Mathf.RoundToInt( current_aggressivity );
					}
				}

				public float FitnessInPercent
				{
					get{
						float max_fitness = ( DefaultPower + DefaultHealth + DefaultStamina ) / 3;
						float current_fitness = ( ( DefaultPower * PowerInPercent / 100 ) + ( DefaultHealth * HealthInPercent / 100 ) + ( DefaultStamina * StaminaInPercent / 100 ) ) / 3;

						if( ! UseAdvanced )
						{
							max_fitness = DefaultHealth;
							current_fitness = ( DefaultHealth * HealthInPercent / 100 );
						}

						if( max_fitness > 0 )
							current_fitness = ( 100 / max_fitness ) * current_fitness;
						else
							current_fitness = 0;

						if( IsDead )
							current_fitness = 0;

						return FixedPercent( current_fitness );
					}
				}

				public void UpdateBegin( Vector3 _velocity )
				{
					// if respawn is required the creature is dead and doesn't need anymore information 
					if( m_RespawnRequired )
					{
						m_RespawnTimer += Time.deltaTime;
						m_ReactionTimer = 0;
						m_SenseTimer = 0;
					}
					else
					{
						m_ReactionTimer += Time.deltaTime;
						m_SenseTimer += Time.deltaTime;

						if( UseAging )
						{
							m_Age +=  Time.deltaTime;

							if( m_Age >= MaxAge )
							{
								AddDamage( 100 );
								AddStress( 100 );
								AddHunger( 100 );
								AddThirst( 100 );
								AddDebility( 100 );
							}
						}

						if( UseTemperature )
							Temperature = CreatureRegister.EnvironmentInfos.Temperature;

						if( UseTime )
						{
							TimeHour = CreatureRegister.EnvironmentInfos.TimeHour;
							TimeMinutes = CreatureRegister.EnvironmentInfos.TimeMinutes;
							TimeSeconds = CreatureRegister.EnvironmentInfos.TimeSeconds;
						}
					}
				}

				public void FixedUpdate()
				{
				}

				float FixedPercent( float _value )
				{
					/*
					float _f = 0.05f;
					_value = (int)(_value/_f+0.5f)*_f;*/
					if( _value < 0 ) _value = 0;
					if( _value > 100 ) _value = 100;

					return (float)System.Math.Round( _value, 2 );
				}

				float FixedMultiplier( float _value )
				{
					if( _value < 0 ) _value = 0;
					if( _value > 1 ) _value = 1;
					
					return _value;
				}

			}
		}
	}
}
