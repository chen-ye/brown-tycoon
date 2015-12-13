using UnityEngine;
using System.Collections;

using ICE.Creatures.EnumTypes;

namespace ICE.Creatures.Objects
{
	public static class Tools 
	{
		//--------------------------------------------------
		
		public static Vector3 GetRandomRectPosition( Vector3 origin, float x = 10.0f, float z = 10.0f, bool centered = false ) 
		{ 
			Vector3 new_position = origin;
			
			if( centered )
			{
				new_position.x += UnityEngine.Random.Range( -(x/2), (x/2) );
				new_position.z += UnityEngine.Random.Range( -(z/2), (z/2) );
			}
			else
			{
				new_position.x += UnityEngine.Random.Range( 0, x );
				new_position.z += UnityEngine.Random.Range( 0, z );
			}
			
			//new_position.y = GetGroundLevel( new_position );
			
			return new_position;
		}

		//--------------------------------------------------

		public static Vector3 GetRandomPosition( Vector3 _position, float _radius ) { 
			
			if( _radius == 0 )
				return _position;
			
			Vector2 _new_circle_point = UnityEngine.Random.insideUnitCircle * _radius;
			
			Vector3 _new_position = Vector3.zero;
			_new_position.x = _position.x + _new_circle_point.x;
			_new_position.z = _position.z + _new_circle_point.y;
			_new_position.y = 0;

			return _new_position;
			
		}

		public static float GetGroundLevel( Vector3 position, GroundCheckType _type, LayerMask _layerMask )
		{
			if( _type == GroundCheckType.NONE )
				return position.y;
			
			Vector3 pos = position;
			pos.y = 1000;
			
			if( _type == GroundCheckType.RAYCAST )
			{
				RaycastHit hit;
				if (Physics.Raycast( pos, Vector3.down, out hit, Mathf.Infinity, _layerMask.value ) )
					position.y = hit.point.y;
			}
			else 
				position.y = Terrain.activeTerrain.SampleHeight( position );
			
			return position.y;
		}

		public static float GetDirectionAngle( Transform _target, Vector3 _position )
		{
			Vector3 _heading = _position - _target.position;
			float _angle = Tools.GetOffsetAngle( _heading );
			_angle -= _target.transform.eulerAngles.y;
			
			if( _angle < 0 )
				_angle = _angle + 360;
			
			if( _angle > 180 )
				_angle -= 360; 

			return _angle;
		}

		public static Vector3 GetDirectionPosition( Transform _transform, float _angle, float _distance )
		{
			if( _transform == null )
				return Vector3.zero;

			_angle += _transform.eulerAngles.y;

			if( _angle > 360 )
				_angle = _angle - 360;

			Vector3 _world_offset = GetAnglePosition( _transform.position, _angle, _distance );
			
			return _world_offset;
		}

		public static Vector3 GetOffsetPosition( Transform _transform, Vector3 _offset )
		{
			if( _transform == null )
				return Vector3.zero;
			
			Vector3 _local_offset = _offset;
			
			_local_offset.x = _local_offset.x/_transform.lossyScale.x;
			_local_offset.y = _local_offset.y/_transform.lossyScale.y;
			_local_offset.z = _local_offset.z/_transform.lossyScale.z;
			
			return _transform.TransformPoint( _local_offset );
		}

		//returns negative value when left, positive when right, and 0 for forward/backward
		public static float AngleDirection( Vector3 _forward, Vector3 _up, Vector3 _heading )
		{
			Vector3 _perpendicular = Vector3.Cross( _forward, _heading );
			float _direction = Vector3.Dot( _perpendicular , _up  );

			return _direction;
		}   

		public static Vector3 GetAnglePosition( Vector3 _center, float _angle, float _radius )
		{ 
			float _a = _angle * Mathf.PI / 180f;			
			return _center + new Vector3(Mathf.Sin(_a) * _radius, 0, Mathf.Cos(_a) * _radius );
		}

		public static float GetOffsetAngle( Vector3 _offset )
		{ 
			var _local_angle = Mathf.Atan2( _offset.x, _offset.z) * Mathf.Rad2Deg;

			if( _local_angle < 0 )
				_local_angle = 360 + _local_angle;
			
			return _local_angle;
		}

		public static float GetOffsetAngleRaw( Vector3 _offset )
		{ 
			return Mathf.Atan2( _offset.x, _offset.z) * Mathf.Rad2Deg;
		}

		public static float GetPositionAngle( Transform _transform, Vector3 _world_offset )
		{ 
			Vector3 _local_offset = _transform.InverseTransformPoint( _world_offset );

			_local_offset.x = _world_offset.x*_transform.lossyScale.x;
			_local_offset.y = _world_offset.y*_transform.lossyScale.y;
			_local_offset.z = _world_offset.z*_transform.lossyScale.z;	

			var _local_angle = Mathf.Atan2( _local_offset.x, _local_offset.z) * Mathf.Rad2Deg;


			if( _local_angle < 0 )
				_local_angle = 360 + _local_angle;

			return _local_angle;
		}

		//********************************************************************************
		// GetHorizontalDistance
		//********************************************************************************
		public static float GetHorizontalDistance( Vector3 pos1, Vector3 pos2 )
		{
			Vector3 pos_1 = pos1;
			Vector3 pos_2 = pos2;
			
			pos_1.y = 0;
			pos_2.y = 0;
			
			return Vector3.Distance(pos_1, pos_2);
		}


		public static bool IsInLayerMask(GameObject _object, LayerMask _layerMask )
		{
			// Convert the object's layer to a bitfield for comparison
			int _mask = (1 << _object.layer);
			if ((_layerMask.value & _mask) > 0)  // Extra round brackets required!
				return true;
			else
				return false;
		}

		public static bool IsInLayerMask(int layer, int layerMask)
		{
			return (layerMask & 1<<layer) == 0;
		}

		public static LayerMask NamesToMask(params string[] layerNames)
		{
			LayerMask ret = (LayerMask)0;
			foreach(var name in layerNames)
			{
				ret |= (1 << LayerMask.NameToLayer(name));
			}
			return ret;
		}

		public static float FahrenheitToCelsius( float _fahrenheit )
		{
			return (5f / 9f) * (_fahrenheit - 32f);
		}

		public static float CelsiusToFahrenheit( float _celsius )
		{
			return _celsius * (9f / 5f) + 32f;
		}
	}

}
