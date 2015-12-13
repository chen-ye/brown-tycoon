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
	
	public static class EditorRegister
	{	
		private static ICECreatureRegister m_creature_register;

		public static void Print( string _name )
		{
		


			EditorGUILayout.LabelField( _name + "", ICEEditorStyle.LabelBold );
			
			EditorGUILayout.Separator();


			m_creature_register = ICECreatureRegister.Register;
			
			if( m_creature_register == null )
			{
				Info.Warning( Info.REGISTER_MISSING );
		
				ICEEditorLayout.BeginHorizontal();
				ICEEditorLayout.Label( "Add Creature Register", true);
				if (GUILayout.Button("ADD REGISTER", ICEEditorStyle.ButtonLarge ) )
				{
					GameObject _object = new GameObject();
					m_creature_register = _object.AddComponent<ICECreatureRegister>();
					_object.name = "CreatureRegister";

					if( m_creature_register != null )
						m_creature_register.Scan();
				}
				ICEEditorLayout.EndHorizontal( Info.REGISTER );
				
			}
			else if( ! m_creature_register.isActiveAndEnabled )
			{
				Info.Warning( Info.REGISTER_DISABLED );
				
				ICEEditorLayout.BeginHorizontal();
				ICEEditorLayout.Label( "Activate Creature Register", true);
				if (GUILayout.Button("ACTIVATE", ICEEditorStyle.ButtonLarge ) )
				{
					m_creature_register.gameObject.SetActive( true);
				}
				ICEEditorLayout.EndHorizontal(  Info.REGISTER  );
			}
			else
			{
				ICEEditorLayout.BeginHorizontal();
				
					GameObject _registered_object = DrawRegisterPopup( "Creature Register" );
					
					if( _registered_object != null )
					{
						if (GUILayout.Button("SELECT", ICEEditorStyle.ButtonMiddle ) )
							Selection.activeGameObject = _registered_object;					
					}
					else
					{
						if (GUILayout.Button("SCAN", ICEEditorStyle.ButtonMiddle ) )
							m_creature_register.Scan();
					}
					
					if (GUILayout.Button("REGISTER", ICEEditorStyle.ButtonMiddle ) )
						Selection.activeGameObject = m_creature_register.gameObject;
				
				ICEEditorLayout.EndHorizontal( Info.REGISTER );
			}
		}

	

		private static int _register_popup_index = 0;
		private static GameObject DrawRegisterPopup( string _title )
		{
			if( m_creature_register == null )
				m_creature_register = GameObject.FindObjectOfType<ICECreatureRegister>();
			
			if( m_creature_register == null )
			{
				EditorGUILayout.LabelField( _title );
				return null;
			}
			else if( m_creature_register.ReferenceCreatures.Count == 0 )
			{
				EditorGUILayout.LabelField( _title );
				return null;
			}
			else
			{
				List<CreatureReferenceObject> _creatures = m_creature_register.ReferenceCreatures;
				
				string[] _names = new string[_creatures.Count];
				
				if( _register_popup_index > _creatures.Count )
					_register_popup_index = 0;
				
				for(int i=0;i < _creatures.Count ;i++)
				{
					_names[i] = _creatures[i].Name;
				}
				
				_register_popup_index = EditorGUILayout.Popup( _title, _register_popup_index, _names );
				
				return _creatures[ _register_popup_index ].Creature;
				
			}			
		}
	}
}