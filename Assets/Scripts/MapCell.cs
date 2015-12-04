using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapCell {
	
	public enum CellKind {
		UNUSED,
		WALL,
		WALKABLE
	}

	
	public CellKind cellKind; 
	public bool isBorder;
	public Door door;
	public int zoneID;
	
	public MapCell () {
		cellKind = CellKind.UNUSED;
		isBorder = false;
		zoneID = 0; // 0 = zona no asignada.
		door = null;
	}

}
